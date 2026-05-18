using System;
using GalaxyPPG.Common.Models;

namespace GalaxyPPG.Server.Alarms
{
    public class AlarmEventArgs : EventArgs
    {
        public AlarmEventArgs(string code, string message, SensorRecord record, ParticipantInfo participant)
        {
            Code = code;
            Message = message;
            Record = record;
            Participant = participant;
            CreatedAtUtc = DateTime.UtcNow;
        }

        public string Code { get; private set; }

        public string Message { get; private set; }

        public SensorRecord Record { get; private set; }

        public ParticipantInfo Participant { get; private set; }

        public DateTime CreatedAtUtc { get; private set; }
    }
}
