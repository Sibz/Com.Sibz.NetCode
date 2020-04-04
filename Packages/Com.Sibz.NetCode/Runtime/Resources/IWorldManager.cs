using System;
using System.Collections.Generic;
using Unity.Entities;

namespace Sibz.NetCode
{
    public interface IWorldManager : IDisposable
    {
        IWorldManagerOptions Options { get; }
        World World { get; }
        void CreateWorld(List<Type> systems);
        void DestroyWorld();
    }
}