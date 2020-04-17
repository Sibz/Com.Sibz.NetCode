using System;
using System.Collections.Generic;
using Sibz.NetCode.WorldExtensions;
using Sibz.WorldSystemHelpers;
using Unity.Entities;
using Unity.NetCode;
using UnityEngine;

namespace Sibz.NetCode
{
    public abstract class WorldCreatorBase : IWorldCreator
    {
        private const string WorldAlreadyCreatedError = "Can not create world as world is already created.";

        private const string NoPrefabWarning = "Option {0} is null. World can only communicate " +
                                               "if a ghost collection is present.";

        private readonly List<Type> systemsCache = new List<Type>();

        public World World { get; protected set; }
        public bool WorldIsCreated => World?.IsCreated ?? false;
        public IWorldCreatorOptions Options { get; }
        public Action WorldCreated { get; set; }

        public virtual Type DefaultSystemGroup => SimSystemGroup;
        public virtual Type InitSystemGroup => typeof(ClientInitializationSystemGroup);
        public virtual Type SimSystemGroup => typeof(ClientSimulationSystemGroup);
        public virtual Type PresSystemGroup => typeof(ClientPresentationSystemGroup);

        public WorldCreatorBase(IWorldCreatorOptions options)
        {
            if (options is null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            Options = options;

            CacheSystems();
        }

        private void CacheSystems()
        {
            systemsCache.AppendTypesWithAttribute<ClientAndServerSystemAttribute>();
            foreach (Type attributeType in Options.SystemAttributes)
            {
                systemsCache.AppendTypesWithAttribute(attributeType);
            }

            systemsCache.AddRange(Options.Systems);
        }

        /// <summary>
        ///     Define call to ClientServerBootstrap.CreateServerWorld or CreateClientWorld in here.
        /// </summary>
        /// <param name="worldName"></param>
        /// <returns></returns>
        protected abstract World BootStrapCreateWorld(string worldName);

        private void InjectSystems()
        {
            World.ImportSystemsFromList(systemsCache, DefaultSystemGroup, new Dictionary<Type, Type>
            {
                { typeof(InitializationSystemGroup), InitSystemGroup },
                { typeof(SimulationSystemGroup), SimSystemGroup },
                { typeof(PresentationSystemGroup), PresSystemGroup }
            });
        }

        public void CreateWorld()
        {
            if (WorldIsCreated)
            {
                throw new InvalidOperationException(WorldAlreadyCreatedError);
            }

            World = BootStrapCreateWorld(Options.WorldName);

            InjectSystems();

            ImportPrefabs();

            WorldCreated?.Invoke();
        }

        public virtual void ImportPrefabs()
        {
            if (Options.GhostCollectionPrefab is null)
            {
                Debug.LogWarning(string.Format(NoPrefabWarning, nameof(Options.GhostCollectionPrefab)));
                return;
            }

            World.ImportGhostCollection(Options.GhostCollectionPrefab);
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