using System;
using System.Collections.Generic;
using Sibz.WorldSystemHelpers;
using Unity.Entities;

namespace Sibz.NetCode
{
    public abstract class WorldManagerBase<TDefaultSystemGroup> : IWorldManager
        where TDefaultSystemGroup : ComponentSystemGroup
    {
        public World World { get; protected set; }
        public bool WorldIsCreated => World?.IsCreated ?? false;

        public Action WorldCreated;
        public Action WorldDestroyed;
        public Action PreDestroyWorld;

        public IWorldManagerOptions Options { get; }

        public WorldManagerBase(IWorldManagerOptions options)
        {
            if (options is null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            Options = options;
        }

        /// <summary>
        ///     Define call to ClientServerBootstrap.CreateServerWorld or CreateClientWorld in here.
        /// </summary>
        /// <param name="worldName"></param>
        /// <returns></returns>
        protected abstract World BootStrapCreateWorld(string worldName);

        public void CreateWorld(List<Type> systems)
        {
            if (systems is null)
            {
                throw new ArgumentNullException(nameof(systems));
            }

            if (WorldIsCreated)
            {
                throw new InvalidOperationException("Can not create world. World is all ready created.");
            }

            World = BootStrapCreateWorld(Options.WorldName);

            World.ImportSystemsFromList<TDefaultSystemGroup>(systems);

            ImportPrefabs();

            WorldCreated?.Invoke();
        }

        protected virtual void ImportPrefabs()
        {
            // TODO Import prefabs into world;
        }

        public void DestroyWorld()
        {
            if (!WorldIsCreated)
            {
                return;
            }

            PreDestroyWorld?.Invoke();

            World.Dispose();

            WorldDestroyed?.Invoke();
        }

        public void Dispose() => DestroyWorld();
    }
}