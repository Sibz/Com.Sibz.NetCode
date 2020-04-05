using System;
using Unity.Networking.Transport;

namespace Sibz.NetCode
{
    public interface IServerWorldCallbackProvider : IWorldCallbackProvider
    {
        Action<NetworkConnection> ClientConnected { get; set; }
        Action<NetworkConnection> ClientDisconnected { get; set; }
        Action ListenSuccess { get; set; }
        Action ListenFailed { get; set; }
        Action Closed { get; set; }
    }
}