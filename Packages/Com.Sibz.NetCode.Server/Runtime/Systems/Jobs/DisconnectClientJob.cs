using Unity.Collections;
using Unity.Entities;
using Unity.NetCode;

namespace Sibz.NetCode.Server
{
    public struct DisconnectClientJob
    {
        public EntityCommandBuffer.Concurrent CommandBuffer;

        public void Execute(Entity entity, int entityInQueryIndex, ref NetworkIdComponent idComponent, NativeArray<DisconnectClient> networkIds)
        {
            for (int i = 0; i < networkIds.Length; i++)
            {
                if (networkIds[i].NetworkConnectionId != idComponent.Value)
                {
                    continue;
                }
                CommandBuffer.AddComponent<NetworkStreamRequestDisconnect>(entityInQueryIndex, entity);
                break;
            }
        }
    }
}