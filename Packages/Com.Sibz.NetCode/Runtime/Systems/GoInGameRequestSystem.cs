using Unity.NetCode;

namespace Sibz.NetCode
{
    [ClientAndServerSystem]
    public class GoInGameRequestSystem : RpcCommandRequestSystem<GoInGameRequest>
    {
    }
}