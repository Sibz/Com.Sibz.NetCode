using Sibz.NetCode;
using Unity.Entities;
using Unity.NetCode;

namespace Sibz
{
    public static partial class Util
    {
        public static NetworkStreamReceiveSystem NetworkStreamReceiveSystem(this World world) => world.GetExistingSystem<NetworkStreamReceiveSystem>();
    }
}