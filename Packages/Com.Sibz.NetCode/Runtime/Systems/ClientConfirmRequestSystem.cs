using Sibz.NetCode;
using Unity.NetCode;

namespace Packages.Com.Sibz.NetCode.Server.Runtime.Systems
{
    [ClientAndServerSystem]
    public class ClientConfirmRequestSystem : RpcCommandRequestSystem<ConfirmConnectionRequest>
    {
    }
}