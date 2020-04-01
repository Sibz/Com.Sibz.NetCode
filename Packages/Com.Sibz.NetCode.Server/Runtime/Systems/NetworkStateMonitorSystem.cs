using Unity.Entities;
using Unity.Jobs;
using Unity.NetCode;

namespace Sibz.NetCode.Server
{
    [ServerSystem]
    public sealed class NetworkStateMonitorSystem : JobComponentSystem
    {
        private EntityQuery connectionCountQuery;
        private EntityQuery connectionInGameCountQuery;
        private NetworkStreamReceiveSystem pNetworkStreamReceiveSystem;
        private NetworkState lastKnownState;

        private NetworkStreamReceiveSystem NetworkStreamReceiveSystem =>
            pNetworkStreamReceiveSystem
            ?? (pNetworkStreamReceiveSystem = World.GetExistingSystem<NetworkStreamReceiveSystem>());

        protected override void OnCreate()
        {
            RequireSingletonForUpdate<NetworkStatus>();

            connectionInGameCountQuery = GetEntityQuery(typeof(NetworkStreamInGame));

            connectionCountQuery = GetEntityQuery(typeof(NetworkStreamConnection));
        }

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            UpdateStateJob updateStateJob = new UpdateStateJob
            {
                Listening = NetworkStreamReceiveSystem.Driver.Listening,
                ConnectionCount = connectionCountQuery.CalculateEntityCount(),
                InGameCount = connectionInGameCountQuery.CalculateEntityCount()
            };

            inputDeps = Entities.ForEach((ref NetworkStatus status) => { updateStateJob.Execute(ref status); })
                .Schedule(inputDeps);

            return inputDeps;
        }
    }
}