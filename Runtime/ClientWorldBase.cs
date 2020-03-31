using System;
using System.Collections.Generic;
using Sibz.WorldSystemHelpers;
using Unity.NetCode;

namespace Sibz.NetCode
{
    public abstract class ClientWorldBase : WorldBase<ClientSimulationSystemGroup>
    {
        protected ClientWorldBase(IWorldOptionsBase options) : base(options, ClientServerBootstrap.CreateClientWorld)
        {

        }
    }
}