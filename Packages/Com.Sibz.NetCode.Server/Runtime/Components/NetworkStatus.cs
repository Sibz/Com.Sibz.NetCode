using System.Diagnostics.Contracts;
using Unity.Entities;

namespace Sibz.NetCode.Server
{
    public struct NetworkStatus : IComponentData
    {
        public NetworkState State;
        public int InGameClientCount;
        public int ConnectionCount;
    }
}