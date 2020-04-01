using System;
using Unity.Entities;
using Unity.Jobs;
using Unity.NetCode;
using UnityEngine;

namespace Sibz.NetCode
{
    public abstract class NetworkStateMonitorSystemBase<TStatusComponent, TState, TJob> : JobComponentSystem
    where TStatusComponent : struct, INetworkStatus<TState>
    where TState: Enum
    where TJob: struct, INetworkStateChangeJob<TStatusComponent>
    {
        private NetworkStreamReceiveSystem pNetworkStreamReceiveSystem;
        private TState lastKnownState;

        private NetworkStreamReceiveSystem NetworkStreamReceiveSystem =>
            pNetworkStreamReceiveSystem
            ?? (pNetworkStreamReceiveSystem = World.GetExistingSystem<NetworkStreamReceiveSystem>());

        protected override void OnCreate()
        {
            RequireSingletonForUpdate<TStatusComponent>();
        }

        protected abstract TJob CreateJob();

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            TJob updateStateJob = CreateJob();

            inputDeps = Entities.ForEach((ref TStatusComponent status) => { updateStateJob.Execute(ref status); })
                .Schedule(inputDeps);

            TState currentState = GetSingleton<TStatusComponent>().State;

            if (lastKnownState == currentState)
            {
                return inputDeps;
            }

            World.EnqueueEvent(new NetworkStateChangeEvent {StatusEntity = GetSingletonEntity<TStatusComponent>()});
            lastKnownState = currentState;

            return inputDeps;
        }
    }
}