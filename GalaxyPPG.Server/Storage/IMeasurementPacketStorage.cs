using GalaxyPPG.Common.Models;

namespace GalaxyPPG.Server.Storage
{
    public interface IMeasurementPacketStorage
    {
        string Save(MeasurementPacket packet);
    }
}
