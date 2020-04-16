using Sibz.EntityEvents;

namespace Sibz.NetCode.Client
{
    public struct DisconnectClientFailedEvent : IEventComponentData
    {
        public int Id;
    }
}