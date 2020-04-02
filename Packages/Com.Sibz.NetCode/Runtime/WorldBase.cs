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
    public abstract class WorldBase<TDefaultSystemGroup, TStatusComponent> : IWorldBase
        where TDefaultSystemGroup : ComponentSystemGroup
    {
        public World World { get; }

        public Action<TStatusComponent> NetworksStateChange;

        protected readonly BeginInitCommandBuffer CommandBuffer;
        protected Entity NetworkStatusEntity;
        protected NetworkStreamReceiveSystem NetworkStreamReceiveSystem;
        protected NetCodeHookSystem HookSystem;

        private readonly IReadOnlyList<Type> baseIncludeSystems = new List<Type>
        {
            typeof(EventComponentSystem)
        };

        protected WorldBase(IWorldOptionsBase options, Func<World, string, World> creationMethod,
            List<Type> systems = null)
        {
            if (creationMethod is null || options is null)
            {
                throw new ArgumentNullException(creationMethod is null ? nameof(creationMethod) : nameof(options));
            }

            World = creationMethod.Invoke(World.DefaultGameObjectInjectionWorld, options.WorldName);

            ImportSystems(systems);

            CommandBuffer = new BeginInitCommandBuffer(World);

            options.SharedDataPrefabs.Instantiate();

            NetworkStatusEntity =
                World.EntityManager.CreateEntity(typeof(TStatusComponent));

            NetworkStreamReceiveSystem = World.GetExistingSystem<NetworkStreamReceiveSystem>();

            HookSystem = World.GetExistingSystem<NetCodeHookSystem>();

            HookSystem.RegisterHook<NetworkStateChangeEvent>(OnNetworkStateChange);

            World.EnqueueEvent<WorldCreated>();
        }

        private void ImportSystems(List<Type> systems)
        {
            systems = systems ?? new List<Type>();

            systems.AddRange(baseIncludeSystems);

            systems.AppendTypesWithAttribute<ClientAndServerSystemAttribute>();

            World.ImportSystemsFromList<TDefaultSystemGroup>(systems);
        }

        public void Dispose()
        {
            World.Dispose();
        }

        internal void OnNetworkStateChange(IEventComponentData status)
        {
            NetworksStateChange?.Invoke((TStatusComponent)status);
        }
    }
}