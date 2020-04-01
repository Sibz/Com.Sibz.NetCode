using Sibz.EntityEvents;
using Unity.Entities;

namespace Sibz.NetCode
{
    public struct NetworkStateChangeEvent : IEventComponentData
    {
        public Entity StatusEntity;
    }
}