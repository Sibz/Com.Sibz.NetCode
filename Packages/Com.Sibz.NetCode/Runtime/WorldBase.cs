using System;
using System.Collections.Generic;
using Sibz.CommandBufferHelpers;
using Sibz.EntityEvents;
using Sibz.WorldSystemHelpers;
using Unity.Entities;
using Unity.NetCode;

[assembly: DisableAutoCreation]

namespace Sibz.NetCode
{
    public abstract partial class WorldBase<TDefaultSystemGroup> : IWorldBase
        where TDefaultSystemGroup : ComponentSystemGroup
    {
        public World World { get; protected set; }

        protected IWorldOptionsBase Options { get; }
        //protected BeginInitCommandBuffer CommandBuffer { get; private set; }
        protected NetworkStreamReceiveSystem NetworkStreamReceiveSystem;
        protected NetCodeHookSystem HookSystem;
        protected Func<World, string, World> CreationMethod;
        protected readonly List<Type> Systems;

        private IWorldManager worldManager;

        protected WorldBase(IWorldOptionsBase options, Func<World, string, World> creationMethod,
            List<Type> systems = null, IWorldManager worldManager = null)
        {
            if (creationMethod is null || options is null)
            {
                throw new ArgumentNullException(creationMethod is null ? nameof(creationMethod) : nameof(options));
            }

            Systems = systems.AppendTypesWithAttribute<ClientAndServerSystemAttribute>();

            Options = options;

            this.worldManager = worldManager ?? new WorldManagerClass(this);

            if (Options.ConnectOnSpawn)
            {
               worldManager.CreateWorld();
            }
        }

        public void Dispose()
        {
            if (!(World is null) && World.IsCreated)
            {
                World.Dispose();
            }
        }
    }
}