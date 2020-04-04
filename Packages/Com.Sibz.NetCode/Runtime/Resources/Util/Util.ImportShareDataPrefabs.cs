using System;
using System.Collections.Generic;
using Unity.Entities;
using Unity.NetCode;
using UnityEngine;

namespace Sibz
{
    public static partial class Util
    {
        private const string NoSystemError = "{0}: {1} does not exist.";

        public static void ImportShareDataPrefabs(World world, List<GameObject> prefabs)
        {
            if (prefabs == null)
            {
                throw new ArgumentNullException(nameof(prefabs));
            }

            if (!(World.DefaultGameObjectInjectionWorld.GetOrCreateSystem<ConvertToEntitySystem>() is
                ConvertToEntitySystem ctes))
            {
                throw new InvalidOperationException(
                    string.Format(NoSystemError, nameof(ImportShareDataPrefabs), nameof(ConvertToEntitySystem)));
            }

            foreach (GameObject prefab in prefabs)
            {
                var conversionSettings = new GameObjectConversionSettings(
                    world,
                    GameObjectConversionUtility.ConversionFlags.AssignName |
                    GameObjectConversionUtility.ConversionFlags.AddEntityGUID,
                    ctes.BlobAssetStore);
                ctes.AddToBeConverted(world, prefab.GetComponent<ConvertToClientServerEntity>());

                GameObjectConversionUtility.ConvertGameObjectHierarchy(prefab, conversionSettings);
            }
        }
    }
}