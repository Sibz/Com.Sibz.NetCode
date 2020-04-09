using Unity.Networking.Transport;

namespace Sibz.NetCode
{
    public interface IServerNetworkStreamProxy
    {
        bool Listen(NetworkEndPoint endPoint);
    }
}