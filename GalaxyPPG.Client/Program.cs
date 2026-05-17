using System;
using System.Collections.Generic;
using System.Configuration;
using GalaxyPPG.Common.Models;

namespace GalaxyPPG.Client
{
    internal class Program
    {
        private static void Main()
        {
            Console.WriteLine("GalaxyPPG client");
            Console.WriteLine("Service URL: " + ConfigurationManager.AppSettings["GalaxyPpgServiceUrl"]);

            MeasurementPacket packet = CreateDemoPacket();

            Console.WriteLine("Prepared demo packet:");
            Console.WriteLine("Participant: " + packet.Participant.ParticipantCode);
            Console.WriteLine("Device: " + packet.Participant.DeviceName);
            Console.WriteLine("Records: " + packet.Records.Count);
            Console.WriteLine("WCF sending will be added in the next step.");
        }

        private static MeasurementPacket CreateDemoPacket()
        {
            return new MeasurementPacket(
                new ParticipantInfo("P01", "GalaxyWatch", "DemoSession"),
                new List<SensorRecord>
                {
                    new SensorRecord(SensorType.Ppg, 1716000000000, 0.82, "a.u.", "BVP.csv"),
                    new SensorRecord(SensorType.HeartRate, 1716000001000, 74.0, "bpm", "HR.csv"),
                    new SensorRecord(SensorType.Accelerometer, 1716000002000, 0.18, "g", "ACC.csv")
                });
        }
    }
}
