using System;
using Sibz.EntityEvents;
using Sibz.NetCode.WorldExtensions;
using Unity.Entities;
using Unity.NetCode;
using UnityEngine;

namespace Sibz.NetCode.Client
{
    [ClientSystem]
    public class ClientConnectSystem : ComponentSystem
    {
        private EntityQuery networkStreamQuery;
        private EntityQuery incomingConfirmRequestQuery;

        protected override void OnCreate()
        {
            RequireSingletonForUpdate<Connecting>();
            networkStreamQuery = GetEntityQuery(typeof(NetworkStreamConnection));
            incomingConfirmRequestQuery = GetEntityQuery(new EntityQueryDesc
            {
                All = new[]
                {
                    ComponentType.ReadOnly<ConfirmConnectionRequest>(),
                    ComponentType.ReadOnly<ReceiveRpcCommandRequestComponent>()
                }
            });
        }

        protected override void OnUpdate()
        {
            var connecting = GetSingleton<Connecting>();

            if (connecting.TimeoutTime < Time.ElapsedTime)
            {
                EntityManager.DestroyEntity(GetSingletonEntity<Connecting>());
                World.EnqueueEvent(new ConnectionFailedEvent {Message = "Connection timed out"});
            }

            if (HasSingleton<ConnectionInitiatedEvent>())
            {
                World.GetNetworkStreamReceiveSystem().Connect(connecting.EndPoint);
                connecting.State = NetworkState.ConnectingToServer;
                EntityManager.SetComponentData(GetSingletonEntity<Connecting>(), connecting);
            }

            if (connecting.State == NetworkState.ConnectingToServer)
            {
                connecting.State = NetworkState.GoingInGame;
            }

            /*Entities.ForEach((Entity connectEntity, ref ClientConnect connect) =>
            {
                ProcessInitialState(ref connect);

                ProcessConnectingState(ref connect);

                ProcessGoingInGameState(ref connect, connectEntity, buffer);

                ProcessTimeOut(ref connect, connectEntity, buffer);
            });*/
        }

        /*private void ProcessTimeOut(ref ClientConnect clientConnect, Entity connectEntity, EntityCommandBuffer buffer)
        {
            if (!(clientConnect.TimeoutTime > UnityEngine.Time.time))
            {
                return;
            }

            buffer.AddComponent(buffer.CreateEntity(),
                new ConnectionCompleteEvent
                {
                    Success = false,
                    Message = $"Connection Timeout ({clientConnect.Timeout}). State: {clientConnect.State}"
                });

            PostUpdateCommands.DestroyEntity(connectEntity);
        }

        private void ProcessGoingInGameState(ref ClientConnect clientConnect, Entity connectEntity,
            EntityCommandBuffer buffer)
        {
            if (clientConnect.State != NetworkState.GoingInGame ||
                incomingConfirmRequestQuery.CalculateEntityCount() <= 0)
            {
                return;
            }

            buffer.AddComponent(buffer.CreateEntity(), new ConnectionCompleteEvent { Success = true });
            PostUpdateCommands.DestroyEntity(connectEntity);
            PostUpdateCommands.DestroyEntity(incomingConfirmRequestQuery);
        }

        private void ProcessConnectingState(ref ClientConnect clientConnect)
        {
            if (clientConnect.State != NetworkState.ConnectingToServer ||
                networkStreamQuery.CalculateEntityCount() <= 0)
            {
                return;
            }

            EntityManager.AddComponent<NetworkStreamInGame>(GetSingletonEntity<NetworkStreamConnection>());

            CreateRpcRequestSystem.CreateRpcRequest<GoInGameRequest>(World, default);

            clientConnect.State = NetworkState.GoingInGame;
        }

        private void ProcessInitialState(ref ClientConnect clientConnect)
        {
            if (clientConnect.State != NetworkState.InitialRequest)
            {
                return;
            }

            network.Connect(clientConnect.EndPoint);
/*#if DEBUG
            WorldBase.Debug($"{World.Name}:{clientConnect.EndPoint.Port} Connecting...");
#endif#1#
            clientConnect.State = NetworkState.ConnectingToServer;
        }*/
    }
}