﻿using System;
using Sibz.EntityEvents;
using Sibz.NetCode.Client;
using Sibz.NetCode.WorldExtensions;
using Unity.Entities;
using Unity.NetCode;
using Unity.Networking.Transport;

[assembly: DisableAutoCreation]

namespace Sibz.NetCode
{
    public class ClientWorld : WorldBase, IClientWorldCallbackProvider
    {
        private const string WorldNotCreatedError =
            "{0}: Unable to connect, world is not created.";

        public Action Connecting { get; set; }
        public Action<int> Connected { get; set; }
        public Action<string> ConnectionFailed { get; set; }
        public Action Disconnected { get; set; }
        public int NetworkId { get; private set; }
        protected new ClientOptions Options { get; }

        public ClientWorld(ClientOptions options) : base(options, new ClientWorldCreator(options))
        {
            Options = options ?? throw new ArgumentNullException(nameof(options));

            Connected += i => NetworkId = i;
            Disconnected += () => NetworkId = 0;

            void OnWorldCreate()
            {
                HookSystem hookSystem = World.GetHookSystem();
                hookSystem.RegisterHook<DisconnectedEvent>(e => Disconnected?.Invoke());
                hookSystem.RegisterHook<ConnectionInitiatedEvent>(e => Connecting?.Invoke());
                hookSystem.RegisterHook<ConnectionCompleteEvent>(e =>
                {
                    Connected?.Invoke(World.EntityManager
                        .GetComponentData<NetworkIdComponent>(
                            World.GetExistingSystem<CreateRpcRequestSystem>().CommandTargetComponentEntity)
                        .Value);
                });
                hookSystem.RegisterHook<ConnectionFailedEvent>(e =>
                    ConnectionFailed?.Invoke(((ConnectionFailedEvent) e).Message.ToString()));
            }

            if (WorldCreator.WorldIsCreated)
            {
                OnWorldCreate();
            }

            WorldCreated += OnWorldCreate;
        }

        public void Connect()
        {
            if (!WorldCreator.WorldIsCreated)
            {
                throw new InvalidOperationException(string.Format(WorldNotCreatedError, nameof(Connect)));
            }

            World.CreateSingleton(new Connecting
            {
                EndPoint = NetworkEndPoint.Parse(Options.Address, Options.Port, Options.NetworkFamily),
                Timeout = Options.TimeOut,
                State = NetworkState.InitialRequest
            });
        }

        public void Disconnect()
        {
            World.CreateSingleton<Disconnect>();
        }
    }
}