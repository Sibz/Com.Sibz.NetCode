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

            if (ProcessTimeout(ref connecting))
            {
                return;
            }

            ProcessConnectionInitiatedEvent(ref connecting);

            ProcessConnectionGoInGame(ref connecting);

            ProcessConnectionComplete(ref connecting);

            if (HasSingleton<Connecting>())
            {
                EntityManager.SetComponentData(GetSingletonEntity<Connecting>(), connecting);
            }
        }

        private bool ProcessTimeout(ref Connecting connecting)
        {
            if (connecting.TimeoutTime < Time.ElapsedTime)
            {
                EntityManager.DestroyEntity(GetSingletonEntity<Connecting>());
                World.EnqueueEvent(new ConnectionFailedEvent { Message = "Connection timed out" });
                return true;
            }

            return false;
        }

        private void ProcessConnectionInitiatedEvent(ref Connecting connecting)
        {
            if (!HasSingleton<ConnectionInitiatedEvent>())
            {
                return;
            }

            NetworkStreamSystemProxy.Connect(connecting.EndPoint);
            connecting.State = NetworkState.ConnectingToServer;
        }

        private void ProcessConnectionGoInGame(ref Connecting connecting)
        {
            if (connecting.State != NetworkState.ConnectingToServer || !HasSingleton<NetworkIdComponent>())
            {
                return;
            }

            connecting.State = NetworkState.GoingInGame;
            EntityManager.AddComponent<NetworkStreamInGame>(GetSingletonEntity<NetworkIdComponent>());
            World.CreateRpcRequest<GoInGameRequest>();
        }

        private void ProcessConnectionComplete(ref Connecting connecting)
        {
            if (connecting.State != NetworkState.GoingInGame || incomingConfirmRequestQuery.CalculateEntityCount() != 1)
            {
                return;
            }

            World.EnqueueEvent(new ConnectionCompleteEvent());
            EntityManager.DestroyEntity(GetSingletonEntity<Connecting>());
            EntityManager.DestroyEntity(incomingConfirmRequestQuery);
        }
    }
}