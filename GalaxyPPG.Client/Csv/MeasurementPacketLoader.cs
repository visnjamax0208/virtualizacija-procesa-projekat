using System.Collections.Generic;
using System.IO;
using GalaxyPPG.Common.Models;

namespace GalaxyPPG.Client.Csv
{
    public class MeasurementPacketLoader
    {
        public MeasurementPacket LoadFromDirectory(string directoryPath, ParticipantInfo participant)
        {
            List<SensorRecord> records = new List<SensorRecord>();

            using (CsvSensorFileParser parser = new CsvSensorFileParser())
            {
                foreach (string filePath in Directory.GetFiles(directoryPath, "*.csv", SearchOption.AllDirectories))
                {
                    string fileName = Path.GetFileName(filePath);
                    if (fileName == "HR.csv" || fileName == "BVP.csv" || fileName == "ACC.csv")
                    {
                        records.AddRange(parser.ParseFile(filePath));
                    }
                }
            }

            return new MeasurementPacket(participant, records);
        }
    }
}
