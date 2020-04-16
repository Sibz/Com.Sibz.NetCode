using Sibz.CommandBufferHelpers;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.NetCode;
using UnityEngine;

namespace Sibz.NetCode.Server
{
    [ServerSystem]
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    public class DisconnectClientSystem : SystemBase
    {
        private EntityQuery disconnectClientQuery;
        private EndInitCommandBuffer endInitCommandBuffer;
        protected override void OnCreate()
        {
            disconnectClientQuery = GetEntityQuery(typeof(DisconnectClient));
            endInitCommandBuffer = new EndInitCommandBuffer(World);
            RequireForUpdate(disconnectClientQuery);
        }

        protected override void OnUpdate()
        {
            var ids = disconnectClientQuery
                .ToComponentDataArrayAsync<DisconnectClient>(Allocator.TempJob, out JobHandle handle);

            var job = new DisconnectClientJob
            {
                CommandBuffer = endInitCommandBuffer.Concurrent
            };
            Dependency = Entities
                .WithNone<NetworkStreamDisconnected, NetworkStreamRequestDisconnect>()
                .WithDeallocateOnJobCompletion(ids)
                .ForEach((Entity entity, int entityInQueryIndex, ref NetworkIdComponent idComponent) =>
            {
                job.Execute(entity, entityInQueryIndex, ref idComponent, ids);
            }).Schedule(handle);

            endInitCommandBuffer.AddJobDependency(Dependency);
        }
    }
}