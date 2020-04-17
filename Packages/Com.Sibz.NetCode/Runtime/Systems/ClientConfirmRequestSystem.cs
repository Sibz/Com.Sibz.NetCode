using Unity.NetCode;

namespace Sibz.NetCode
{
    [ClientAndServerSystem]
    public class ClientConfirmRequestSystem : RpcCommandRequestSystem<ConfirmConnectionRequest>
    {
    }
}