using Unity.Entities;
using Unity.Networking.Transport;

namespace Sibz.NetCode.Client
{
    public struct ClientConnect : IComponentData
    {
        public NetworkEndPoint EndPoint;
        public int Timeout;
        public float InitialTime;
        public NetworkState State;

        public float TimeoutTime => InitialTime + Timeout;
    }
}