using System;
using System.Collections.Generic;
using Sibz.NetCode.Server;
using Unity.Entities;
using Unity.NetCode;
using Unity.Networking.Transport;

[assembly: DisableAutoCreation]

namespace Sibz.NetCode
{
    public class ServerWorld : WorldBase<ServerSimulationSystemGroup, NetworkStatus>
    {
        public Action<Entity> ClientConnected;
        public Action<Entity> ClientDisconnected;
        protected ServerOptions Options { get; }

        public ServerWorld(ServerOptions options = null, List<Type> systems = null)
            : base(options ?? new ServerOptions(), ClientServerBootstrap.CreateServerWorld,
                systems.AppendTypesWithAttribute<ServerSystemAttribute>())
        {
            Options = options ?? new ServerOptions();

            if (Options.ConnectOnSpawn)
            {
                Listen();
            }
        }

        public void Listen()
        {
            NetworkEndPoint endPoint = NetworkEndPoint.Parse(Options.Address, Options.Port, Options.NetworkFamily);

            NetworkStreamReceiveSystem.Listen(endPoint);

            NetworkStatus networkStatus = new NetworkStatus
            {
                NetworkState = NetworkStreamReceiveSystem.Driver.Listening
                    ? NetworkState.Listening
                    : NetworkState.ListenFailed
            };

            World.EntityManager.SetComponentData(NetworkStatusEntity, networkStatus);
        }

        public void Disconnect()
        {
            World.DestroySystem(NetworkStreamReceiveSystem);
            NetworkStreamReceiveSystem = World.CreateSystem<NetworkStreamReceiveSystem>();
                World.GetExistingSystem<NetworkReceiveSystemGroup>()
                .AddSystemToUpdateList(NetworkStreamReceiveSystem);
                World.GetExistingSystem<NetworkReceiveSystemGroup>().SortSystemUpdateList();
        }
    }
}