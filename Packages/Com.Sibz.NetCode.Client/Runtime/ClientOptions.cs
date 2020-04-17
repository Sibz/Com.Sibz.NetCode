using System;
using System.Collections.Generic;
using Unity.Networking.Transport;
using UnityEngine;

namespace Sibz.NetCode
{
    public class ClientOptions : INetworkEndpointSettings, IWorldOptions
    {
        public string Address { get; set; } = "0.0.0.0";
        public ushort Port { get; set; } = 21650;
        public NetworkFamily NetworkFamily { get; set; } = NetworkFamily.Ipv4;
        public string WorldName { get; set; } = "Client";
        public bool CreateWorldOnInstantiate { get; set; } = true;

        public List<Type> Systems { get; set; } = new List<Type>();

        public List<Type> SystemAttributes { get; } = new List<Type> { typeof(ClientSystemAttribute) };

        public GameObject GhostCollectionPrefab { get; set; }
        public int TimeOut { get; set; } = 10;
    }
}