using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using GalaxyPPG.Common.Models;

namespace GalaxyPPG.Client.Csv
{
    public class CsvSensorFileParser : IDisposable
    {
        private bool disposed;

        public IList<SensorRecord> ParseFile(string filePath)
        {
            ThrowIfDisposed();

            if (string.IsNullOrWhiteSpace(filePath))
            {
                throw new ArgumentException("CSV file path is required.", "filePath");
            }

            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException("CSV file was not found.", filePath);
            }

            SensorType sensorType = ResolveSensorType(filePath);
            string unit = ResolveUnit(sensorType);
            List<SensorRecord> records = new List<SensorRecord>();

            using (FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read))
            using (StreamReader reader = new StreamReader(fileStream))
            {
                string headerLine = reader.ReadLine();
                if (string.IsNullOrWhiteSpace(headerLine))
                {
                    return records;
                }

                string[] headers = SplitCsvLine(headerLine);
                string line;

                while ((line = reader.ReadLine()) != null)
                {
                    if (string.IsNullOrWhiteSpace(line))
                    {
                        continue;
                    }

                    string[] values = SplitCsvLine(line);
                    SensorRecord record = CreateRecord(sensorType, unit, filePath, headers, values);
                    if (record != null)
                    {
                        records.Add(record);
                    }
                }
            }

            return records;
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

        private static SensorRecord CreateRecord(SensorType sensorType, string unit, string filePath, string[] headers, string[] values)
        {
            int timestampIndex = FindColumn(headers, "timestamp", "time", "unixTime");
            if (timestampIndex < 0 || timestampIndex >= values.Length)
            {
                return null;
            }

            long timestamp = TimestampNormalizer.ToUnixMilliseconds(values[timestampIndex]);

            double value;
            if (sensorType == SensorType.Accelerometer)
            {
                value = ReadAccelerationMagnitude(headers, values);
            }
            else
            {
                int valueIndex = FindColumn(headers, "value", "hr", "bvp", "ppg");
                if (valueIndex < 0 || valueIndex >= values.Length)
                {
                    return null;
                }

                value = double.Parse(values[valueIndex], CultureInfo.InvariantCulture);
            }

            return new SensorRecord(sensorType, timestamp, value, unit, Path.GetFileName(filePath));
        }

        private static double ReadAccelerationMagnitude(string[] headers, string[] values)
        {
            double x = ReadDouble(headers, values, "x");
            double y = ReadDouble(headers, values, "y");
            double z = ReadDouble(headers, values, "z");

            return Math.Sqrt((x * x) + (y * y) + (z * z));
        }

        private static double ReadDouble(string[] headers, string[] values, string columnName)
        {
            int index = FindColumn(headers, columnName);
            if (index < 0 || index >= values.Length)
            {
                return 0;
            }

            return double.Parse(values[index], CultureInfo.InvariantCulture);
        }

        private static int FindColumn(string[] headers, params string[] names)
        {
            for (int i = 0; i < headers.Length; i++)
            {
                string header = headers[i].Trim();
                foreach (string name in names)
                {
                    if (string.Equals(header, name, StringComparison.OrdinalIgnoreCase))
                    {
                        return i;
                    }
                }
            }

            return -1;
        }

        private static SensorType ResolveSensorType(string filePath)
        {
            string fileName = Path.GetFileNameWithoutExtension(filePath);

            if (string.Equals(fileName, "HR", StringComparison.OrdinalIgnoreCase))
            {
                return SensorType.HeartRate;
            }

            if (string.Equals(fileName, "BVP", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(fileName, "PPG", StringComparison.OrdinalIgnoreCase))
            {
                return SensorType.Ppg;
            }

            if (string.Equals(fileName, "ACC", StringComparison.OrdinalIgnoreCase))
            {
                return SensorType.Accelerometer;
            }

            return SensorType.Unknown;
        }

        private static string ResolveUnit(SensorType sensorType)
        {
            switch (sensorType)
            {
                case SensorType.HeartRate:
                    return "bpm";
                case SensorType.Ppg:
                    return "a.u.";
                case SensorType.Accelerometer:
                    return "g";
                default:
                    return string.Empty;
            }
        }

        private static string[] SplitCsvLine(string line)
        {
            return line.Split(',');
        }

        private void ThrowIfDisposed()
        {
            if (disposed)
            {
                throw new ObjectDisposedException(GetType().FullName);
            }
        }
    }
}
