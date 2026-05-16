using System.Runtime.Serialization;

namespace GalaxyPPG.Common.Models
{
    [DataContract]
    public enum SensorType
    {
        [EnumMember]
        Unknown = 0,

        [EnumMember]
        Ppg = 1,

        [EnumMember]
        HeartRate = 2,

        [EnumMember]
        Ibi = 3,

        [EnumMember]
        Accelerometer = 4,

        [EnumMember]
        SkinTemperature = 5,

        [EnumMember]
        Ecg = 6
    }
}
