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

        protected override void OnCreate()
        {
            triggerQuery = GetEntityQuery(typeof(DisconnectAllClients));

            RequireForUpdate(triggerQuery);
        }

        protected override void OnUpdate()
        {
        }

    }
}