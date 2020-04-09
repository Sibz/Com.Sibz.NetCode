using System;
using System.Collections.Generic;
using Sibz.EntityEvents;
using Sibz.NetCode.WorldExtensions;
using Unity.Entities;
using UnityEngine;

namespace Sibz.NetCode
{
    public abstract class WorldManagerBase : IWorldManager, IWorldCallbackProvider
    {
        private const string WorldAlreadyCreatedError = "Can not create world as world is already created.";

        private const string NoPrefabsWarning = "Option {0} is null, or list is empty. World can only communicate " +
                                                "if ghost collections are present.";

        public World World { get; protected set; }
        public bool WorldIsCreated => World?.IsCreated ?? false;
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

        protected abstract void InjectSystems(List<Type> systems);

        public void CreateWorld(List<Type> systems)
        {
            if (systems is null)
            {
                throw new ArgumentNullException(nameof(systems));
            }

            if (WorldIsCreated)
            {
                throw new InvalidOperationException(WorldAlreadyCreatedError);
            }

            World = BootStrapCreateWorld(Options.WorldName);

            InjectSystems(systems);

            ImportPrefabs();

            WorldCreated?.Invoke();
        }

        public virtual void ImportPrefabs()
        {
            if (Options.GhostCollectionPrefabs is null || Options.GhostCollectionPrefabs.Count == 0)
            {
                Debug.LogWarning(string.Format(NoPrefabsWarning, nameof(Options.GhostCollectionPrefabs)));
                return;
            }

            World.ImportGhostCollection(Options.GhostCollectionPrefabs);
        }

        public void DestroyWorld()
        {
            if (!WorldIsCreated)
            {
                return;
            }

            PreWorldDestroy?.Invoke();

            World.Dispose();

            WorldDestroyed?.Invoke();
        }

        public void Dispose() => DestroyWorld();
        public Action WorldCreated { get; set; }
        public Action WorldDestroyed { get; set; }
        public Action PreWorldDestroy { get; set; }
    }
}