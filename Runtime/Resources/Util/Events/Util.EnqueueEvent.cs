using Sibz.EntityEvents;
using Unity.Entities;

namespace Sibz
{
    public static partial class Util
    {
        public static void EnqueueEvent<T>(this World world, T eventData = default)
            where T : struct, IEventComponentData
        {
            // ReSharper disable once HeapView.BoxingAllocation
            world.GetOrCreateSystem<EventComponentSystem>().EnqueueEvent(eventData);
        }
    }
}