using System;
using System.Collections.Generic;
using UnityEngine;

namespace Sibz.NetCode
{
    public interface IWorldManagerOptions
    {
        string WorldName { get; }
        bool CreateWorldOnInstantiate { get; }
        List<Type> Systems { get; }
        List<GameObject> GhostCollectionPrefabs { get; }
    }
}