using System;
using System.Collections.Generic;
using Sibz.EntityEvents;
using Sibz.NetCode.WorldExtensions;
using Sibz.WorldSystemHelpers;
using Unity.Entities;
using Unity.NetCode;
using Unity.Networking.Transport;

namespace Sibz.NetCode.Client
{
    public class ClientWorldManager : WorldManagerBase, IClientWorldManager
    {
        private const string WorldNotCreatedError =
            "{0}: Unable to connect, world is not created.";

        public new IClientWorldCallbackProvider CallbackProvider { protected get; set; }

        public ClientWorldManager(IWorldManagerOptions options, IWorldCallbackProvider callbackProvider = null) : base(
            options, callbackProvider)
        {
        }

        public void Connect(INetworkEndpointSettings settings, int timeout = 10)
        {
            if (settings is null)
            {
                throw new ArgumentNullException(nameof(settings));
            }

            if (!WorldIsCreated)
            {
                throw new InvalidOperationException(string.Format(WorldNotCreatedError, nameof(Connect)));
            }

            var endPoint = NetworkEndPoint.Parse(settings.Address, settings.Port, settings.NetworkFamily);

            World.CreateSingleton(new Connecting
            {
                EndPoint = endPoint,
                TimeoutTime = (float)World.Time.ElapsedTime + timeout,
                State = NetworkState.ConnectingToServer
            });

            World.EnqueueEvent(new ConnectionInitiatedEvent());

            CallbackProvider?.Connecting?.Invoke();
        }

        protected override World BootStrapCreateWorld(string worldName) =>
            ClientServerBootstrap.CreateClientWorld(
                World.DefaultGameObjectInjectionWorld,
                worldName);

        protected override void InjectSystems(List<Type> systems) =>
            World.ImportSystemsFromList<ClientSimulationSystemGroup>(systems);
    }
}