using System;
using System.Collections.Generic;
using Packages.Components;
using Sibz.NetCode.Server;
using Sibz.NetCode.WorldExtensions;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.NetCode;
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

                hookSystem.RegisterHook<ListeningEvent>(e => { ListenSuccess?.Invoke(); });
                hookSystem.RegisterHook<ListenFailedEvent>(e => { ListenFailed?.Invoke(); });
                hookSystem.RegisterHook<DisconnectingEvent>(e => { Closed?.Invoke(); });
                hookSystem.RegisterHook<ClientConnectedEvent>(e =>
                {
                    ClientConnected?.Invoke(((ClientConnectedEvent) e).ConnectionEntity);
                });
                hookSystem.RegisterHook<ClientDisconnectedEvent>(e =>
                {
                    ClientDisconnected?.Invoke(((ClientDisconnectedEvent) e).NetworkId);
                });
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

        public void DisconnectAllClients()
        {
            World.CreateSingleton<DisconnectAllClients>();
        }

        public void DisconnectClient(int networkConnectionId)
        {
            World.CreateSingleton(new DisconnectClient { NetworkConnectionId = networkConnectionId });
        }

        public void Close()
        {
            World.CreateSingleton<Disconnect>();
        }

        public Entity GetNetworkConnectionEntityById(int id)
        {
            EntityQuery eq = World.EntityManager.CreateEntityQuery(typeof(NetworkIdComponent));
            using (NativeArray<NetworkIdComponent> comps = eq.ToComponentDataArrayAsync<NetworkIdComponent>(Allocator.TempJob, out JobHandle jh1))
            using (NativeArray<Entity> entities = eq.ToEntityArrayAsync(Allocator.TempJob, out JobHandle jh2)) {
                jh1.Complete();
                jh2.Complete();
                for (int i = 0; i < comps.Length; i++)
                {

                        if (comps[i].Value == id)
                        {
                            return entities[i];
                        }
                }
            }
            throw new KeyNotFoundException($"Unable to locate network entity with id {id}");
        }
    }
}