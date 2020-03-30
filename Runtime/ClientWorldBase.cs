using System;
using System.Collections.Generic;
using Sibz.WorldSystemHelpers;

namespace Sibz.NetCode
{
    public abstract class ClientWorldBase : WorldBase
    {
        protected ClientWorldBase(IWorldOptionsBase options) : base(options, true)
        {

        }

        protected override List<Type> SystemAttributeTypes
        {
            get
            {
                var l = base.SystemAttributeTypes;
                l.Add(typeof(ClientConnectSystem));
                return l;
            }
        }
    }
}