namespace Sibz.NetCode
{
    public interface IServerWorldCreator : IWorldCreator
    {
        void Listen(INetworkEndpointSettings settings);
        void DisconnectAllClients();
        void DisconnectClient(int networkConnectionId);
    }
}