using System.Threading.Tasks;
using Sibz.EntityEvents;
using Sibz.NetCode.Server;
using Sibz.NetCode.WorldExtensions;
using Unity.Entities;

namespace Sibz.NetCode
{
    [ServerSystem]
    [UpdateBefore(typeof(DestroyWorldSystem))]
    public class DisconnectSystem : ComponentSystem
    {
        protected override void OnCreate()
        {
            RequireSingletonForUpdate<Disconnect>();
        }

        protected override void OnUpdate()
        {
            if (!HasSingleton<DestroyWorld>())
            {
                World.CreateSingleton<DestroyWorld>();
            }

            if (!HasSingleton<DisconnectingEvent>())
            {
                World.EnqueueEvent<DisconnectingEvent>();
                return;
            }

            EntityManager.DestroyEntity(GetSingletonEntity<Disconnect>());
        }
    }
}