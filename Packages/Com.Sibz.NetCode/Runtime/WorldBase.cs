using System;
using System.Collections.Generic;
using Sibz.CommandBufferHelpers;
using Sibz.EntityEvents;
using Sibz.WorldSystemHelpers;
using Unity.Entities;

[assembly: DisableAutoCreation]

namespace Sibz.NetCode
{
    public abstract class WorldBase<TDefaultSystemGroup> : IWorldBase
        where TDefaultSystemGroup : ComponentSystemGroup
    {
        public World World { get; }

        protected readonly BeginInitCommandBuffer CommandBuffer;

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

            World.EnqueueEvent<WorldCreated>();
        }

        private void ImportSystems(List<Type> systems)
        {
            systems = systems ?? new List<Type>();

            systems.AddRange(baseIncludeSystems);

            systems.AppendTypesWithAttribute<WorldBaseSystemAttribute>();

            World.ImportSystemsFromList<TDefaultSystemGroup>(systems);
        }

        public void Dispose()
        {
            World.Dispose();
        }
    }
}