using System;
using System.Collections.Generic;
using Sibz.WorldSystemHelpers;
using Unity.Entities;
using Unity.NetCode;

namespace Sibz.NetCode.WorldExtensions
{
    public static class ImportGhostSystemsWorldExtension
    {
        private const string WrongWorldError = "Can only import ghost systems into client or server NetCode worlds";
        private const string GhostSendSystemName = "GhostSendSystem";
        private const string GhostReceiveSystemName = "GhostReceiveSystem";
        private const string DefaultGhostSpawnSystemName = "DefaultGhostSpawnSystem";

        public static void ImportGhostSystems(this World world)
        {
            if (world.GetExistingSystem<ClientSimulationSystemGroup>() is ClientSimulationSystemGroup)
            {
                ImportClientSystems(world);
            } else if (world.GetExistingSystem<ServerSimulationSystemGroup>() is ServerSimulationSystemGroup)
            {
                ImportServerSystems(world);
            }
            else
            {
                throw new InvalidOperationException(WrongWorldError);
            }
        }

        private static void ImportServerSystems(World world)
        {
            List<Type> systems = new List<Type>();
            systems.AppendTypesThatAreSubclassOf(GhostSendSystemName);
            RemoveSystemsThatAreAlreadyInWorld(world, systems);
            world.ImportSystemsFromList<ServerSimulationSystemGroup>(systems);
        }

        private static void ImportClientSystems(World world)
        {
            // GhostReceiveSystem, DefaultGhostSpawnSystem
            //[UpdateInGroup(typeof(GhostUpdateSystemGroup))]
            List<Type> systems = new List<Type>();
            systems.AppendTypesThatAreSubclassOf(GhostReceiveSystemName);
            systems.AppendTypesThatAreSubclassOf(DefaultGhostSpawnSystemName);
            systems.AppendTypesWithUpdateInGroupAttribute(typeof(GhostUpdateSystemGroup));
            RemoveSystemsThatAreAlreadyInWorld(world, systems);
            world.ImportSystemsFromList<ClientSimulationSystemGroup>(systems);
        }

        private static void RemoveSystemsThatAreAlreadyInWorld(World world, ICollection<Type> systems)
        {
            foreach (ComponentSystemBase worldSystem in world.Systems)
            {
                if (systems.Contains(worldSystem.GetType()))
                {
                    systems.Remove(worldSystem.GetType());
                }
            }
        }
    }
}