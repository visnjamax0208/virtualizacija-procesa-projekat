using System.Configuration;
using System.Globalization;

namespace GalaxyPPG.Server.Alarms
{
    public class AlarmThresholds
    {
        public AlarmThresholds(double minPpgValue, double maxAccelerationMagnitude, double minHeartRate, double maxHeartRate)
        {
            MinPpgValue = minPpgValue;
            MaxAccelerationMagnitude = maxAccelerationMagnitude;
            MinHeartRate = minHeartRate;
            MaxHeartRate = maxHeartRate;
        }

        public double MinPpgValue { get; private set; }

        public double MaxAccelerationMagnitude { get; private set; }

        public double MinHeartRate { get; private set; }

        public double MaxHeartRate { get; private set; }

        public static AlarmThresholds FromConfiguration()
        {
            return new AlarmThresholds(
                ReadDouble("GalaxyPpgMinPpgValue", 0.80),
                ReadDouble("GalaxyPpgMaxAccelerationMagnitude", 1.25),
                ReadDouble("GalaxyPpgMinHeartRate", 45.0),
                ReadDouble("GalaxyPpgMaxHeartRate", 120.0));
        }

        private static double ReadDouble(string key, double defaultValue)
        {
            string value = ConfigurationManager.AppSettings[key];
            if (string.IsNullOrWhiteSpace(value))
            {
                return defaultValue;
            }

            return double.Parse(value, CultureInfo.InvariantCulture);
        }
    }
}
