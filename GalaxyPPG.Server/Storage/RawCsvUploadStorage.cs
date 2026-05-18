using System;
using System.IO;
using GalaxyPPG.Common.Models;

namespace GalaxyPPG.Server.Storage
{
    public class RawCsvUploadStorage : IDisposable
    {
        private readonly string rootDirectory;
        private bool disposed;

        public RawCsvUploadStorage(string rootDirectory)
        {
            if (string.IsNullOrWhiteSpace(rootDirectory))
            {
                throw new ArgumentException("Storage root directory is required.", "rootDirectory");
            }

            this.rootDirectory = rootDirectory;
        }

        public string Save(RawCsvUpload upload)
        {
            ThrowIfDisposed();

            string participantCode = SafePathPart(upload.Participant.ParticipantCode);
            string deviceName = SafePathPart(upload.Participant.DeviceName);
            string uploadDirectory = Path.Combine(rootDirectory, "raw", participantCode, deviceName);
            Directory.CreateDirectory(uploadDirectory);

            string fileName = string.Format("{0:yyyyMMdd_HHmmssfff}_{1}", DateTime.UtcNow, SafePathPart(upload.FileName));
            string filePath = Path.Combine(uploadDirectory, fileName);

            using (MemoryStream memoryStream = new MemoryStream(upload.Content))
            using (FileStream fileStream = new FileStream(filePath, FileMode.CreateNew, FileAccess.Write, FileShare.Read))
            {
                memoryStream.CopyTo(fileStream);
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
    }
}
