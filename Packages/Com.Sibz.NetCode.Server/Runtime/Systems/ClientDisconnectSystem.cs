using Sibz.CommandBufferHelpers;
using Sibz.EntityEvents;
using Unity.Entities;
using Unity.Jobs;
using Unity.NetCode;

namespace Sibz.NetCode.Server
{
    [ServerSystem]
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    public class ClientDisconnectSystem : JobComponentSystem
    {
        private EndSimCommandBuffer commandBuffer;

        protected override void OnCreate()
        {
            commandBuffer = new EndSimCommandBuffer(World);
        }

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            ClientDisconnectedJob job = new ClientDisconnectedJob
            {
                CommandBuffer = commandBuffer.Concurrent,
                EnqueueEventJobPart = World.GetEnqueueEventJobPart<ClientDisconnectedEvent>()
            };
            inputDeps = Entities.WithAll<NetworkStreamDisconnected>()
                .ForEach((Entity e, int entityInQueryIndex, ref NetworkIdComponent networkId,
                    ref CommandTargetComponent state) =>
                {
                    job.Execute(entityInQueryIndex, state.targetEntity, networkId);
                }).Schedule(inputDeps);
            commandBuffer.AddJobDependency(inputDeps);
            World.EventSystemAddJobDependency(inputDeps);

            return inputDeps;
        }
    }
}