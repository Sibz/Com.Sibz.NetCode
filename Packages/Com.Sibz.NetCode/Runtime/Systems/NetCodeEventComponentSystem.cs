using Sibz.EntityEvents;
using Unity.Entities;

namespace Sibz.NetCode
{
    [ClientAndServerSystem]
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    public class NetCodeEventComponentSystem : EventComponentSystem
    {
    }
}