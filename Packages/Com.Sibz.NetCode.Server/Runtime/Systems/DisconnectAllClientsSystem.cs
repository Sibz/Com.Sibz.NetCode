using Sibz.NetCode;
using Unity.Entities;

namespace Packages.Systems
{
    [ServerSystem]
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    public class DisconnectAllClientsSystem : SystemBase
    {
        protected override void OnUpdate()
        {

        }
    }
}