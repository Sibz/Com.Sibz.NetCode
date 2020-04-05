using System;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.NetCode;
using UnityEngine;

namespace Sibz
{
    public static partial class Util
    {
        private const string NoSystemError = "{0}: {1} does not exist.";

        public static void ImportGhostCollections(this World world, List<GameObject> prefabs)
        {
            if (world == null)
            {
                throw new ArgumentNullException(nameof(world));
            }

            if (prefabs == null)
            {
                throw new ArgumentNullException(nameof(prefabs));
            }

            if (World.DefaultGameObjectInjectionWorld.GetExistingSystem<ConvertToEntitySystem>() is
                null)
            {
                throw new InvalidOperationException(
                    string.Format(NoSystemError, nameof(ImportGhostCollections), nameof(ConvertToEntitySystem)));
            }

            foreach (GameObject prefab in prefabs)
            {
                ImportGhostCollection(world, prefab);
            }
        }

        private static void ImportGhostCollection(World world, GameObject prefab)
        {
            if (prefab.GetComponentInChildren<GhostCollectionAuthoringComponent>() is null)
            {
                throw new ArgumentException(
                    $"{nameof(ImportGhostCollection)}: Prefab should have {nameof(GhostCollectionAuthoringComponent)} attached.",
                    nameof(prefab));
            }

            var convertToEntitySystem =
                World.DefaultGameObjectInjectionWorld.GetOrCreateSystem<ConvertToEntitySystem>();
            var conversionSettings =
                new GameObjectConversionSettings(
                    world,
                    GameObjectConversionUtility.ConversionFlags.AssignName,
                    convertToEntitySystem.BlobAssetStore
                );

            Entity entity = GameObjectConversionUtility.ConvertGameObjectHierarchy(
                prefab, conversionSettings);

            RemovePrefabComponentFromSelfAndDirectChildren(world, entity);
        }

        private static void RemovePrefabComponentFromSelfAndDirectChildren(World world, Entity entity)
        {
            DynamicBuffer<LinkedEntityGroup> buff = world.EntityManager.GetBuffer<LinkedEntityGroup>(entity);

            var commandBuffer = new EntityCommandBuffer(Allocator.TempJob);

            for (int i = 0; i < buff.Length; i++)
            {
                commandBuffer.RemoveComponent<Prefab>(buff[i].Value);
            }

            commandBuffer.RemoveComponent<Prefab>(entity);
            commandBuffer.Playback(world.EntityManager);
            commandBuffer.Dispose();
        }
    }
}