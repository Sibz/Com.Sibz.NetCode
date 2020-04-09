using Sibz.NetCode.Server;
using Unity.Entities;

namespace Sibz.NetCode
{
    public class DisconnectSystem : ComponentSystem
    {
        protected override void OnCreate()
        {
            RequireSingletonForUpdate<Disconnect>();
        }

        protected override void OnUpdate()
        {

        }
    }
}