using Sibz.NetCode;
using Unity.Entities;

namespace Sibz.NetCode.WorldExtensions
{
    public static class GetHookSystemWorldExtension
    {
        public static NetCodeHookSystem GetHookSystem(this World world) => world.GetExistingSystem<NetCodeHookSystem>();
    }
}