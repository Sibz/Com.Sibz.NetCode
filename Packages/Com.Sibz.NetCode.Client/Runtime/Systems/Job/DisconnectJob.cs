using Sibz.EntityEvents;
using Unity.Entities;
using Unity.NetCode;

namespace Sibz.NetCode.Client
{
    public struct DisconnectJob
    {
        public EntityCommandBuffer.Concurrent CommandBuffer;
        public Entity Disconnect;

        public void Execute(int index, Entity networkStreamEntity)
        {
            CommandBuffer.DestroyEntity(index, Disconnect);
            CommandBuffer.AddComponent<NetworkStreamRequestDisconnect>(index, networkStreamEntity);
        }
    }
}