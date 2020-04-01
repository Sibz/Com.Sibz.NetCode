using Sibz.EntityEvents;
using Unity.Networking.Transport;

namespace Sibz.NetCode.Server
{
    public struct ServerConnect : IEventComponentData
    {
        public NetworkEndPoint EndPoint;
        public int Timeout;
        public float InitialTime;

        public float TimeoutTime => InitialTime + Timeout;
    }
}