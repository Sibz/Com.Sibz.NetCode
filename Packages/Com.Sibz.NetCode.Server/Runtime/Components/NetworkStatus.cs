using Unity.Entities;

namespace Sibz.NetCode.Components
{
    public struct NetworkStatus : IComponentData
    {
        public NetworkState State;
    }
}