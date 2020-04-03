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

        public IWorldManager WorldManager { set => worldManager = value; }

        protected virtual IWorldOptionsBase Options { get; }
        protected BeginInitCommandBuffer CommandBuffer { get; private set; }
        protected NetworkStreamReceiveSystem NetworkStreamReceiveSystem;
        protected NetCodeHookSystem HookSystem;
        protected Func<World, string, World> CreationMethod;
        protected readonly List<Type> Systems;

        private IWorldManager worldManager;

        protected WorldBase(IWorldOptionsBase options, Func<World, string, World> creationMethod,
            List<Type> systems = null)
        {
            if (creationMethod is null || options is null)
            {
                throw new ArgumentNullException(creationMethod is null ? nameof(creationMethod) : nameof(options));
            }

            Systems = systems.AppendTypesWithAttribute<ClientAndServerSystemAttribute>();

            Options = options;

            worldManager = worldManager ?? new WorldManagerClass(this);
        }

        public void Dispose()
        {
            if (World.IsCreated)
            {
                World.Dispose();
            }
        }
    }
}