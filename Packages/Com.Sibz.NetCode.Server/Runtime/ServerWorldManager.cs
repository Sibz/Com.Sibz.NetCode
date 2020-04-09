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
    public class ServerWorldCreator : WorldCreatorBase, IServerWorldCreator
    {
        private const string WorldNotCreatedError =
            "{0}.{1}: Can only listen after world is created.";

        public bool IsListening { get; protected set; }

        protected Action<IEventComponentData> OnListen;
        protected Action<IEventComponentData> OnListenFailed;
        protected Action<IEventComponentData> OnDisconnect;

        public ServerWorldCreator(IWorldCreatorOptions options) :
            base(options)
        {
            OnListen += (x) => IsListening = true;
            void OnStopListening(IEventComponentData data) => IsListening = false;
            OnListenFailed += OnStopListening;
            OnDisconnect += OnStopListening;


        }

        public void Listen(INetworkEndpointSettings settings)
        {
            settings = settings ?? throw new ArgumentNullException(nameof(settings));
            if (!WorldIsCreated)
            {
                throw new InvalidOperationException(string.Format(
                    WorldNotCreatedError,
                    nameof(ServerWorldCreator),
                    nameof(Listen)));
            }

            World.CreateSingleton(new Listen
            {
                EndPoint = NetworkEndPoint.Parse(settings.Address, settings.Port, settings.NetworkFamily)
            });
        }

        protected override World BootStrapCreateWorld(string worldName) =>
            ClientServerBootstrap.CreateServerWorld(World.DefaultGameObjectInjectionWorld, worldName);

        protected override void InjectSystems(List<Type> systems) =>
            World.ImportSystemsFromList<ServerSimulationSystemGroup>(systems);
    }
}