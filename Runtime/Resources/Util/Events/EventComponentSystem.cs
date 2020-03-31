using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Sibz.CommandBufferHelpers;
using Unity.Entities;
using Unity.Jobs;

namespace Sibz.EntityEvents
{
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
            foreach (object eventComponentData in eventQueue)
            {
                CreateSingletonFromObject(eventComponentData);
            }

            return inputDeps;
        }

        private void CreateSingletonFromObject(object obj)
        {
            typeof(EventComponentSystem).GetMethod("CreateSingleton", BindingFlags.Instance | BindingFlags.NonPublic)?
                .MakeGenericMethod(obj.GetType()).Invoke(this, new object[0]);
        }

        // ReSharper disable once UnusedMember.Local
        private void CreateSingleton<T>(T obj)
            where T : struct, IComponentData
        {
            commandBuffer.Buffer.CreateSingleton(obj);
        }
    }
}