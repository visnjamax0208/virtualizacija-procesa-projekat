using System.ServiceModel;
using GalaxyPPG.Common.Exceptions;
using GalaxyPPG.Common.Models;

namespace GalaxyPPG.Common.Contracts
{
    [ServiceContract]
    public interface IGalaxyPpgService
    {
        [OperationContract]
        string Ping();

        [OperationContract]
        [FaultContract(typeof(GalaxyPpgFault))]
        ProcessingResult SubmitPacket(MeasurementPacket packet);
    }
}
