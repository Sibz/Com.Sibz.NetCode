using Sibz.EntityEvents;
using Unity.Entities;

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
            Listen ev = GetSingleton<Listen>();
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