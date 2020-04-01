using Unity.Entities;
using Unity.Jobs;
using Unity.NetCode;

namespace Sibz.NetCode.Server
{
    public sealed class NetworkStateMonitorSystem : JobComponentSystem
    {
        private EntityQuery connectionCountQuery;
        private EntityQuery connectionInGameCountQuery;
        private NetworkStreamReceiveSystem networkStreamReceiveSystem;
        private float timeoutTime = -1;

        protected override void OnCreate()
        {
            RequireSingletonForUpdate<NetworkStatus>();

            networkStreamReceiveSystem = World.GetExistingSystem<NetworkStreamReceiveSystem>();

            connectionInGameCountQuery = GetEntityQuery(typeof(NetworkStreamInGame));

            connectionCountQuery = GetEntityQuery(typeof(NetworkStreamConnection));
        }

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            UpdateStateJob updateStateJob = new UpdateStateJob
            {
                Listening = networkStreamReceiveSystem.Driver.Listening,
                ConnectionCount = connectionCountQuery.CalculateEntityCount(),
                InGameCount = connectionInGameCountQuery.CalculateEntityCount()
            };

            inputDeps = Entities.ForEach((ref NetworkStatus status) =>
            {
                updateStateJob.Execute(ref status);
            }).Schedule(inputDeps);

            return inputDeps;
        }

        public struct UpdateStateJob
        {
            public bool Listening;
            public int ConnectionCount;
            public int InGameCount;

            public void Execute(ref NetworkStatus status)
            {
                // ReSharper disable once ConvertIfStatementToSwitchStatement
                if (status.State == NetworkState.Uninitialised)
                {
                    return;
                }

                if (status.State == NetworkState.Listening && !Listening)
                {
                    status.State = NetworkState.Disconnected;
                }

                if (status.State != NetworkState.Listening)
                {
                    status.ConnectionCount = 0;
                    status.InGameClientCount = 0;
                    return;
                }

                status.ConnectionCount = ConnectionCount;
                status.InGameClientCount = InGameCount;
            }
        }
    }
}