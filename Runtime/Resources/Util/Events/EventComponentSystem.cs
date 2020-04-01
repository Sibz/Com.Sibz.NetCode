using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Sibz.CommandBufferHelpers;
using Unity.Entities;
using Unity.Jobs;

namespace Sibz.EntityEvents
{
    [AlwaysUpdateSystem]
    public class EventComponentSystem : JobComponentSystem
    {
        private EntityQuery allEventComponentsQuery;
        private BeginInitCommandBuffer commandBuffer;
        private readonly Queue<object> eventQueue = new Queue<object>();

        // ReSharper disable once MemberCanBePrivate.Global
        public static readonly ComponentType[] EventTypes = GetEventTypes();

        private static ComponentType[] GetEventTypes()
        {
            var types = new List<ComponentType>();
            foreach (Assembly a in AppDomain.CurrentDomain.GetAssemblies())
            {
                types.AddRange(a.GetTypes()
                    .Where(x => x.IsValueType && x.GetInterfaces().Contains(typeof(IEventComponentData)))
                    .Select(t => (ComponentType) t));
            }

            return types.ToArray();
        }


        public void EnqueueEvent(object eventData)
        {
            eventQueue.Enqueue(eventData);
        }

        protected override void OnCreate()
        {
            allEventComponentsQuery = GetEntityQuery(new EntityQueryDesc {Any = EventTypes});
            commandBuffer = new BeginInitCommandBuffer(World);
        }

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            commandBuffer.Buffer.DestroyEntity(allEventComponentsQuery);

            while (eventQueue.Count > 0)
            {
                CreateSingletonFromObject(eventQueue.Dequeue());
            }

            return inputDeps;
        }

        private void CreateSingletonFromObject(object obj)
        {
            var method = typeof(EventComponentSystem).GetMethod(nameof(CreateSingleton),
                BindingFlags.Instance | BindingFlags.NonPublic);
            if (method is null)
            {
                throw new NullReferenceException($"Unable to get method {nameof(CreateSingleton)}");
            }

            method.MakeGenericMethod(obj.GetType()).Invoke(this, new object[1] { obj });
        }

        // ReSharper disable once UnusedMember.Local
        private void CreateSingleton<T>(T obj)
            where T : struct, IComponentData
        {
            commandBuffer.Buffer.CreateSingleton(obj);
        }
    }
}