using System.Runtime.Serialization;

namespace GalaxyPPG.Common.Models
{
    [DataContract]
    public class ProcessingResult
    {
        public ProcessingResult()
        {
        }

        public ProcessingResult(bool success, int acceptedRecords, string message)
        {
            Success = success;
            AcceptedRecords = acceptedRecords;
            Message = message;
        }

        [DataMember]
        public bool Success { get; set; }

        [DataMember]
        public int AcceptedRecords { get; set; }

        [DataMember]
        public string Message { get; set; }
    }
}
