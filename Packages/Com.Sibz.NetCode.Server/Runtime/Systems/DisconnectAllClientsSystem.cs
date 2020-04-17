using Packages.Components;
using Unity.Entities;
using Unity.NetCode;

namespace Sibz.NetCode.Server
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
                All = new[]
                {
                    ComponentType.ReadOnly<NetworkIdComponent>(),
                    ComponentType.ReadOnly<NetworkStreamConnection>()
                },
                None = new[]
                {
                    ComponentType.ReadOnly<NetworkStreamDisconnected>(),
                    ComponentType.ReadOnly<NetworkStreamRequestDisconnect>()
                }
            });

            RequireForUpdate(triggerQuery);
        }

        protected override void OnUpdate()
        {
            EntityManager.DestroyEntity(triggerQuery);
            EntityManager.AddComponent<NetworkStreamRequestDisconnect>(networkQuery);
        }
    }
}