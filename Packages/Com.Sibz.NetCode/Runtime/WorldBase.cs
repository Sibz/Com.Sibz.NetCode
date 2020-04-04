using System;
using System.Collections.Generic;
using Unity.Entities;

[assembly: DisableAutoCreation]

namespace Sibz.NetCode
{
    public abstract class WorldBase : IWorldBase
    {
        public World World => WorldManager.World;

        protected readonly List<Type> Systems;

        protected readonly IWorldManager WorldManager;

        protected WorldBase(IWorldManager worldManager)
        {
            if (worldManager is null)
            {
                throw new ArgumentNullException(nameof(worldManager));
            }

            Systems = worldManager.Options.Systems.AppendTypesWithAttribute<ClientAndServerSystemAttribute>();

            WorldManager = worldManager;

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