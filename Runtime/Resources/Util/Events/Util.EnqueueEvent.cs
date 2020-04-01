using System;
using Sibz.EntityEvents;
using Unity.Entities;

namespace Sibz
{
    public static partial class Util
    {
        public static void EnqueueEvent<T>(this World world, T eventData = default)
            where T : struct, IEventComponentData
        {
            EventComponentSystem system = world.GetExistingSystem<EventComponentSystem>();
            if (system is null)
            {
                throw new NullReferenceException($"{nameof(EventComponentSystem)} is null. Unable to enqueue event");
            }
            // ReSharper disable once HeapView.BoxingAllocation
            system.EnqueueEvent(eventData);
        }
    }
}