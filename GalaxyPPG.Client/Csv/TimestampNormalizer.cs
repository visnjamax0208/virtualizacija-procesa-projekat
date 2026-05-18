using System.Globalization;

namespace GalaxyPPG.Client.Csv
{
    public static class TimestampNormalizer
    {
        public static long ToUnixMilliseconds(string rawTimestamp)
        {
            long timestamp = long.Parse(rawTimestamp, CultureInfo.InvariantCulture);

            if (timestamp > 99999999999999999L)
            {
                return timestamp / 1000000L;
            }

            if (timestamp > 99999999999999L)
            {
                return timestamp / 1000L;
            }

            return timestamp;
        }
    }
}
