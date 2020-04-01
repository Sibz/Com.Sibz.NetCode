using System.Collections.Generic;
using Sibz.NetCode.Components;
using Unity.Entities;
using Unity.NetCode;
using Unity.Networking.Transport;
using UnityEngine;

namespace Sibz.NetCode
{
    public class ServerWorld : WorldBase<ServerSimulationSystemGroup>
    {
        protected ServerOptions Options { get; }
        protected Entity NetworkStatusEntity;

        protected ServerWorld(ServerOptions options) : base(options, ClientServerBootstrap.CreateServerWorld)
        {
            Options = options;

            NetworkStatusEntity =
                World.EntityManager.CreateEntity(typeof(NetworkStatus));

            if (options.ConnectOnSpawn)
            {
                Listen();
            }
        }

        public void Listen()
        {
            World.GetExistingSystem<NetworkStreamReceiveSystem>()
                .Listen(NetworkEndPoint.Parse(Options.Address, Options.Port, Options.NetworkFamily));
            World.EntityManager.SetComponentData(NetworkStatusEntity, new NetworkStatus { State = NetworkState.ListenRequested });
        }
    }
}