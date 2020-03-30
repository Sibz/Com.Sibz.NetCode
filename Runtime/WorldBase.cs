using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Sibz.CommandBufferHelpers;
using Sibz.NetCode.Internal;
using Sibz.NetCode.Internal.Util;
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

        private readonly IImportMethods importer = new ImportMethods();

        protected WorldBase(IWorldOptionsBase options, bool isClient)
        {
            CreateWorld(out World, options.WorldName, isClient);

            CommandBuffer = new BeginInitCommandBuffer(World);

            importer.ImportSystems(World, GetSystemTypes(), isClient);

            importer.ImportSharedDataPrefabs(options.SharedDataPrefabs);
            //CreateEventEntity<WorldCreated>();
        }

        private static void CreateWorld(out World world, string name, bool isClient)
        {
            world = isClient
                ? ClientServerBootstrap.CreateClientWorld(World.DefaultGameObjectInjectionWorld, name)
                : ClientServerBootstrap.CreateServerWorld(World.DefaultGameObjectInjectionWorld, name);
        }

        private List<Type> GetAttributeTypes()
        {
            var attrTypes = new List<Type>();
            attrTypes.AddRange(IncludeAttributeTypes(attrTypes));
            attrTypes.Add(typeof(WorldBaseSystemAttribute));
            return attrTypes;
        }

        private IEnumerable<Type> GetSystemTypes()
        {
            var systemTypes = new List<Type>();
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

        protected Entity CreateEventEntity<T>()
            where T : struct, IComponentData =>
            CommandBuffer.Buffer.CreateSingleton<T>();
        protected Entity CreateEventEntity<T>(T data)
            where T : struct, IComponentData =>
            CommandBuffer.Buffer.CreateSingleton(data);


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