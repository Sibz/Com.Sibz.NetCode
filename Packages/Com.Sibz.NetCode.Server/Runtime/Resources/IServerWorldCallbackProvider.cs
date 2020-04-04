﻿using System;
using Unity.Networking.Transport;

namespace Sibz.NetCode
{
    public interface IServerWorldCallbackProvider
    {
        Action<NetworkConnection> ClientConnected { get; set; }
        Action<NetworkConnection> ClientDisconnected { get; set; }
    }
}