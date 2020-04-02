using Sibz.EntityEvents;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;

namespace Sibz.NetCode
{
    [BurstCompile]
    public struct UpdateNetworkStateJob<TStatusComponent, TJob> : IJobChunk
        where TStatusComponent : struct, IComponentData
        where TJob : INetworkStateChangeJob<TStatusComponent>
    {
        public TJob Job;
        public ArchetypeChunkComponentType<TStatusComponent> StatusComponentType;
        public uint LastSystemVersion;
        public EnqueueEventJobPart<NetworkStateChangeEvent> EnqueueJobPart;

        public void Execute(ArchetypeChunk chunk, int chunkIndex, int firstEntityIndex)
        {
            NativeArray<TStatusComponent> statuses;
            if (!chunk.DidChange(StatusComponentType, LastSystemVersion)
                || (statuses = chunk.GetNativeArray(StatusComponentType)).Length == 0)
            {
                return;
            }

            TStatusComponent status = statuses[0];
            Job.Execute(ref status);
            statuses[0] = status;

            EnqueueJobPart.Execute();
        }
    }
}