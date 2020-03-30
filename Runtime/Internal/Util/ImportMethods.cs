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
    public class ImportMethods : IImportMethods
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

        public void ImportSystemsWithAttributes(World world, IEnumerable<Type> attributes, bool isClient)
        {
            if (world is null)
            {
                throw new ArgumentNullException(nameof(world));
            }

            if (attributes is null)
            {
                throw new ArgumentNullException(nameof(attributes));
            }

            Type defaultGroupType =
                isClient ? typeof(ClientSimulationSystemGroup) : typeof(ServerSimulationSystemGroup);
            foreach (Type attribute in attributes)
            {
                world.ImportSystemsWithAttribute(attribute, defaultGroupType);
            }
        }

        public void ImportSystemsFromList(World world, IEnumerable<Type> systems, bool isClient)
        {
            world.ImportSystemsFromList(systems, GetDefaultGroupType(isClient));
        }

        private static Type GetDefaultGroupType(bool isClient) =>
            isClient ? typeof(ClientSimulationSystemGroup) : typeof(ServerSimulationSystemGroup);
    }
}