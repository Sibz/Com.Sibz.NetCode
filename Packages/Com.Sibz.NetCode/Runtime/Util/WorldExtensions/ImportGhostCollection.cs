using System;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.NetCode;
using UnityEngine;

namespace Sibz.NetCode.WorldExtensions
{
    public static class ImportGhostCollectionWorldExtension
    {
        private const string NoSystemError = "{0}: {1} does not exist.";
        private const string InvalidPrefabError = "{0}: Prefab should have {1} attached.";

        public static void ImportGhostCollection(this World world, GameObject prefab)
        {
            var convertToEntitySystem =
                World.DefaultGameObjectInjectionWorld.GetOrCreateSystem<ConvertToEntitySystem>();

            ImportGhostCollection(world, convertToEntitySystem, prefab);
        }

        public static void ImportGhostCollection(this World world, IEnumerable<GameObject> prefabs)
        {
            var convertToEntitySystem =
                World.DefaultGameObjectInjectionWorld.GetOrCreateSystem<ConvertToEntitySystem>();

            if (prefabs is null)
            {
                throw new ArgumentNullException(nameof(prefabs));
            }

            foreach (GameObject prefab in prefabs)
            {
                ImportGhostCollection(world, convertToEntitySystem, prefab);
            }
        }

        private static void ImportGhostCollection(World world, ConvertToEntitySystem convertToEntitySystem, GameObject prefab)
        {
            if (world is null)
            {
                throw new ArgumentNullException(nameof(world));
            }

            if (convertToEntitySystem is null)
            {
                throw new InvalidOperationException(
                    string.Format(NoSystemError, nameof(ImportGhostCollection), nameof(ConvertToEntitySystem)));
            }

            if (prefab is null)
            {
                throw new ArgumentNullException(nameof(prefab));
            }

            if (prefab.GetComponentInChildren<GhostCollectionAuthoringComponent>() is null)
            {
                throw new ArgumentException(
                    string.Format(
                        InvalidPrefabError,
                        nameof(ImportGhostCollection),
                        nameof(GhostCollectionAuthoringComponent)
                    ),
                    nameof(prefab)
                );
            }

            var conversionSettings =
                new GameObjectConversionSettings(
                    world,
                    GameObjectConversionUtility.ConversionFlags.AssignName,
                    convertToEntitySystem.BlobAssetStore
                );

            Entity entity = GameObjectConversionUtility.ConvertGameObjectHierarchy(
                prefab, conversionSettings);

            RemovePrefabComponentFromEntityAndDirectChildren(world, entity);
        }

        private static void RemovePrefabComponentFromEntityAndDirectChildren(World world, Entity entity)
        {
            DynamicBuffer<LinkedEntityGroup> buff = world.EntityManager.GetBuffer<LinkedEntityGroup>(entity);

            var commandBuffer = new EntityCommandBuffer(Allocator.TempJob);

            for (var i = 0; i < buff.Length; i++)
            {
                commandBuffer.RemoveComponent<Prefab>(buff[i].Value);
            }

            commandBuffer.RemoveComponent<Prefab>(entity);
            commandBuffer.Playback(world.EntityManager);
            commandBuffer.Dispose();
        }
    }
}