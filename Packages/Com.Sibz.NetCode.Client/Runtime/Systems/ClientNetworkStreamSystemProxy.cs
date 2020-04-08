using Sibz.NetCode.WorldExtensions;
using Unity.Entities;
using Unity.Networking.Transport;

namespace Sibz.NetCode.Client
{
    public class ClientNetworkStreamSystemProxy : IClientNetworkStreamSystemProxy
    {
        private World world;

        public ClientNetworkStreamSystemProxy(World world)
        {
            this.world = world;
        }

        public Entity Connect(NetworkEndPoint endPoint) => world.GetNetworkStreamReceiveSystem().Connect(endPoint);
    }
}