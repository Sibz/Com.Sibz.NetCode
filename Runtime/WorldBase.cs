using System;
using System.Collections.Generic;
using Sibz.CommandBufferHelpers;
using Sibz.NetCode.Internal;
using Unity.Entities;
using Unity.NetCode;

[assembly: DisableAutoCreation]

namespace Sibz.NetCode
{
    public abstract class WorldBase
    {
        public readonly World World;

        protected readonly BeginInitCommandBuffer CommandBuffer;

        protected virtual List<Type> SystemAttributeTypes { get; } = new List<Type> {typeof(WorldBaseSystemAttribute)};
        protected virtual IEnumerable<Type> OverrideSystemImports { get; } = null;

        protected WorldBase(IWorldOptionsBase options, bool isClient)
        {
            WorldBaseInternal.CreateWorld(out World, options.WorldName, isClient);

            CommandBuffer = new BeginInitCommandBuffer(World);

            // ReSharper disable twice VirtualMemberCallInConstructor
            WorldBaseInternal.ImportSystems(World, SystemAttributeTypes, OverrideSystemImports, isClient);

            WorldBaseInternal.ImportMethods.ImportSharedDataPrefabs(options.SharedDataPrefabs);

            CreateEventEntity<WorldCreated>();
        }

        protected void CreateRpcRequest<T>(T rpcCommand)
            where T : struct, IRpcCommand => ConvertToRpcRequestSystem.CreateRpcRequest(World, rpcCommand);

        protected Entity CreateSingleton<T>(T data)
            where T : struct, IComponentData => WorldBaseInternal.CreateSingleton<T>(World, data);

        protected Entity CreateSingleton<T>()
            where T : struct, IComponentData => WorldBaseInternal.CreateSingleton<T>(World);

        protected Entity CreateEventEntity<T>()
            where T : struct, IComponentData =>
            CommandBuffer.Buffer.CreateSingleton<T>();


#if DEBUG
        public static int DebugLevel = 1;

        public static void Debug(string message, int level = 1)
        {
            if (level <= DebugLevel)
            {
                UnityEngine.Debug.Log(message);
            }
        }
#endif
    }
}