using System;
using System.Collections.Generic;
using UnityEngine;

namespace Sibz.NetCode.Tests
{
    public class MyWorldOptions : IWorldOptions
    {
        public static int TestCount;
        public static MyWorldOptions Defaults => new MyWorldOptions();
        public string WorldName => $"TestWorld{TestCount++}";
        public bool CreateWorldOnInstantiate { get; set; }
        public List<Type> Systems { get; } = new List<Type>();
        public List<Type> SystemAttributes { get; set; } = new List<Type>();
        public List<GameObject> GhostCollectionPrefabs { get; set; } = new List<GameObject>();
    }
}