using Unity.Networking.Transport;

namespace Sibz.NetCode
{
    public interface INetworkEndpointSettings
    {
        string Address { get; set; }
        ushort Port { get; set; }
        NetworkFamily NetworkFamily { get; set; }
    }
}