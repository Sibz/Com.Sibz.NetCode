using System;
using System.Collections.Generic;
using Sibz.EntityEvents;
using Sibz.NetCode.WorldExtensions;
using Sibz.WorldSystemHelpers;
using Unity.Entities;
using Unity.NetCode;
using Unity.Networking.Transport;

namespace Sibz.NetCode.Server
{
    public class ServerWorldManager : WorldManagerBase, IServerWorldManager, IServerWorldCallbackProvider
    {
        private const string WorldNotCreatedError =
            "{0}.{1}: Can only listen after world is created.";

        private const string HookSystemDoesNotExistError = "A HookSystem must exist";

        public bool IsListening { get; protected set; }

        protected Action<IEventComponentData> OnListen;
        protected Action<IEventComponentData> OnListenFailed;
        protected Action<IEventComponentData> OnDisconnect;

        public ServerWorldManager(IWorldManagerOptions options) :
            base(options)
        {
            OnListen += (x) => IsListening = true;
            void OnStopListening(IEventComponentData data) => IsListening = false;
            OnListenFailed += OnStopListening;
            OnDisconnect += OnStopListening;

            WorldCreated += () =>
            {
                NetCodeHookSystem hookSystem =
                    World.GetHookSystem()
                    ?? throw new InvalidOperationException(HookSystemDoesNotExistError);

                hookSystem.RegisterHook<ListeningEvent>(OnListen);
                hookSystem.RegisterHook<ListenFailedEvent>(OnListenFailed);
                hookSystem.RegisterHook<DisconnectingEvent>(OnDisconnect);
            };
        }

        public void Listen(INetworkEndpointSettings settings)
        {
            settings = settings ?? throw new ArgumentNullException(nameof(settings));
            if (!WorldIsCreated)
            {
                throw new InvalidOperationException(string.Format(
                    WorldNotCreatedError,
                    nameof(ServerWorldManager),
                    nameof(Listen)));
            }

            World.CreateSingleton(new Listen
            {
                EndPoint = NetworkEndPoint.Parse(settings.Address, settings.Port, settings.NetworkFamily)
            });
        }

        public void DisconnectAllClients() => throw new NotImplementedException();

        public void DisconnectClient(int networkConnectionId) => throw new NotImplementedException();

        protected override World BootStrapCreateWorld(string worldName) =>
            ClientServerBootstrap.CreateServerWorld(World.DefaultGameObjectInjectionWorld, worldName);

        protected override void InjectSystems(List<Type> systems) =>
            World.ImportSystemsFromList<ServerSimulationSystemGroup>(systems);

        public Action<NetworkConnection> ClientConnected { get; set; }
        public Action<NetworkConnection> ClientDisconnected { get; set; }
        public Action ListenSuccess { get; set; }
        public Action ListenFailed { get; set; }
        public Action Closed { get; set; }
    }
}