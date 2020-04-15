using Unity.Entities;

namespace Sibz.NetCode.WorldExtensions
{
    public static class GetHookSystemWorldExtension
    {
        public static NetCodeHookSystem GetHookSystem(this World world)
        {
            return world.GetExistingSystem<NetCodeHookSystem>();
        }
    }
}