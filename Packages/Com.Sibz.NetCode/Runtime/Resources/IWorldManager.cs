using System;
using System.Collections.Generic;
using Unity.Entities;

namespace Sibz.NetCode
{
    public interface IWorldManager : IDisposable
    {
        World World { get; }
        bool CreateWorldOnInstantiate { get; }
        List<Type> GetSystemsList();
        void CreateWorld(List<Type> systems);
        void DestroyWorld();
    }
}