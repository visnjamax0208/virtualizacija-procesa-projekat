using System;
using GalaxyPPG.Common.Models;

namespace GalaxyPPG.Server.Alarms
{
    public delegate void MeasurementAlarmEventHandler(object sender, AlarmEventArgs e);

    public class MeasurementAlarmDetector
    {
        private readonly AlarmThresholds thresholds;

        public MeasurementAlarmDetector(AlarmThresholds thresholds)
        {
            if (thresholds == null)
            {
                throw new ArgumentNullException("thresholds");
            }

            this.thresholds = thresholds;
        }

        public event MeasurementAlarmEventHandler AlarmDetected;

        public int Analyze(MeasurementPacket packet)
        {
            int alarmCount = 0;

            foreach (SensorRecord record in packet.Records)
            {
                if (IsAlarm(record, out string code, out string message))
                {
                    alarmCount++;
                    OnAlarmDetected(new AlarmEventArgs(code, message, record, packet.Participant));
                }
            }

            return alarmCount;
        }

        protected virtual void OnAlarmDetected(AlarmEventArgs e)
        {
            MeasurementAlarmEventHandler handler = AlarmDetected;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        private bool IsAlarm(SensorRecord record, out string code, out string message)
        {
            code = string.Empty;
            message = string.Empty;

            if (record.SensorType == SensorType.Ppg && record.Value < thresholds.MinPpgValue)
            {
                code = "WEAK_PPG_SIGNAL";
                message = string.Format("Weak PPG signal detected: {0}.", record.Value);
                return true;
            }

            if (record.SensorType == SensorType.Accelerometer && record.Value > thresholds.MaxAccelerationMagnitude)
            {
                code = "EXCESSIVE_MOVEMENT";
                message = string.Format("Excessive movement detected: {0}.", record.Value);
                return true;
            }

            if (record.SensorType == SensorType.HeartRate &&
                (record.Value < thresholds.MinHeartRate || record.Value > thresholds.MaxHeartRate))
            {
                code = "HEART_RATE_OUT_OF_RANGE";
                message = string.Format("Heart rate out of range: {0}.", record.Value);
                return true;
            }

            return false;
        }
    }
}
