using Sibz.EntityEvents;
using Sibz.NetCode;
using Sibz.NetCode.Client;
using Sibz.NetCode.WorldExtensions;
using Unity.Entities;
using Unity.NetCode;

namespace Packages.Com.Sibz.NetCode.Client.Runtime.Systems
{
    [ClientSystem]
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    [AlwaysUpdateSystem]
    public class ConnectionMonitorSystem : SystemBase
    {
        private EntityQuery networkStream;

        protected override void OnCreate()
        {
            networkStream =
                EntityManager.CreateEntityQuery(typeof(NetworkIdComponent), typeof(NetworkStreamConnection));
            Enabled = false;
            World.GetHookSystem().RegisterHook<ConnectionCompleteEvent>((e)=>Enabled=true);
        }

        protected override void OnUpdate()
        {
            if (networkStream.CalculateEntityCount() > 0)
            {
                return;
            }

            World.EnqueueEvent<DisconnectedEvent>();

            Enabled = false;
        }
    }
}