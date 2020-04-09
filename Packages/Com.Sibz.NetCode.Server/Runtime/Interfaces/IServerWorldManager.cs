namespace Sibz.NetCode
{
    public interface IServerWorldManager : IWorldManager
    {
        void Listen(INetworkEndpointSettings settings);
        void DisconnectAllClients();
        void DisconnectClient(int networkConnectionId);
        void Close();
    }
}