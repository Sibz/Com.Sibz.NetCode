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

        public Action<Entity> ClientConnected { get; set; }
        public Action<int> ClientDisconnected { get; set; }
        public Action ListenSuccess { get; set; }
        public Action ListenFailed { get; set; }
        public Action Closed { get; set; }
        protected ServerOptions Options { get; }

        public ServerWorld(ServerOptions options) : base(options, new ServerWorldCreator(options))
        {
            Options = options ?? throw new ArgumentNullException(nameof(options));

            void OnWorldCreated()
            {
                NetCodeHookSystem hookSystem =
                    World.GetHookSystem()
                    ?? throw new InvalidOperationException(HookSystemDoesNotExistError);

                hookSystem.RegisterHook<ListeningEvent>((e) => { ListenSuccess?.Invoke(); });
                hookSystem.RegisterHook<ListenFailedEvent>((e) => { ListenFailed?.Invoke(); });
                hookSystem.RegisterHook<DisconnectingEvent>((e) => { Closed?.Invoke(); });
                hookSystem.RegisterHook<ClientConnectedEvent>((e) => { ClientConnected?.Invoke(((ClientConnectedEvent)e).ConnectionEntity);});
                hookSystem.RegisterHook<ClientDisconnectedEvent>((e) => { ClientDisconnected?.Invoke(((ClientDisconnectedEvent)e).NetworkId);});
            }

            WorldCreated += OnWorldCreated;
            if (WorldCreator.WorldIsCreated)
            {
                OnWorldCreated();
            }
        }

        public void Listen()
        {
            if (!WorldCreator.WorldIsCreated)
            {
                CreateWorld();
            }

            World.CreateSingleton(new Listen
            {
                EndPoint = NetworkEndPoint.Parse(Options.Address, Options.Port, Options.NetworkFamily)
            });
        }

        public void DisconnectAllClients() =>
            throw new NotImplementedException();

        public void DisconnectClient(int networkConnectionId) =>
            throw new NotImplementedException();

        public void Close()
        {
            World.CreateSingleton<Disconnect>();
        }
    }
}