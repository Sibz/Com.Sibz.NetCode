﻿using System;
using Unity.Entities;
using Unity.Jobs;

namespace Sibz.EntityEvents
{
    public static class WorldEventExtensions
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

        public static EnqueueEventJobPart<T> GetEnqueueEventJobPart<T>(this World world, T eventData = default)
            where T : struct, IEventComponentData
        {
            EventComponentSystem system = world.GetExistingSystem<EventComponentSystem>();
            if (system is null)
            {
                throw new NullReferenceException($"{nameof(EventComponentSystem)} is null. Unable to enqueue event");
            }

            return system.GetJobPart(eventData);
        }

        public static void EventSystemAddJobDependency(this World world, JobHandle job)
        {
            EventComponentSystem system = world.GetExistingSystem<EventComponentSystem>();
            if (system is null)
            {
                throw new NullReferenceException($"{nameof(EventComponentSystem)} is null. Unable to enqueue event");
            }

            system.ConcurrentBufferAddJobDependency(job);
        }
    }
}