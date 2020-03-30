using System;
using System.Collections.Generic;
using System.Linq;
using Sibz.WorldSystemHelpers;
using Unity.Entities;
using Unity.NetCode;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Sibz.NetCode.Internal.Util
{
    internal class ImportMethods : IImportMethods
    {
        public void ImportSharedDataPrefabs(IEnumerable<GameObject> sharedDataPrefabs)
        {
            if (sharedDataPrefabs is null)
            {
                throw new ArgumentNullException(nameof(sharedDataPrefabs));
            }
            var prefabs = sharedDataPrefabs.ToArray();
            foreach (GameObject prefab in prefabs)
            {
                Object.Instantiate(prefab);
            }
        }

        public void ImportSystemsFromList(World world, IEnumerable<Type> systems, bool isClient)
        {
            world.ImportSystemsFromList(systems, GetDefaultGroupType(isClient));
        }

        public Type GetDefaultGroupType(bool isClient) =>
            isClient ? typeof(ClientSimulationSystemGroup) : typeof(ServerSimulationSystemGroup);
    }
}