using System;
using System.Collections.Generic;
using System.Linq;
using Sibz.NetCode.Internal.Util;
using Sibz.WorldSystemHelpers;
using Unity.Entities;
using Unity.NetCode;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Sibz.NetCode.Internal
{
#if !RELEASE
    public static class WorldBaseInternal
#else
    internal static class WorldBaseInternal
#endif
    {
        internal static readonly ImportMethods ImportMethods = new ImportMethods();

        // TODO Single functions should in separate package (EntityHelpers?)
        // Also wouldn't hurt to make them extensions
        public static Entity CreateSingleton<T>(World world, T data)
            where T : struct, IComponentData
        {
            Entity entity = CreateSingleton<T>(world);
            world.EntityManager.SetComponentData(entity, data);
            return entity;
        }

        public static Entity CreateSingleton<T>(World world)
            where T : IComponentData
        {
            return world.EntityManager.CreateEntity(typeof(T));
        }

        public static void CreateWorld(out World world, string name, bool isClient)
        {
            world = isClient
                ? ClientServerBootstrap.CreateClientWorld(World.DefaultGameObjectInjectionWorld, name)
                : ClientServerBootstrap.CreateServerWorld(World.DefaultGameObjectInjectionWorld, name);
        }

        public static void ImportSystems(World world, IEnumerable<Type> systems,
            bool isClient, IImportMethods im = null)
        {
            if (world is null || systems  is null)
            {
                throw new ArgumentNullException(world is null?nameof(world):nameof(systems));
            }

            (im ?? ImportMethods).ImportSystemsFromList(world, systems, isClient);
        }
    }
}