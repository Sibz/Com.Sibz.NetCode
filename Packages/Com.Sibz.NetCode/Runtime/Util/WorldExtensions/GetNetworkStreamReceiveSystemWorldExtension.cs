using Unity.Entities;
using Unity.NetCode;

namespace Sibz.NetCode.WorldExtensions
{
    public static class GetNetworkStreamReceiveSystemWorldExtension
    {
        public static NetworkStreamReceiveSystem GetNetworkStreamReceiveSystem(this World world) =>
            world.GetExistingSystem<NetworkStreamReceiveSystem>();
    }
}