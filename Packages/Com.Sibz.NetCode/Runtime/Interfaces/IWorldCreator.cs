using System;
using Unity.Entities;

namespace Sibz.NetCode
{
    public interface IWorldCreator : IDisposable, IWorldCreatorCallbackProvider
    {
        bool WorldIsCreated { get; }
        IWorldCreatorOptions Options { get; }
        World World { get; }
        Type DefaultSystemGroup { get; }
        Type InitSystemGroup { get; }
        Type SimSystemGroup { get; }
        Type PresSystemGroup { get; }
        void CreateWorld();
    }
}