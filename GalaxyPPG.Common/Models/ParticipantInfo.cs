using System.Runtime.Serialization;

namespace GalaxyPPG.Common.Models
{
    [DataContract]
    public class ParticipantInfo
    {
        public ParticipantInfo()
        {
        }

        public ParticipantInfo(string participantCode, string deviceName, string sessionName)
        {
            ParticipantCode = participantCode;
            DeviceName = deviceName;
            SessionName = sessionName;
        }

        [DataMember]
        public string ParticipantCode { get; set; }

        [DataMember]
        public string DeviceName { get; set; }

        [DataMember]
        public string SessionName { get; set; }
    }
}
