using System;
using System.Collections.Generic;
using Unity.Networking.Transport;
using UnityEngine;

namespace Sibz.NetCode
{
    public interface IWorldOptionsBase
    {
        string WorldName { get; set; }

        /// <summary>
        ///     Root prefabs containing collections to import into world
        /// </summary>
        List<GameObject> SharedDataPrefabs { get; }

        /// <summary>
        ///     Systems with these attributes will be imported into the world so long as they are in the
        ///     same assembly as the attribute
        /// </summary>
        List<Type> SystemImportAttributes { get; }

        bool ConnectOnSpawn { get; set; }

        int ConnectTimeout { get; set; }
        string Address { get; set; }
        ushort Port { get; set; }
        NetworkFamily NetworkFamily { get; set; }
    }
}