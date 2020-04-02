using Unity.Entities;
using Unity.Jobs;
using Unity.NetCode;
using UnityEngine;

namespace Sibz.NetCode.Server
{
    [ServerSystem]
    public sealed class
        NetworkStateMonitorSystem : NetworkStateMonitorSystemBase<NetworkStatus, NetworkState, UpdateStateJob>
    {
        private EntityQuery connectionCountQuery;
        private EntityQuery connectionInGameCountQuery;


        protected override void OnCreate()
        {
            connectionInGameCountQuery = GetEntityQuery(typeof(NetworkStreamInGame));

            connectionCountQuery = GetEntityQuery(typeof(NetworkStreamConnection));
            base.OnCreate();
        }

        protected override UpdateStateJob CreateJob() =>
            new UpdateStateJob
            {
                Listening = NetworkStreamReceiveSystem.Driver.Listening,
                ConnectionCount = connectionCountQuery.CalculateEntityCount(),
                InGameCount = connectionInGameCountQuery.CalculateEntityCount()
            };
    }
}