using Sibz.NetCode.Client;
using Unity.Entities;
using Unity.Networking.Transport;

namespace Sibz.NetCode.Tests.Client
{
    public class MyClientNetworkStreamSystemProxy : IClientNetworkStreamSystemProxy
    {
      public Entity Connect(NetworkEndPoint endPoint) => Entity.Null;
    }
}