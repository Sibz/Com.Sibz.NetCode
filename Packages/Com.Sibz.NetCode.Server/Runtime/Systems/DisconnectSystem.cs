using System.Threading.Tasks;
using Sibz.EntityEvents;
using Sibz.NetCode.Server;
using Unity.Entities;

namespace Sibz.NetCode
{
    [ServerSystem]
    public class DisconnectSystem : ComponentSystem
    {
        protected override void OnCreate()
        {
            RequireSingletonForUpdate<Disconnect>();
        }

        protected override void OnUpdate()
        {
            if (!HasSingleton<DisconnectingEvent>())
            {
                World.EnqueueEvent<DisconnectingEvent>();
                return;
            }

            EntityManager.DestroyEntity(GetSingletonEntity<Disconnect>());

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
    }
}