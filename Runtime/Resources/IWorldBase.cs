using System;
using Unity.Entities;

namespace Sibz.NetCode
{
    public interface IWorldBase : IDisposable
    {
        World World { get; }
    }
}