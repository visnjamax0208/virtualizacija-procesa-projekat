using System;
using System.Globalization;
using System.IO;
using System.Text;
using GalaxyPPG.Common.Models;

namespace GalaxyPPG.Server.Storage
{
    public class FileMeasurementPacketStorage : IMeasurementPacketStorage, IDisposable
    {
        private readonly string rootDirectory;
        private bool disposed;

        public FileMeasurementPacketStorage(string rootDirectory)
        {
            if (string.IsNullOrWhiteSpace(rootDirectory))
            {
                throw new ArgumentException("Storage root directory is required.", "rootDirectory");
            }

            this.rootDirectory = rootDirectory;
        }

        public string Save(MeasurementPacket packet)
        {
            ThrowIfDisposed();

            string participantCode = SafePathPart(packet.Participant.ParticipantCode);
            string deviceName = SafePathPart(packet.Participant.DeviceName);
            string sessionName = SafePathPart(packet.Participant.SessionName);

            string packetDirectory = Path.Combine(rootDirectory, participantCode, deviceName);
            Directory.CreateDirectory(packetDirectory);

            string fileName = string.Format(
                CultureInfo.InvariantCulture,
                "{0}_{1:yyyyMMdd_HHmmssfff}.csv",
                sessionName,
                DateTime.UtcNow);

            string filePath = Path.Combine(packetDirectory, fileName);

            using (FileStream fileStream = new FileStream(filePath, FileMode.CreateNew, FileAccess.Write, FileShare.Read))
            using (StreamWriter writer = new StreamWriter(fileStream, Encoding.UTF8))
            {
                writer.WriteLine("sensorType,timestampUnixMs,value,unit,sourceFile");

                foreach (SensorRecord record in packet.Records)
                {
                    writer.WriteLine(
                        "{0},{1},{2},{3},{4}",
                        record.SensorType,
                        record.TimestampUnixMs,
                        record.Value.ToString(CultureInfo.InvariantCulture),
                        EscapeCsv(record.Unit),
                        EscapeCsv(record.SourceFile));
                }
            }

            return filePath;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
            {
                return;
            }

            disposed = true;
        }

        private void ThrowIfDisposed()
        {
            if (disposed)
            {
                throw new ObjectDisposedException(GetType().FullName);
            }
        }

        private static string SafePathPart(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return "unknown";
            }

            string safeValue = value.Trim();
            foreach (char invalidChar in Path.GetInvalidFileNameChars())
            {
                safeValue = safeValue.Replace(invalidChar, '_');
            }

            return safeValue;
        }

        private static string EscapeCsv(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return string.Empty;
            }

            if (value.Contains(",") || value.Contains("\"") || value.Contains("\r") || value.Contains("\n"))
            {
                return "\"" + value.Replace("\"", "\"\"") + "\"";
            }

            return value;
        }
    }
}
