using Sibz.EntityEvents;

namespace Sibz.NetCode.Server
{
    public struct ClientDisconnectedEvent : IEventComponentData
    {
        public int NetworkId;
    }
}