﻿using Sibz.CommandBufferHelpers;
using Sibz.EntityEvents;
using Sibz.NetCode.Server;
using Unity.Entities;
using Unity.Jobs;
using Unity.NetCode;

namespace Sibz.NetCode
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
            inputDeps = Entities.WithNone<SendRpcCommandRequestComponent>().ForEach(
                (Entity reqEnt, int entityInQueryIndex, ref GoInGameRequest req,
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