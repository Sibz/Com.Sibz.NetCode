using System;
using Unity.Entities;
using Unity.Networking.Transport;

namespace Sibz.NetCode
{
    public interface IServerWorldCallbackProvider : IWorldCallbackProvider
    {
        Action<Entity> ClientConnected { get; set; }
        Action<int> ClientDisconnected { get; set; }
        Action ListenSuccess { get; set; }
        Action ListenFailed { get; set; }
        Action Closed { get; set; }
    }
}