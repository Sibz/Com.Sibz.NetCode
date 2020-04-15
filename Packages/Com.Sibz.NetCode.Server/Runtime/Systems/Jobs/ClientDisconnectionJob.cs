using Sibz.EntityEvents;
using Unity.Entities;
using Unity.NetCode;

namespace Sibz.NetCode.Server
{
    public struct ClientDisconnectedJob
    {
        public EntityCommandBuffer.Concurrent CommandBuffer;
        public EnqueueEventJobPart<ClientDisconnectedEvent> EnqueueEventJobPart;

        public void Execute(int index, Entity linkedEntity, NetworkIdComponent networkId)
        {
            if (linkedEntity != Entity.Null)
            {
                CommandBuffer.DestroyEntity(index, linkedEntity);
            }

            EnqueueEventJobPart.EventData.NetworkId = networkId.Value;
            EnqueueEventJobPart.Execute();
        }
    }
}