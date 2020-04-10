namespace Sibz.NetCode.Server
{
    public interface IServerWorldCreator : IWorldCreator
    {
        void Listen(INetworkEndpointSettings settings);
    }
}