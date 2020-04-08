using Unity.Entities;
using Unity.Networking.Transport;

namespace Sibz.NetCode.Client
{
    public interface IClientNetworkStreamSystemProxy
    {
        Entity Connect(NetworkEndPoint endPoint);
    }
}