﻿using Sibz.EntityEvents;
using Unity.Entities;
using Unity.NetCode;
using UnityEngine;

namespace Sibz.NetCode.Client
{
    [ClientSystem]
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    public class ClientConnectSystem : SystemBase
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
            Connecting connecting = GetSingleton<Connecting>();

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

            if (connecting.Timeout < 0)
            {
                Debug.Log(connecting.Timeout);
                EntityManager.DestroyEntity(GetSingletonEntity<Connecting>());
                World.EnqueueEvent(new ConnectionFailedEvent { Message = "Connection timed out" });
                return true;
            }

            connecting.Timeout -= World.Time.DeltaTime;

            return false;
        }

        private void ProcessConnectionInitiatedEvent(ref Connecting connecting)
        {
            if (connecting.State != NetworkState.InitialRequest)
            {
                return;
            }

            if (!HasSingleton<ConnectionInitiatedEvent>())
            {
                World.EnqueueEvent<ConnectionInitiatedEvent>();
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