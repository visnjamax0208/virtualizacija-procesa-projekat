using System;
using System.ServiceModel;
using GalaxyPPG.Common.Contracts;
using GalaxyPPG.Common.Exceptions;
using GalaxyPPG.Common.Models;
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

            string savedFilePath;
            using (FileMeasurementPacketStorage storage = new FileMeasurementPacketStorage(StoragePathProvider.GetStorageRoot()))
            {
                savedFilePath = storage.Save(packet);
            }

            return new ProcessingResult(
                true,
                packet.Records.Count,
                string.Format(
                    "Accepted {0} records for participant {1}. Saved to {2}.",
                    packet.Records.Count,
                    packet.Participant.ParticipantCode,
                    savedFilePath));
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

        private static void ThrowFault(string code, string message)
        {
            throw new FaultException<GalaxyPpgFault>(
                new GalaxyPpgFault(code, message),
                new FaultReason(message));
        }
    }
}
