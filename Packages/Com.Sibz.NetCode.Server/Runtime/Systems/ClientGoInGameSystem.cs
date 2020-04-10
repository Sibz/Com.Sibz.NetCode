using Sibz.CommandBufferHelpers;
using Sibz.EntityEvents;
using Unity.Entities;
using Unity.Jobs;
using Unity.NetCode;

namespace Sibz.NetCode.Server
{
    [ServerSystem]
    public class ClientGoInGameSystem : JobComponentSystem
    {
        private EndSimCommandBuffer commandBuffer;

        protected override void OnCreate()
        {
            commandBuffer = new EndSimCommandBuffer(World);
        }

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            var job = new GoInGameJob
            {
                CommandBuffer = commandBuffer.Concurrent,
                EnqueueClientConnectedEventJobPart = World.GetEnqueueEventJobPart<ClientConnectedEvent>()
            };
            inputDeps = Entities
                .WithNone<SendRpcCommandRequestComponent>()
                .WithoutBurst()
                .WithAll<GoInGameRequest>()
                .ForEach(
                    (Entity reqEnt, int entityInQueryIndex,
                        ref ReceiveRpcCommandRequestComponent reqSrc) =>
                    {
                        job.Execute(entityInQueryIndex, reqEnt, ref reqSrc);
                    }).Schedule(inputDeps);

            commandBuffer.AddJobDependency(inputDeps);
            World.EventSystemAddJobDependency(inputDeps);

            return inputDeps;
        }
    }
}