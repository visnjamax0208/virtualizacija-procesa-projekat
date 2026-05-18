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
            string line = string.Format(
                CultureInfo.InvariantCulture,
                "{0:o}|{1}|{2}|{3}|{4}|{5}|{6}",
                e.CreatedAtUtc,
                e.Participant.ParticipantCode,
                e.Participant.DeviceName,
                e.Code,
                e.Record.SensorType,
                e.Record.Value,
                e.Message);

            using (FileStream fileStream = new FileStream(logFilePath, FileMode.Append, FileAccess.Write, FileShare.Read))
            using (StreamWriter writer = new StreamWriter(fileStream, Encoding.UTF8))
            {
                writer.WriteLine(line);
            }
        }
    }
}
