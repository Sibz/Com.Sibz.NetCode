using System;
using System.Collections.Generic;
using Sibz.EntityEvents;
using Sibz.NetCode.Server;
using Sibz.WorldSystemHelpers;
using Unity.Entities;
using Unity.NetCode;
using Unity.Networking.Transport;

[assembly: DisableAutoCreation]

namespace Sibz.NetCode
{
    public class ServerWorld : WorldBase
    {
        public Action<Entity> ClientConnected;
        public Action<Entity> ClientDisconnected;
        protected ServerOptions Options { get; }

        public void Listen()
        {
            NetworkEndPoint endPoint = NetworkEndPoint.Parse(Options.Address, Options.Port, Options.NetworkFamily);

            World.GetNetworkStreamReceiveSystem().Listen(endPoint);

            /*NetworkStatus networkStatus = new NetworkStatus
            {
                NetworkState = NetworkStreamReceiveSystem.Driver.Listening
                    ? NetworkState.Listening
                    : NetworkState.ListenFailed
            };*/

            /*
            World.EntityManager.SetComponentData(NetworkStatusEntity, networkStatus);

            World.EnqueueEvent(new NetworkStateChangeEvent {StatusEntity = NetworkStatusEntity});*/
        }

        public ServerWorld(IWorldManager worldManager) : base(worldManager)
        {
        }
    }
}