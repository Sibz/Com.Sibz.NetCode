using Unity.Entities;

namespace Sibz.NetCode
{
    public interface INetworkStatus<T> : IComponentData
    {
        T State { get; }
    }
}