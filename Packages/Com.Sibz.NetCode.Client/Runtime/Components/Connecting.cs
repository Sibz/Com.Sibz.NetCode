using Unity.Entities;
using Unity.Networking.Transport;

namespace Sibz.NetCode.Client
{
    public struct Connecting : IComponentData
    {
        public NetworkEndPoint EndPoint;
        public NetworkState State;
        public float Timeout;
    }
}