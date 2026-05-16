using System.Collections.Generic;
using System.Runtime.Serialization;

namespace GalaxyPPG.Common.Models
{
    [DataContract]
    public class MeasurementPacket
    {
        public MeasurementPacket(ParticipantInfo participant, List<SensorRecord> records)
        {
            Participant = participant;
            Records = records;
        }

        [DataMember]
        public ParticipantInfo Participant { get; set; }

        [DataMember]
        public List<SensorRecord> Records { get; set; }
    }
}
