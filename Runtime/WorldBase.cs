using System;
using System.Collections.Generic;
using Sibz.CommandBufferHelpers;
using Sibz.WorldSystemHelpers;
using Unity.Entities;

[assembly: DisableAutoCreation]

namespace Sibz.NetCode
{
    public abstract class WorldBase<TDefaultSystemGroup> : IDisposable
        where TDefaultSystemGroup : ComponentSystemGroup
    {
        public readonly World World;

        protected readonly BeginInitCommandBuffer CommandBuffer;

        protected WorldBase(IWorldOptionsBase options, Func<World, string, World> creationMethod,
            List<Type> systems = null)
        {
            if (creationMethod is null || options is null)
            {
                throw new ArgumentNullException(creationMethod is null ? nameof(creationMethod) : nameof(options));
            }

            World = creationMethod.Invoke(World.DefaultGameObjectInjectionWorld, options.WorldName);

            World.ImportSystemsFromList<TDefaultSystemGroup>(systems.AppendTypesWithAttribute<WorldBaseSystemAttribute>());

            CommandBuffer = new BeginInitCommandBuffer(World);

            options.SharedDataPrefabs.Instantiate();

            World.EnqueueEvent<WorldCreated>();
        }

        public void Dispose()
        {
            World.Dispose();
        }
    }
}