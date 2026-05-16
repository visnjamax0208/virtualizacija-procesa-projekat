using System.Runtime.Serialization;

namespace GalaxyPPG.Common.Exceptions
{
    [DataContract]
    public class GalaxyPpgFault
    {
        public GalaxyPpgFault(string code, string message)
        {
            Code = code;
            Message = message;
        }

        [DataMember]
        public string Code { get; set; }

        [DataMember]
        public string Message { get; set; }
    }
}
