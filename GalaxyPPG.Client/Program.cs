using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.ServiceModel;
using GalaxyPPG.Common.Models;
using GalaxyPPG.Client.Csv;
using GalaxyPPG.Client.ServiceClients;

namespace GalaxyPPG.Client
{
    internal class Program
    {
        private static void Main()
        {
            string serviceUrl = ConfigurationManager.AppSettings["GalaxyPpgServiceUrl"];

            Console.WriteLine("GalaxyPPG client");
            Console.WriteLine("Service URL: " + serviceUrl);

            MeasurementPacket packet = CreatePacket();

            Console.WriteLine("Prepared demo packet:");
            Console.WriteLine("Participant: " + packet.Participant.ParticipantCode);
            Console.WriteLine("Device: " + packet.Participant.DeviceName);
            Console.WriteLine("Records: " + packet.Records.Count);

            try
            {
                using (GalaxyPpgServiceClient client = new GalaxyPpgServiceClient(serviceUrl))
                {
                    Console.WriteLine("Server response: " + client.Ping());

                    ProcessingResult result = client.SubmitPacket(packet);
                    Console.WriteLine("Submit success: " + result.Success);
                    Console.WriteLine("Accepted records: " + result.AcceptedRecords);
                    Console.WriteLine("Message: " + result.Message);
                }
            }
            catch (EndpointNotFoundException)
            {
                Console.WriteLine("Server is not available. Start GalaxyPPG.Server and try again.");
            }
            catch (FaultException exception)
            {
                Console.WriteLine("Server rejected the packet: " + exception.Message);
            }
            catch (CommunicationException exception)
            {
                Console.WriteLine("Communication error: " + exception.Message);
            }
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

        private static MeasurementPacket CreatePacket()
        {
            string sampleDataDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "SampleData", "P01", "GalaxyWatch");
            if (!Directory.Exists(sampleDataDirectory))
            {
                return CreateDemoPacket();
            }

            ParticipantInfo participant = new ParticipantInfo("P01", "GalaxyWatch", "SampleData");
            MeasurementPacketLoader loader = new MeasurementPacketLoader();
            MeasurementPacket packet = loader.LoadFromDirectory(sampleDataDirectory, participant);

            if (packet.Records == null || packet.Records.Count == 0)
            {
                return CreateDemoPacket();
            }

            return packet;
        }
    }
}
