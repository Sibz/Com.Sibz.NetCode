namespace Sibz.NetCode
{
    public interface IClientWorldManager
    {
        void Connect(INetworkEndpointSettings settings, int timeout = 10);
    }
}