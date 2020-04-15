using Sibz.EntityEvents;
using Unity.Entities;
using Unity.NetCode;

namespace Sibz.NetCode.Client
{
    public struct DisconnectJob
    {
        public EntityCommandBuffer.Concurrent CommandBuffer;
        public bool EventExists;
        public Entity Disconnect;
        public EnqueueEventJobPart<DisconnectedEvent> EnqueueEventJobPart;

        public void Execute(int index, Entity networkStreamEntity)
        {
            if (!EventExists)
            {
                EnqueueEventJobPart.Execute();
                return;
            }

            CommandBuffer.DestroyEntity(index, Disconnect);
            CommandBuffer.AddComponent<NetworkStreamRequestDisconnect>(index, networkStreamEntity);
        }
    }
}