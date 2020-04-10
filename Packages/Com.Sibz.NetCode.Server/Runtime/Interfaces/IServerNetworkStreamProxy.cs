using Unity.Networking.Transport;

namespace Sibz.NetCode.Server
{
    public interface IServerNetworkStreamProxy
    {
        bool Listen(NetworkEndPoint endPoint);
    }
}