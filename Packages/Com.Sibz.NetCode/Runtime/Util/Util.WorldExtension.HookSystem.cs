using Sibz.NetCode;
using Unity.Entities;

namespace Sibz
{
    public static partial class Util
    {
        public static NetCodeHookSystem GetHookSystem(this World world) => world.GetExistingSystem<NetCodeHookSystem>();
    }
}