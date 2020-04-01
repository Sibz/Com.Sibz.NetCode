using Sibz.EntityEvents;
using Unity.Networking.Transport;

namespace Sibz.NetCode
{
    public struct ServerConnect : IEventComponentData
    {
        public NetworkEndPoint EndPoint;
        public int Timeout;
        public float InitialTime;

        public float TimeoutTime => InitialTime + Timeout;
    }
}