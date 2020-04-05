using System;
using System.Collections.Generic;
using Unity.Networking.Transport;
using UnityEngine;

namespace Sibz.NetCode
{
    public class ClientOptions : INetworkEndpointSettings
    {
        public string WorldName { get; set; } = "Client";
        public List<GameObject> SharedDataPrefabs { get; } = new List<GameObject>();
        public List<Type> SystemImportAttributes { get; } = new List<Type>();
        public bool ConnectOnSpawn { get; set; } = false;
        public int ConnectTimeout { get; set; } = 10;
        public string Address { get; set; } = "127.0.0.1";
        public ushort Port { get; set; } = 21650;
        public NetworkFamily NetworkFamily { get; set; } = NetworkFamily.Ipv4;
    }
}