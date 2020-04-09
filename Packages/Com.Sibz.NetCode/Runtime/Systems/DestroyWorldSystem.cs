using System;
using System.Threading.Tasks;
using Sibz.EntityEvents;
using Unity.Entities;

namespace Sibz.NetCode
{
    [ClientAndServerSystem]
    public class DestroyWorldSystem : ComponentSystem
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

            // After OnUpdate runs, last system version is updated
            // so need to wait for that otherwise it will throw
            // a null ref error.
            var oldSystemVersion = LastSystemVersion;
            new Task(() =>
            {
                while (oldSystemVersion == LastSystemVersion)
                {
                    Task.Delay(1);
                }

                World.Dispose();
            }).Start();
        }

        protected override void OnDestroy()
        {
            OnDestroyed?.Invoke();
            base.OnDestroy();
        }
    }
}