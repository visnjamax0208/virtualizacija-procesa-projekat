using System;
using System.ServiceModel;
using GalaxyPPG.Common.Contracts;
using GalaxyPPG.Common.Models;

namespace GalaxyPPG.Client.ServiceClients
{
    public class GalaxyPpgServiceClient : IDisposable
    {
        private readonly ChannelFactory<IGalaxyPpgService> channelFactory;
        private readonly IGalaxyPpgService channel;
        private bool disposed;

        public GalaxyPpgServiceClient(string serviceUrl)
        {
            if (string.IsNullOrWhiteSpace(serviceUrl))
            {
                throw new ArgumentException("Service URL is required.", "serviceUrl");
            }

            BasicHttpBinding binding = new BasicHttpBinding();
            EndpointAddress endpointAddress = new EndpointAddress(serviceUrl);

            channelFactory = new ChannelFactory<IGalaxyPpgService>(binding, endpointAddress);
            channel = channelFactory.CreateChannel();
        }

        public string Ping()
        {
            ThrowIfDisposed();
            return channel.Ping();
        }

        public ProcessingResult SubmitPacket(MeasurementPacket packet)
        {
            ThrowIfDisposed();
            return channel.SubmitPacket(packet);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
            {
                return;
            }

            if (disposing)
            {
                CloseCommunicationObject(channel as ICommunicationObject);
                CloseCommunicationObject(channelFactory);
            }

            disposed = true;
        }

        private static void CloseCommunicationObject(ICommunicationObject communicationObject)
        {
            if (communicationObject == null)
            {
                return;
            }

            try
            {
                if (communicationObject.State == CommunicationState.Faulted)
                {
                    communicationObject.Abort();
                }
                else
                {
                    communicationObject.Close();
                }
            }
            catch (CommunicationException)
            {
                communicationObject.Abort();
            }
            catch (TimeoutException)
            {
                communicationObject.Abort();
            }
        }

        private void ThrowIfDisposed()
        {
            if (disposed)
            {
                throw new ObjectDisposedException(GetType().FullName);
            }
        }
    }
}
