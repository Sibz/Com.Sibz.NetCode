using Sibz.NetCode;
using Unity.Entities;
using Unity.NetCode;

namespace Sibz
{
    public static partial class Util
    {
        public static NetworkStreamReceiveSystem GetNetworkStreamReceiveSystem(this World world) => world.GetExistingSystem<NetworkStreamReceiveSystem>();
    }
}