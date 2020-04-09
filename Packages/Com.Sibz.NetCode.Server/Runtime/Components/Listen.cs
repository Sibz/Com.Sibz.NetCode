using Sibz.EntityEvents;
using Unity.Networking.Transport;

namespace Sibz.NetCode.Server
{
    public struct Listen : IEventComponentData
    {
        public NetworkEndPoint EndPoint;
    }
}