using Unity.Entities;

[assembly: DisableAutoCreation]

namespace Sibz.NetCode
{
    public class ServerWorld : WorldBase
    {
        public ServerWorld(IWorldManager worldManager) : base(worldManager)
        {
        }
    }
}