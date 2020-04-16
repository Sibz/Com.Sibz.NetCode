using Unity.Entities;

namespace Sibz.NetCode.Server
{
    [ServerSystem]
    public class DisconnectClientSystem : SystemBase
    {
        private EntityQuery disconnectClientQuery;
        protected override void OnCreate()
        {
            disconnectClientQuery = GetEntityQuery(typeof(DisconnectClient));
            RequireForUpdate(disconnectClientQuery);
        }

        protected override void OnUpdate()
        {

        }
    }
}