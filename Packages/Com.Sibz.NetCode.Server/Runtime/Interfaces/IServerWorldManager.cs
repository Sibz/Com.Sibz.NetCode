namespace Sibz.NetCode
{
    public interface IServerWorldManager : IWorldManager
    {
        bool Listen(INetworkEndpointSettings settings);
        void DisconnectAllClients();
        void DisconnectClient(int networkConnectionId);
        void Close();
    }
}