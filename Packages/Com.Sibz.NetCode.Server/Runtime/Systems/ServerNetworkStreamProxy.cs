using Sibz.NetCode.WorldExtensions;
using Unity.Entities;
using Unity.Networking.Transport;

namespace Sibz.NetCode
{
    public class ServerNetworkStreamProxy : IServerNetworkStreamProxy
    {
        private readonly World world;

        public ServerNetworkStreamProxy(World world)
        {
            this.world = world;
        }
        public bool Listen(NetworkEndPoint endPoint)
        {
            return world.GetNetworkStreamReceiveSystem().Listen(endPoint);
        }
    }
}