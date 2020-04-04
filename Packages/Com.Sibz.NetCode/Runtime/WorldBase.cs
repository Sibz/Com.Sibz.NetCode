using System;
using System.Collections.Generic;
using Sibz.EntityEvents;
using Unity.Entities;

[assembly: DisableAutoCreation]

namespace Sibz.NetCode
{
    public abstract class WorldBase : IWorldBase, IWorldCallbackProvider
    {
        public World World => WorldManager.World;

        protected readonly List<Type> Systems;

        protected readonly IWorldManager WorldManager;

        public Action WorldCreated { get; set; }
        public Action WorldDestroyed { get; set; }
        public Action PreWorldDestroy { get; set; }

        protected WorldBase(IWorldManager worldManager)
        {
            if (worldManager is null)
            {
                throw new ArgumentNullException(nameof(worldManager));
            }

            Systems = worldManager.Options.Systems.AppendTypesWithAttribute<ClientAndServerSystemAttribute>();

            WorldManager = worldManager;

            worldManager.CallbackProvider = this;

            WorldCreated += () => World.EnqueueEvent(new WorldCreatedEvent());

            if (worldManager.Options.CreateWorldOnInstantiate)
            {
                CreateWorld();
            }
        }

        protected void CreateWorld() => WorldManager.CreateWorld(Systems);


        protected void DestroyWorld() => WorldManager.DestroyWorld();


        public void Dispose() => WorldManager.DestroyWorld();
    }
}