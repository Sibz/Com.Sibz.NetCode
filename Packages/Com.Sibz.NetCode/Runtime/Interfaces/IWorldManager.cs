using System;
using System.Collections.Generic;
using Unity.Entities;

namespace Sibz.NetCode
{
    public interface IWorldManager : IDisposable
    {
        IWorldCallbackProvider CallbackProvider { set; }
        IWorldManagerOptions Options { get; }
        World World { get; }
        void CreateWorld(List<Type> systems);
        void DestroyWorld();
    }
}