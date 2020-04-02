using System;
using Sibz.EntityEvents;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.NetCode;

namespace Sibz.NetCode
{
    public abstract class NetworkStateMonitorSystemBase<TStatusComponent, TState, TJob> : JobComponentSystem
    where TStatusComponent : struct, INetworkStatus<TState>
    where TState: Enum
    where TJob: struct, INetworkStateChangeJob<TStatusComponent>
    {
        private EntityQuery networkStatusQuery;
        private NetworkStreamReceiveSystem pNetworkStreamReceiveSystem;

        protected NetworkStreamReceiveSystem NetworkStreamReceiveSystem =>
            pNetworkStreamReceiveSystem
            ?? (pNetworkStreamReceiveSystem = World.GetExistingSystem<NetworkStreamReceiveSystem>());

        protected override void OnCreate()
        {
            networkStatusQuery = GetEntityQuery(typeof(TStatusComponent));
            RequireForUpdate(networkStatusQuery);
        }

        protected abstract TJob CreateJob();

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            inputDeps = new UpdateNetworkStateJob<TStatusComponent, TJob>
            {
                Job = CreateJob(),
                StatusComponentType = GetArchetypeChunkComponentType<TStatusComponent>(),
                LastSystemVersion = LastSystemVersion,
                EnqueueJobPart = World.GetEnqueueEventJobPart(
                    new NetworkStateChangeEvent
                    {
                        StatusEntity = GetSingletonEntity<TStatusComponent>()
                    })
            }.Schedule(networkStatusQuery, inputDeps);

            World.EventSystemAddJobDependency(inputDeps);

            return inputDeps;
        }
    }
}