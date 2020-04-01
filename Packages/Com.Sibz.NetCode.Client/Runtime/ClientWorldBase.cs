using Unity.NetCode;

namespace Sibz.NetCode.Client
{
    public abstract class ClientWorldBase : WorldBase<ClientSimulationSystemGroup>
    {
        protected ClientWorldBase(IWorldOptionsBase options) : base(options, ClientServerBootstrap.CreateClientWorld)
        {
        }
    }
}