using Sibz.EntityEvents;
using Sibz.NetCode.Server;
using Unity.Entities;
using Unity.Networking.Transport;
using UnityEngine;

namespace Sibz.NetCode.Server
{
    [ServerSystem]
    public class ListenSystem : ComponentSystem
    {
        public IServerNetworkStreamProxy NetworkStreamProxy { get; set; }
        protected override void OnCreate()
        {
            RequireSingletonForUpdate<Listen>();
            NetworkStreamProxy = new ServerNetworkStreamProxy(World);
        }

        protected override void OnUpdate()
        {
            var ev = GetSingleton<Listen>();
            EntityManager.DestroyEntity(GetSingletonEntity<Listen>());

            if (NetworkStreamProxy.Listen(ev.EndPoint))
            {
                EntityManager.CreateEntity(typeof(Listening));
                World.EnqueueEvent<ListeningEvent>();
            }
            else
            {
                World.EnqueueEvent<ListenFailedEvent>();
            }
        }
    }
}