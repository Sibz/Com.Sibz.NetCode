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
    public class ServerWorldManager : WorldManagerBase, IServerWorldManager
    {
        private const string WorldNotCreatedError =
            "{0}.{1}: Can only listen after world is created.";

        private const string NoCallbackProvider =
            "{0}.{1}: Callback provider has not been set";

        public new IServerWorldCallbackProvider CallbackProvider { protected get; set; }
        public bool IsListening { get; protected set; }

        protected Action<IEventComponentData> OnListen;
        protected Action<IEventComponentData> OnListenFailed;
        protected Action<IEventComponentData> OnDisconnect;

        public ServerWorldManager(IWorldManagerOptions options, IServerWorldCallbackProvider callbackProvider = null) :
            base(options)
        {
            OnListen += (x) => IsListening = true;
            void OnStopListening(IEventComponentData data) => IsListening = false;
            OnListenFailed += OnStopListening;
            OnDisconnect += OnStopListening;


            CallbackProvider = callbackProvider ?? (IServerWorldCallbackProvider)base.CallbackProvider;

            if (CallbackProvider is null)
            {
                throw new InvalidOperationException(string.Format(
                    NoCallbackProvider,
                    nameof(ServerWorldManager),
                    nameof(Listen)));
            }

            CallbackProvider.WorldCreated += () =>
            {
                World.GetHookSystem().RegisterHook<ListeningEvent>(OnListen);
                World.GetHookSystem().RegisterHook<ListenFailedEvent>(OnListenFailed);
                World.GetHookSystem().RegisterHook<DisconnectingEvent>(OnDisconnect);
            };

            OnDisconnect += (e) => CallbackProvider.Closed?.Invoke();
            OnListenFailed += (e) => CallbackProvider.ListenFailed?.Invoke();
            OnListen += (e) => CallbackProvider.ListenSuccess?.Invoke();
        }

        public void Listen(INetworkEndpointSettings settings)
        {
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

            /*IsListening = World.GetNetworkStreamReceiveSystem().Listen(
                NetworkEndPoint.Parse(settings.Address, settings.Port, settings.NetworkFamily)
            );
            if (IsListening)
            {
                CallbackProvider?.ListenSuccess?.Invoke();
                World.EnqueueEvent<ListeningEvent>();
                World.EntityManager.CreateEntity(typeof(Listening));
            }
            else
            {
                CallbackProvider?.ListenFailed?.Invoke();
                DestroyWorld();
            }
            return IsListening;*/
        }

        public void DisconnectAllClients() => throw new NotImplementedException();

        public void DisconnectClient(int networkConnectionId) => throw new NotImplementedException();

        public void Close()
        {
            DestroyWorld();
            IsListening = false;
            CallbackProvider?.Closed?.Invoke();
        }

        protected override World BootStrapCreateWorld(string worldName) =>
            ClientServerBootstrap.CreateServerWorld(World.DefaultGameObjectInjectionWorld, worldName);

        protected override void InjectSystems(List<Type> systems) =>
            World.ImportSystemsFromList<ServerSimulationSystemGroup>(systems);
    }
}