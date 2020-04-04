using System;
using System.Collections.Generic;
using Sibz.EntityEvents;
using Sibz.NetCode.Server;
using Sibz.WorldSystemHelpers;
using Unity.Entities;
using Unity.NetCode;
using Unity.Networking.Transport;

[assembly: DisableAutoCreation]

namespace Sibz.NetCode
{
    public class ServerWorld : WorldBase
    {
        public ServerWorld(IWorldManager worldManager) : base(worldManager)
        {
        }
    }
}