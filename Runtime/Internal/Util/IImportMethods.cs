using System;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

namespace Sibz.NetCode.Internal.Util
{
    public interface IImportMethods
    {
        void ImportSharedDataPrefabs(IEnumerable<GameObject> sharedDataPrefabs);
        void ImportSystemsWithAttributes(World world, IEnumerable<Type> attributes, bool isClient);
        void ImportSystemsFromList(World world, IEnumerable<Type> systems, bool isClient);
    }
}