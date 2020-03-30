using System;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

namespace Sibz.NetCode.Internal.Util
{
    public interface IImportMethods
    {
        void ImportSharedDataPrefabs(IEnumerable<GameObject> sharedDataPrefabs);
        void ImportSystemsFromList(World world, IEnumerable<Type> systems, bool isClient);
        Type GetDefaultGroupType(bool isClient);
        void ImportSystems(World world, IEnumerable<Type> systems,
            bool isClient, IImportMethods im = null);
    }
}