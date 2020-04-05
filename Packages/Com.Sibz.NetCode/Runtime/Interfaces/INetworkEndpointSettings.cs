using System;
using System.Collections.Generic;
using Unity.Networking.Transport;
using UnityEngine;

namespace Sibz.NetCode
{
    public interface INetworkEndpointSettings
    {
        string Address { get; set; }
        ushort Port { get; set; }
        NetworkFamily NetworkFamily { get; set; }
    }
}