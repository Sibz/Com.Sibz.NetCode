using Unity.Entities;
using Unity.NetCode;

namespace Packages.Com.Sibz.NetCode.Client.Runtime.Systems
{
    public class ConnectionMonitorSystem : SystemBase
    {
        private EntityQuery networkStream;

        protected override void OnCreate()
        {
            networkStream =
                EntityManager.CreateEntityQuery(typeof(NetworkIdComponent), typeof(NetworkStreamConnection));
            RequireForUpdate(networkStream);
        }

        protected override void OnUpdate()
        {

        }
    }
}