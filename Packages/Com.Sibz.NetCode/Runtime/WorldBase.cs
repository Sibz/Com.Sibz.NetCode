using System;
using Sibz.EntityEvents;
using Sibz.NetCode.WorldExtensions;
using Unity.Entities;

[assembly: DisableAutoCreation]

namespace Sibz.NetCode
{
    public abstract class WorldBase : IWorldBase, IWorldCallbackProvider
    {
        protected readonly IWorldCreator WorldCreator;
        public World World => WorldCreator.World;
        public Action WorldCreated { get; set; }
        public Action WorldDestroyed { get; set; }
        public Action PreWorldDestroy { get; set; }

        protected IWorldOptions Options { get; }

        protected WorldBase(IWorldOptions options, IWorldCreator worldCreator)
        {
            if (worldCreator is null)
            {
                throw new ArgumentNullException(nameof(worldCreator));
            }

            Options = options ?? throw new ArgumentNullException(nameof(options));

            WorldCreator = worldCreator;

            worldCreator.WorldCreated += () => WorldCreated?.Invoke();

            WorldCreated += OnWorldCreated;

            if (Options.CreateWorldOnInstantiate)
            {
                CreateWorld();
            }
        }

        private void OnWorldCreated()
        {
            World.EnqueueEvent(new WorldCreatedEvent());
            World.GetHookSystem().RegisterHook<DestroyWorldEvent>(e => PreWorldDestroy?.Invoke());
            World.GetExistingSystem<DestroyWorldSystem>().OnDestroyed += () => WorldDestroyed?.Invoke();
        }

        public void CreateWorld()
        {
            WorldCreator.CreateWorld();
        }

        public void DestroyWorld()
        {
            if (!WorldCreator.WorldIsCreated)
            {
                return;
            }

            World.CreateSingleton<DestroyWorld>();
        }

        public void Dispose()
        {
            WorldCreator.Dispose();
        }
    }
}