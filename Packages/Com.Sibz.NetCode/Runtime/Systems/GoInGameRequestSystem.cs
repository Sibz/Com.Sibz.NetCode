using Sibz.NetCode;
using Unity.NetCode;

namespace Packages.Com.Sibz.NetCode.Client.Runtime.Systems
{
    [ClientAndServerSystem]
    public class GoInGameRequestSystem : RpcCommandRequestSystem<GoInGameRequest>
    {
    }
}