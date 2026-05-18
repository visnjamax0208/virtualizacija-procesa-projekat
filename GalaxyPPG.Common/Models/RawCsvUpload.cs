using System.Runtime.Serialization;

namespace GalaxyPPG.Common.Models
{
    [DataContract]
    public class RawCsvUpload
    {
        public RawCsvUpload()
        {
        }

        public RawCsvUpload(ParticipantInfo participant, string fileName, byte[] content)
        {
            Participant = participant;
            FileName = fileName;
            Content = content;
        }

        [DataMember]
        public ParticipantInfo Participant { get; set; }

        [DataMember]
        public string FileName { get; set; }

        [DataMember]
        public byte[] Content { get; set; }
    }
}
