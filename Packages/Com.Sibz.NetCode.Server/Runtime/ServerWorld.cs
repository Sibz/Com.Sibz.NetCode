using System;
using Sibz.NetCode.Server;
using Unity.Entities;
using Unity.NetCode;
using Unity.Networking.Transport;

[assembly: DisableAutoCreation]

namespace Sibz.NetCode
{
    public class ServerWorld : WorldBase, IServerWorldCallbackProvider
    {
        public Action<NetworkConnection> ClientConnected { get; set; }
        public Action<NetworkConnection> ClientDisconnected { get; set; }
        public Action ListenSuccess { get; set; }
        public Action ListenFailed { get; set; }
        public Action Closed { get; set; }
        protected ServerOptions Options { get; }

        protected IServerWorldManager ServerWorldManager => (IServerWorldManager)WorldManager;

        public ServerWorld(ServerOptions options) : base(new ServerWorldManager(options))
        {
            Options = options;
        }

        public void Listen()
        {
            if (!WorldManager.WorldIsCreated)
            {
                CreateWorld();
            }
            ServerWorldManager.Listen(Options);
        }

        public void DisconnectAllClients() =>
            ServerWorldManager.DisconnectAllClients();

        public void DisconnectClient(int networkConnectionId) =>
            ServerWorldManager.DisconnectClient(networkConnectionId);

        public void Close() =>
            ServerWorldManager.Close();
    }
}