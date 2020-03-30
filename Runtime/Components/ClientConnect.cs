using Unity.Entities;
using Unity.Networking.Transport;

namespace Sibz.NetCode
{
    public struct ClientConnect : IComponentData
    {
        public NetworkEndPoint EndPoint;
        public int Timeout;
        public float InitialTime;
        public ClientConnectionState State;

        public float TimeoutTime => InitialTime + Timeout;
    }
}