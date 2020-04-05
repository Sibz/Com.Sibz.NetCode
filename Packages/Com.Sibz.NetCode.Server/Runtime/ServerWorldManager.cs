using System;
using System.Collections.Generic;
using Sibz.EntityEvents;
using Sibz.NetCode.Server;
using Sibz.NetCode.WorldExtensions;
using Sibz.WorldSystemHelpers;
using Unity.Entities;
using Unity.NetCode;
using Unity.Networking.Transport;

namespace Sibz.NetCode
{
    public class ServerWorldManager : WorldManagerBase, IServerWorldManager
    {
        private const string WorldNotCreatedError =
            "{0}.{1}: Can only listen after world is created.";
        public new IServerWorldCallbackProvider CallbackProvider { protected get; set; }
        public bool IsListening { get; protected set; }

        public ServerWorldManager(IWorldManagerOptions options) : base(options)
        {

        }

        public bool Listen(INetworkEndpointSettings settings)
        {
            if (!WorldIsCreated)
            {
                throw new InvalidOperationException(string.Format(
                    WorldNotCreatedError,
                    nameof(ServerWorldManager),
                    nameof(Listen)));
            }
            IsListening = World.GetNetworkStreamReceiveSystem().Listen(
                NetworkEndPoint.Parse(settings.Address, settings.Port, settings.NetworkFamily)
            );
            if (IsListening)
            {
                CallbackProvider?.ListenSuccess?.Invoke();
                World.EnqueueEvent<ListenStartedEvent>();
                World.EntityManager.CreateEntity(typeof(Listening));
            }
            else
            {
                CallbackProvider?.ListenFailed?.Invoke();
                DestroyWorld();
            }
            return IsListening;
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