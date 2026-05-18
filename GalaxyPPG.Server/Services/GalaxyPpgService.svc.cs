using System;
using System.ServiceModel;
using GalaxyPPG.Common.Contracts;
using GalaxyPPG.Common.Exceptions;
using GalaxyPPG.Common.Models;
using GalaxyPPG.Server.Alarms;
using GalaxyPPG.Server.Storage;

namespace GalaxyPPG.Server.Services
{
    public class GalaxyPpgService : IGalaxyPpgService
    {
        public string Ping()
        {
            return "GalaxyPPG service is running.";
        }

        public ProcessingResult SubmitPacket(MeasurementPacket packet)
        {
            ValidatePacket(packet);

            string storageRoot = StoragePathProvider.GetStorageRoot();
            string savedFilePath;
            using (FileMeasurementPacketStorage storage = new FileMeasurementPacketStorage(storageRoot))
            {
                savedFilePath = storage.Save(packet);
            }

            MeasurementAlarmDetector alarmDetector = new MeasurementAlarmDetector(AlarmThresholds.FromConfiguration());
            AlarmFileLogger alarmLogger = new AlarmFileLogger(storageRoot);
            alarmDetector.AlarmDetected += alarmLogger.HandleAlarm;
            int alarmCount = alarmDetector.Analyze(packet);

            return new ProcessingResult(
                true,
                packet.Records.Count,
                string.Format(
                    "Accepted {0} records for participant {1}. Saved to {2}. Detected alarms: {3}.",
                    packet.Records.Count,
                    packet.Participant.ParticipantCode,
                    savedFilePath,
                    alarmCount));
        }

        public ProcessingResult UploadRawCsv(RawCsvUpload upload)
        {
            ValidateRawUpload(upload);

            string storageRoot = StoragePathProvider.GetStorageRoot();
            string savedFilePath;
            using (RawCsvUploadStorage storage = new RawCsvUploadStorage(storageRoot))
            {
                savedFilePath = storage.Save(upload);
            }

            return new ProcessingResult(
                true,
                upload.Content.Length,
                string.Format("Uploaded raw CSV file {0}. Saved to {1}.", upload.FileName, savedFilePath));
        }

        private static void ValidatePacket(MeasurementPacket packet)
        {
            if (packet == null)
            {
                ThrowFault("PACKET_NULL", "Measurement packet cannot be null.");
            }

            if (packet.Participant == null)
            {
                ThrowFault("PARTICIPANT_MISSING", "Participant information is required.");
            }

            if (string.IsNullOrWhiteSpace(packet.Participant.ParticipantCode))
            {
                ThrowFault("PARTICIPANT_CODE_MISSING", "Participant code is required.");
            }

            if (packet.Records == null || packet.Records.Count == 0)
            {
                ThrowFault("RECORDS_MISSING", "Measurement packet must contain at least one sensor record.");
            }
        }

        private static void ValidateRawUpload(RawCsvUpload upload)
        {
            if (upload == null)
            {
                ThrowFault("UPLOAD_NULL", "Raw CSV upload cannot be null.");
            }

            if (upload.Participant == null)
            {
                ThrowFault("PARTICIPANT_MISSING", "Participant information is required.");
            }

            if (string.IsNullOrWhiteSpace(upload.Participant.ParticipantCode))
            {
                ThrowFault("PARTICIPANT_CODE_MISSING", "Participant code is required.");
            }

            if (string.IsNullOrWhiteSpace(upload.FileName))
            {
                ThrowFault("FILE_NAME_MISSING", "Raw CSV file name is required.");
            }

            if (upload.Content == null || upload.Content.Length == 0)
            {
                ThrowFault("UPLOAD_CONTENT_MISSING", "Raw CSV content is required.");
            }
        }

        private static void ThrowFault(string code, string message)
        {
            throw new FaultException<GalaxyPpgFault>(
                new GalaxyPpgFault(code, message),
                new FaultReason(message));
        }
    }
}
