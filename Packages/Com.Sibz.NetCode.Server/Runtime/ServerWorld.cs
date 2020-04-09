using System;
using Sibz.NetCode.Server;
using Sibz.NetCode.WorldExtensions;
using Unity.Entities;
using Unity.Networking.Transport;

[assembly: DisableAutoCreation]

namespace Sibz.NetCode
{
    public class ServerWorld : WorldBase, IServerWorldCallbackProvider
    {
        private const string HookSystemDoesNotExistError = "A HookSystem must exist";

        public Action<NetworkConnection> ClientConnected { get; set; }
        public Action<NetworkConnection> ClientDisconnected { get; set; }
        public Action ListenSuccess { get; set; }
        public Action ListenFailed { get; set; }
        public Action Closed { get; set; }
        protected ServerOptions Options { get; }

        protected IServerWorldCreator ServerWorldCreator => (IServerWorldCreator)WorldCreator;

        public ServerWorld(ServerOptions options) : base(options, new ServerWorldCreator(options))
        {
            Options = options;

            WorldCreated += () =>
            {
                NetCodeHookSystem hookSystem =
                    World.GetHookSystem()
                    ?? throw new InvalidOperationException(HookSystemDoesNotExistError);

                hookSystem.RegisterHook<ListeningEvent>((e) => { ListenSuccess?.Invoke(); });
                hookSystem.RegisterHook<ListenFailedEvent>((e) => { ListenFailed?.Invoke(); });
                hookSystem.RegisterHook<DisconnectingEvent>((e) => { Closed?.Invoke(); });
            };
        }

        public void Listen()
        {
            if (!WorldCreator.WorldIsCreated)
            {
                CreateWorld();
            }
            ServerWorldCreator.Listen(Options);
        }

        public void DisconnectAllClients() =>
            throw new NotImplementedException();

        public void DisconnectClient(int networkConnectionId) =>
            throw new NotImplementedException();

        public void Close() => DestroyWorld();
    }
}