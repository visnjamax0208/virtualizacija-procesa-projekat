using System.Runtime.Serialization;

namespace GalaxyPPG.Common.Models
{
    [DataContract]
    public class SensorRecord
    {
        public SensorRecord(SensorType sensorType, long timestampUnixMs, double value, string unit, string sourceFile)
        {
            SensorType = sensorType;
            TimestampUnixMs = timestampUnixMs;
            Value = value;
            Unit = unit;
            SourceFile = sourceFile;
        }

        [DataMember]
        public SensorType SensorType { get; set; }

        [DataMember]
        public long TimestampUnixMs { get; set; }

        [DataMember]
        public double Value { get; set; }

        [DataMember]
        public string Unit { get; set; }

        [DataMember]
        public string SourceFile { get; set; }
    }
}
