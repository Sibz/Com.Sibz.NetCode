using Sibz.EntityEvents;
using Sibz.NetCode.Server;
using Unity.Entities;
using Unity.NetCode;

namespace Sibz.NetCode
{
    public struct GoInGameJob
    {
        public EntityCommandBuffer.Concurrent CommandBuffer;
        public EnqueueEventJobPart<ClientConnectedEvent> EnqueueClientConnectedEventJobPart;

        public void Execute(int index, Entity requestEntity, ref ReceiveRpcCommandRequestComponent reqSrc)
        {
            CommandBuffer.AddComponent<NetworkStreamInGame>(index, reqSrc.SourceConnection);
            CommandBuffer.DestroyEntity(index, requestEntity);
            EnqueueClientConnectedEventJobPart.EventData.ConnectionEntity = reqSrc.SourceConnection;
            EnqueueClientConnectedEventJobPart.Execute();
            CommandBuffer.CreateRpcRequest(index, new ConfirmConnectionRequest(), reqSrc.SourceConnection);
        }
    }
}