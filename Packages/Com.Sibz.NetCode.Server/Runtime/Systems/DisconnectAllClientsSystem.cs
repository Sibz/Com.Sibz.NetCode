using Packages.Components;
using Sibz.NetCode;
using Unity.Entities;
using Unity.NetCode;

namespace Packages.Systems
{
    [ServerSystem]
    public class DisconnectAllClientsSystem : SystemBase
    {
        private EntityQuery triggerQuery;
        private EntityQuery networkQuery;


        protected override void OnCreate()
        {
            triggerQuery = GetEntityQuery(typeof(DisconnectAllClients));
            networkQuery = GetEntityQuery(new EntityQueryDesc
            {
                All = new []
                {
                    ComponentType.ReadOnly<NetworkIdComponent>(),
                    ComponentType.ReadOnly<NetworkStreamConnection>(),
                },
                None = new []
                {
                    ComponentType.ReadOnly<NetworkStreamDisconnected>(),
                    ComponentType.ReadOnly<NetworkStreamRequestDisconnect>(),
                }
            });

            RequireForUpdate(triggerQuery);
        }

        protected override void OnUpdate()
        {
            EntityManager.DestroyEntity(triggerQuery);
        }

    }
}