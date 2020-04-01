using Unity.Entities;

namespace Sibz.NetCode.Server
{
    public struct NetworkStatus : IComponentData
    {
        public NetworkState State;
    }
}