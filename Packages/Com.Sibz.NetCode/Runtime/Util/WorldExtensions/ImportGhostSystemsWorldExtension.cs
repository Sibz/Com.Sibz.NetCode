using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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

        public static void ImportGhostSystems(this World world, string filter)
        {
            filter = filter ?? "";
            if (world.GetExistingSystem<ClientSimulationSystemGroup>() is ClientSimulationSystemGroup)
            {
                ImportClientSystems(world, filter);
            }
            else if (world.GetExistingSystem<ServerSimulationSystemGroup>() is ServerSimulationSystemGroup)
            {
                ImportServerSystems(world, filter);
            }
            else
            {
                throw new InvalidOperationException(WrongWorldError);
            }
        }

        private static void ImportServerSystems(World world, string filter)
        {
            ImportSystems(world, new List<Type>()
                .AppendTypesThatAreSubclassOf(GhostSendSystemName), filter);
        }

        private static void ImportClientSystems(World world, string filter)
        {
            ImportSystems(world, new List<Type>()
                .AppendTypesThatAreSubclassOf(GhostReceiveSystemName), filter);
            ImportSystems(world, new List<Type>()
                .AppendTypesThatAreSubclassOf(DefaultGhostSpawnSystemName), filter);
            ImportSystems(world, new List<Type>()
                .AppendTypesWithUpdateInGroupAttribute(typeof(GhostUpdateSystemGroup)), filter);
        }

        private static void ImportSystems(World world, ICollection<Type> systems, string filter)
        {
            systems = systems.Where(x => x.Name.StartsWith(filter)).ToList();
            RemoveSystemsThatAreAlreadyInWorld(world, systems, out ICollection<ComponentSystemBase> removedSystems);
            EnsureRemovedSystemsGetUpdatedInThereGroup(world, removedSystems);
            world.ImportSystemsFromList<ClientSimulationSystemGroup>(systems);
        }

        private static void EnsureRemovedSystemsGetUpdatedInThereGroup(World world, IEnumerable<ComponentSystemBase> removedSystems)
        {
            foreach (ComponentSystemBase removedSystem in removedSystems)
            {
                Type type = removedSystem.GetType();
                if (!(type.GetCustomAttribute<UpdateInGroupAttribute>() is UpdateInGroupAttribute att)
                    || !(world.GetExistingSystem(att.GroupType) is ComponentSystemGroup group)
                    || group.Systems.Contains(removedSystem))
                {
                    continue;
                }

                group.AddSystemToUpdateList(removedSystem);
                group.SortSystemUpdateList();
            }
        }

        private static void RemoveSystemsThatAreAlreadyInWorld(
            World world, ICollection<Type> systems, out ICollection<ComponentSystemBase> removedSystems)
        {
            removedSystems = new List<ComponentSystemBase>();
            foreach (ComponentSystemBase worldSystem in world.Systems)
            {
                if (systems.Contains(worldSystem.GetType()))
                {
                    removedSystems.Add(worldSystem);
                    systems.Remove(worldSystem.GetType());
                }
            }
        }
    }
}