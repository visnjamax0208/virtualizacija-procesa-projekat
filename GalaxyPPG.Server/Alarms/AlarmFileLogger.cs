using System.Globalization;
using System.IO;
using System.Text;

namespace GalaxyPPG.Server.Alarms
{
    public class AlarmFileLogger
    {
        private readonly string logFilePath;

        public AlarmFileLogger(string storageRoot)
        {
            Directory.CreateDirectory(storageRoot);
            logFilePath = Path.Combine(storageRoot, "alarms.log");
        }

        public void HandleAlarm(object sender, AlarmEventArgs e)
        {
            using (FileStream fileStream = new FileStream(logFilePath, FileMode.Append, FileAccess.Write, FileShare.Read))
            using (StreamWriter writer = new StreamWriter(fileStream, Encoding.UTF8))
            {
                writer.WriteLine("Time: {0:yyyy-MM-dd HH:mm:ss} UTC", e.CreatedAtUtc);
                writer.WriteLine("Participant: {0}", e.Participant.ParticipantCode);
                writer.WriteLine("Device: {0}", e.Participant.DeviceName);
                writer.WriteLine("Alarm: {0}", e.Code);
                writer.WriteLine("Sensor: {0}", e.Record.SensorType);
                writer.WriteLine("Value: {0}", e.Record.Value.ToString("0.###", CultureInfo.InvariantCulture));
                writer.WriteLine("Message: {0}", e.Message);
                writer.WriteLine(new string('-', 60));
            }
        }
    }
}
