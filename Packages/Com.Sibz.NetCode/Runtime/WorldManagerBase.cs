using System;
using System.Collections.Generic;
using Sibz.NetCode.WorldExtensions;
using Unity.Entities;
using UnityEngine;

namespace Sibz.NetCode
{
    public abstract class WorldManagerBase : IWorldManager
    {
        private const string WorldAlreadyCreatedError = "Can not create world as world is already created.";

        private const string NoPrefabsWarning = "Option {0} is null, or list is empty. World can only communicate " +
                                                "if ghost collections are present.";

        public World World { get; protected set; }
        public bool WorldIsCreated => World?.IsCreated ?? false;
        public IWorldCallbackProvider CallbackProvider { protected get; set; }
        public IWorldManagerOptions Options { get; }

        public WorldManagerBase(IWorldManagerOptions options, IWorldCallbackProvider callbackProvider = null)
        {
            if (options is null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            Options = options;

            CallbackProvider = callbackProvider;
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

            CallbackProvider?.WorldCreated?.Invoke();
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

            CallbackProvider?.PreWorldDestroy?.Invoke();

            World.Dispose();

            CallbackProvider?.WorldDestroyed?.Invoke();
        }

        public void Dispose() => DestroyWorld();
    }
}