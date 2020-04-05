using System;
using System.Collections.Generic;
using UnityEngine;

namespace Sibz.NetCode.Tests
{
    public class MyWorldManagerOptions : IWorldManagerOptions
    {
        public static int TestCount;
        public static MyWorldManagerOptions Defaults => new MyWorldManagerOptions();
        public string WorldName => $"TestWorld{TestCount++}";
        public bool CreateWorldOnInstantiate { get; set; }
        public List<Type> Systems { get; } = new List<Type>();
        public List<GameObject> GhostCollectionPrefabs { get; set; } = new List<GameObject>();
    }
}