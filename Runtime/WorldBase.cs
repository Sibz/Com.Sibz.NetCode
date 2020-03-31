using System;
using System.Collections.Generic;
using Sibz.CommandBufferHelpers;
using Sibz.NetCode.Internal;
using Sibz.WorldSystemHelpers;
using Unity.Entities;
using Unity.NetCode;

[assembly: DisableAutoCreation]

namespace Sibz.NetCode
{
    public abstract class WorldBase<T> : IDisposable
        where T : ComponentSystemGroup
    {
        public readonly World World;

        protected readonly BeginInitCommandBuffer CommandBuffer;

        protected WorldBase(IWorldOptionsBase options, Func<World, string, World> creationMethod,
            List<Type> systems = null)
        {
            if (creationMethod is null || options is null)
            {
                throw new ArgumentNullException(creationMethod is null ? nameof(creationMethod) : nameof(options));
            }

            World = creationMethod.Invoke(World.DefaultGameObjectInjectionWorld, options.WorldName);

            CommandBuffer = new BeginInitCommandBuffer(World);

            World.ImportSystemsFromList<T>(Util.GetSystemsWithAttribute<WorldBaseSystemAttribute>(systems));

            Util.InstantiateFromList(options.SharedDataPrefabs);

            CreateEventEntity<WorldCreated>();
        }

        protected void CreateRpcRequest<T>(T rpcCommand)
            where T : struct, IRpcCommand => ConvertToRpcRequestSystem.CreateRpcRequest(World, rpcCommand);

        protected Entity CreateEventEntity<T>()
            where T : struct, IComponentData =>
            CommandBuffer.Buffer.CreateSingleton<T>();

        protected Entity CreateEventEntity<T>(T data)
            where T : struct, IComponentData =>
            CommandBuffer.Buffer.CreateSingleton(data);

        public void Dispose()
        {
            World.Dispose();
        }
    }
}