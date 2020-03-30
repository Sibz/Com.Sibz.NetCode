using System;
using System.Collections.Generic;
using Sibz.NetCode.Internal.Util;
using Unity.Entities;
using UnityEngine;

namespace Sibz.NetCode.Tests.Util
{
    public class ImportMethodsTest : IImportMethods
    {
        public CalledMethod Called;
        public enum CalledMethod
        {
            ImportSystemsFromList,
            ImportSharedDataPrefabs
        }

        public void ImportSharedDataPrefabs(IEnumerable<GameObject> sharedDataPrefabs)
        {
            Called = CalledMethod.ImportSharedDataPrefabs;
        }


        public void ImportSystemsFromList(World world, IEnumerable<Type> systems, bool isClient)
        {
            Called = CalledMethod.ImportSystemsFromList;
        }

        public Type GetDefaultGroupType(bool isClient)
        {
            throw new NotImplementedException();
        }
    }
}