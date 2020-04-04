using System;
using System.Collections.Generic;
using Sibz.EntityEvents;
using Sibz.WorldSystemHelpers;
using Unity.Entities;

namespace Sibz.NetCode
{
    public abstract class WorldManagerBase<TDefaultSystemGroup> : IWorldManager
        where TDefaultSystemGroup : ComponentSystemGroup
    {
        public World World { get; protected set; }
        public List<Type> GetSystemsList() => new List<Type>();
        public bool CreateWorldOnInstantiate { get; }
        public bool WorldIsCreated => World?.IsCreated ?? false;

        public Action WorldCreated;
        public Action WorldDestroyed;
        public Action PreDestroyWorld;

        public WorldManagerBase(bool createWorld = false)
        {
            CreateWorldOnInstantiate = createWorld;
        }

        /// <summary>
        /// Define call to ClientServerBootstrap.CreateServerWorld or CreateClientWorld in here.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        protected abstract World BootStrapCreateWorld();

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

            World = BootStrapCreateWorld();

            World.ImportSystemsFromList<TDefaultSystemGroup>(systems);

            // TODO Import prefabs into world

            World.EnqueueEvent<WorldCreatedEvent>();

            WorldCreated?.Invoke();
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

        public void Dispose()
        {
            DestroyWorld();
        }
    }
}