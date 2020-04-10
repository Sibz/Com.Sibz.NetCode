using Sibz.EntityEvents;
using Unity.Entities;

namespace Sibz.NetCode.Server
{
    public struct ClientConnectedEvent : IEventComponentData
    {
        public Entity ConnectionEntity;
    }
}