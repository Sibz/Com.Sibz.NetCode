using Unity.Entities;

namespace Sibz.NetCode
{
    public interface INetworkStateChangeJob<TStatusComponent>
        where TStatusComponent : struct, IComponentData
    {
        void Execute(ref TStatusComponent statusComponent);
    }
}