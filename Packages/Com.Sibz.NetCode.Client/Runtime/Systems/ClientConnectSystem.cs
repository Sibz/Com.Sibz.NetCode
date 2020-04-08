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
        private EntityQuery incomingConfirmRequestQuery;
        public IClientNetworkStreamSystemProxy NetworkStreamSystemProxy { get; set; }

        protected override void OnCreate()
        {
            RequireSingletonForUpdate<Connecting>();
            incomingConfirmRequestQuery =
                GetEntityQuery(
                    typeof(ConfirmConnectionRequest),
                    typeof(ReceiveRpcCommandRequestComponent));
            NetworkStreamSystemProxy = new ClientNetworkStreamSystemProxy(World);
        }

        protected override void OnUpdate()
        {
            var connecting = GetSingleton<Connecting>();

            if (connecting.TimeoutTime < Time.ElapsedTime)
            {
                EntityManager.DestroyEntity(GetSingletonEntity<Connecting>());
                World.EnqueueEvent(new ConnectionFailedEvent { Message = "Connection timed out" });
            }

            if (HasSingleton<ConnectionInitiatedEvent>())
            {
                NetworkStreamSystemProxy.Connect(connecting.EndPoint);
                connecting.State = NetworkState.ConnectingToServer;
            }
            else if (connecting.State == NetworkState.ConnectingToServer && HasSingleton<NetworkIdComponent>())
            {
                Entity targetConnection = GetSingletonEntity<NetworkIdComponent>();
                connecting.State = NetworkState.GoingInGame;
                EntityManager.AddComponent<NetworkStreamInGame>(targetConnection);

                Entity entity =
                    EntityManager.CreateEntity(typeof(GoInGameRequest), typeof(SendRpcCommandRequestComponent));
                EntityManager.SetComponentData(entity,
                    new SendRpcCommandRequestComponent { TargetConnection = targetConnection });
            } else if (connecting.State == NetworkState.GoingInGame &&
                       incomingConfirmRequestQuery.CalculateEntityCount() == 1)
            {
                World.EnqueueEvent(new ConnectionCompleteEvent());
                EntityManager.DestroyEntity(GetSingletonEntity<Connecting>());
                EntityManager.DestroyEntity(incomingConfirmRequestQuery);
            }

            if (HasSingleton<Connecting>())
            {
                EntityManager.SetComponentData(GetSingletonEntity<Connecting>(), connecting);
            }
        }
    }
}