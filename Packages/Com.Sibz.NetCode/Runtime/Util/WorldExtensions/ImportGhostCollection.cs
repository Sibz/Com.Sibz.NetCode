﻿using System;
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
            ImportGhostCollection(world, GetConversionSettings(world), prefab);
        }

        private static void ImportGhostCollection(World world, GameObjectConversionSettings settings, GameObject prefab)
        {
            // ReSharper disable once Unity.NoNullCoalescing
            prefab = prefab ?? throw new ArgumentNullException(nameof(prefab));
            world = world ?? throw new ArgumentNullException(nameof(world));
            settings = settings ?? throw new ArgumentNullException(nameof(settings));

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

            Entity entity = GameObjectConversionUtility.ConvertGameObjectHierarchy(prefab, settings);
            RemovePrefabComponentFromEntityAndDirectChildren(world, entity);
        }

        private static void RemovePrefabComponentFromEntityAndDirectChildren(World world, Entity entity)
        {
            DynamicBuffer<LinkedEntityGroup> buff = world.EntityManager.GetBuffer<LinkedEntityGroup>(entity);
            EntityCommandBuffer commandBuffer = new EntityCommandBuffer(Allocator.TempJob);

            for (int i = 0; i < buff.Length; i++)
            {
                commandBuffer.RemoveComponent<Prefab>(buff[i].Value);
            }

            commandBuffer.RemoveComponent<Prefab>(entity);
            commandBuffer.Playback(world.EntityManager);
            commandBuffer.Dispose();
        }

        private static GameObjectConversionSettings GetConversionSettings(World world)
        {
            return new GameObjectConversionSettings(
                world,
                GameObjectConversionUtility.ConversionFlags.AssignName,
                (World.DefaultGameObjectInjectionWorld.GetOrCreateSystem<ConvertToEntitySystem>()
                 ?? throw new InvalidOperationException(
                     string.Format(NoSystemError, nameof(ImportGhostCollection), nameof(ConvertToEntitySystem))
                 )).BlobAssetStore);
        }
    }
}