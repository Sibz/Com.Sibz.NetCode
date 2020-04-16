using Unity.Entities;

namespace Sibz.NetCode.Server
{
    public struct DisconnectClient : IComponentData
    {
        public int NetworkConnectionId;
    }
}