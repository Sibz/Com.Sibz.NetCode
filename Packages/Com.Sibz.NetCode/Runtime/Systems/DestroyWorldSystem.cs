using System;
using System.Threading.Tasks;
using Sibz.EntityEvents;
using Unity.Entities;

namespace Sibz.NetCode
{
    [ClientAndServerSystem]
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    public class DestroyWorldSystem : SystemBase
    {
        public Action OnDestroyed;

        protected override void OnCreate()
        {
            RequireSingletonForUpdate<DestroyWorld>();
        }

        protected override void OnUpdate()
        {
            if (!HasSingleton<DestroyWorldEvent>())
            {
                World.EnqueueEvent<DestroyWorldEvent>();
                return;
            }

            EntityManager.DestroyEntity(GetSingletonEntity<DestroyWorld>());

            foreach (ComponentSystemBase system in World.Systems)
            {
                system.Enabled = false;
            }
        }

        protected override void OnStopRunning()
        {
            World.Dispose();
        }

        protected override void OnDestroy()
        {
            OnDestroyed?.Invoke();
            base.OnDestroy();
        }
    }
}