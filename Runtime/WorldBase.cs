using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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

        protected virtual List<Type> IncludeAttributeTypes(List<Type> attributeTypes) => attributeTypes;
        protected virtual List<Type> IncludeAdditionalSystems(List<Type> systemTypes) => systemTypes;

        protected WorldBase(IWorldOptionsBase options, bool isClient)
        {
            WorldBaseInternal.CreateWorld(out World, options.WorldName, isClient);

            CommandBuffer = new BeginInitCommandBuffer(World);

            //ReSharper disable twice VirtualMemberCallInConstructor
            WorldBaseInternal.ImportSystems(World, GetSystemTypes(), isClient);

            WorldBaseInternal.ImportMethods.ImportSharedDataPrefabs(options.SharedDataPrefabs);

            //CreateEventEntity<WorldCreated>();
        }

        private List<Type> GetAttributeTypes()
        {
            List<Type> attrTypes = new List<Type>();
            attrTypes.AddRange(IncludeAttributeTypes(attrTypes));
            attrTypes.Add(typeof(WorldBaseSystemAttribute));
            return attrTypes;
        }

        private List<Type> GetSystemTypes()
        {
            List<Type> systemTypes = new List<Type>();
            systemTypes.AddRange(IncludeAdditionalSystems(systemTypes));
            var attrArray = GetAttributeTypes().ToArray();
            foreach (Type attrType in attrArray)
            {
                systemTypes.AddRange(Assembly.GetAssembly(attrType).GetTypes()
                    .Where(x => !(x.GetCustomAttribute(attrType) is null)));
            }
            return systemTypes;
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