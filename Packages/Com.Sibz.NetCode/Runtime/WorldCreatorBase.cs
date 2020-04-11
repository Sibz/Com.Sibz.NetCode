using System;
using System.Collections.Generic;
using Sibz.NetCode.WorldExtensions;
using Unity.Entities;
using UnityEngine;

namespace Sibz.NetCode
{
    public abstract class WorldCreatorBase : IWorldCreator
    {
        private const string WorldAlreadyCreatedError = "Can not create world as world is already created.";

        private const string NoPrefabsWarning = "Option {0} is null, or list is empty. World can only communicate " +
                                                "if ghost collections are present.";

        public World World { get; protected set; }
        public bool WorldIsCreated => World?.IsCreated ?? false;
        public IWorldCreatorOptions Options { get; }
        public Action WorldCreated { get; set; }

        public WorldCreatorBase(IWorldCreatorOptions options)
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

        public void CreateWorld()
        {
            if (WorldIsCreated)
            {
                throw new InvalidOperationException(WorldAlreadyCreatedError);
            }

            World = BootStrapCreateWorld(Options.WorldName);

            InjectSystems(Options.Systems);

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

        public void Dispose()
        {
            if (WorldIsCreated)
            {
                World.Dispose();
            }
        }
    }
}