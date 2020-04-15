using System;
using Unity.Entities;

namespace Sibz.NetCode.Server
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