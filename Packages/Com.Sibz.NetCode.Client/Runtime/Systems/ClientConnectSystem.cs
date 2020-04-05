using System;
using Unity.Entities;
using Unity.NetCode;

namespace Sibz.NetCode.Client
{
    /*[ClientSystem]
    public class ClientConnectSystem : ComponentSystem
    {
        private NetworkStreamReceiveSystem network;
        private EntityQuery connectQuery;
        private EntityQuery networkStreamQuery;
        private EntityQuery incomingConfirmRequestQuery;
        private BeginInitializationEntityCommandBufferSystem bufferSystem;

        protected override void OnCreate()
        {
            connectQuery = GetEntityQuery(new EntityQueryDesc
            {
                All = new[]
                {
                    ComponentType.ReadOnly<ClientConnect>()
                }
            });
            RequireForUpdate(connectQuery);
            network = World.GetExistingSystem<NetworkStreamReceiveSystem>();
            networkStreamQuery = GetEntityQuery(typeof(NetworkStreamConnection));
            incomingConfirmRequestQuery = GetEntityQuery(new EntityQueryDesc
            {
                All = new[]
                {
                    ComponentType.ReadOnly<ConfirmConnectionRequest>(),
                    ComponentType.ReadOnly<ReceiveRpcCommandRequestComponent>()
                }
            });

            bufferSystem = World.GetExistingSystem<BeginInitializationEntityCommandBufferSystem>();
        }

        protected override void OnUpdate()
        {
            if (connectQuery.CalculateEntityCount() > 1)
            {
                throw new SystemException("Expected only one entity with connect component!");
            }

            EntityCommandBuffer buffer = bufferSystem.CreateCommandBuffer();

            Entities.ForEach((Entity connectEntity, ref ClientConnect connect) =>
            {
                ProcessInitialState(ref connect);

                ProcessConnectingState(ref connect);

                ProcessGoingInGameState(ref connect, connectEntity, buffer);

                ProcessTimeOut(ref connect, connectEntity, buffer);
            });
        }

        private void ProcessTimeOut(ref ClientConnect clientConnect, Entity connectEntity, EntityCommandBuffer buffer)
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
        }
    }*/
}