using System;
using System.Collections.Generic;
using UnityEngine;

namespace Sibz.NetCode
{
    public interface IWorldCreatorOptions
    {
        string WorldName { get; }
        List<Type> Systems { get; }
        List<Type> SystemAttributes { get; }
        List<GameObject> GhostCollectionPrefabs { get; }
    }
}