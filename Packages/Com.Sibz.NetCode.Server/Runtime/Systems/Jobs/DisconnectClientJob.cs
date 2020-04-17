using Unity.Collections;
using Unity.Entities;
using Unity.NetCode;

namespace Sibz.NetCode.Server
{
    public struct DisconnectClientJob
    {
        public EntityCommandBuffer.Concurrent CommandBuffer;

        public void Execute(Entity entity, int entityInQueryIndex, ref NetworkIdComponent idComponent,
            ref NativeArray<DisconnectClient> networkIds)
        {
            for (int i = 0; i < networkIds.Length; i++)
            {
                DisconnectClient dc = networkIds[i];
                if (dc.NetworkConnectionId != idComponent.Value)
                {
                    continue;
                }

                CommandBuffer.AddComponent<NetworkStreamRequestDisconnect>(entityInQueryIndex, entity);
                dc.NetworkConnectionId = -1;
                networkIds[i] = dc;
                break;
            }
        }
    }
}