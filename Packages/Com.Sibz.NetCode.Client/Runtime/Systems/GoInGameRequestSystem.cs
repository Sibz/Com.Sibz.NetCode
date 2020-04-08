using Sibz.NetCode;
using Unity.NetCode;

namespace Packages.Com.Sibz.NetCode.Client.Runtime.Systems
{
    [ClientSystem]
    public class GoInGameRequestSystem : RpcCommandRequestSystem<GoInGameRequest>
    {
    }
}