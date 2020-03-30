﻿using Unity.Entities;
using Unity.Jobs;
using Unity.NetCode;
using UnityEngine;

namespace Sibz.NetCode
{
    [WorldBaseSystem]
    [UpdateBefore(typeof(RpcCommandRequestSystemGroup))]
    public class ConvertToRpcRequestSystem : JobComponentSystem
    {
        private EntityQuery requiredQuery;
        private EndInitializationEntityCommandBufferSystem bufferSystem;

        public static void CreateInWorld(World world)
        {
            if (world.GetExistingSystem<ClientSimulationSystemGroup>() is ClientSimulationSystemGroup g)
            {
                g.AddSystemToUpdateList(world.CreateSystem<ConvertToRpcRequestSystem>());
                g.SortSystemUpdateList();
            }
            else if (world.GetExistingSystem<ServerSimulationSystemGroup>() is ServerSimulationSystemGroup g2)
            {
                g2.AddSystemToUpdateList(world.CreateSystem<ConvertToRpcRequestSystem>());
                g2.SortSystemUpdateList();
            }
            else
            {
                Debug.LogWarning("Unable to create ConvertToRpcRequestSystem in world - No Server/Client InitializationSystemGroup");
            }
        }

        public static void CreateRpcRequest<T>(World world, T rpcCommand)
            where T : struct, IRpcCommand
        {
            if (world is null)
            {
                return;
            }
            Entity e = world.EntityManager.CreateEntity(typeof(ConvertToRpcRequest));
            world.EntityManager.AddComponentData(e, rpcCommand);
        }

        protected override void OnCreate()
        {
            requiredQuery = GetEntityQuery(typeof(ConvertToRpcRequest));
            RequireForUpdate(requiredQuery);
            bufferSystem = World.GetExistingSystem<EndInitializationEntityCommandBufferSystem>();
        }

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            ConvertToRpcRequestJob convertToRpcRequestJob = new ConvertToRpcRequestJob
            {
                CommandBuffer = bufferSystem.CreateCommandBuffer().ToConcurrent(),
                TargetConnection = GetSingletonEntity<CommandTargetComponent>()
            };

            inputDeps = Entities.WithAll<ConvertToRpcRequest>().ForEach((Entity e, int entityInQueryIndex) =>
            {
                convertToRpcRequestJob.Execute(e, entityInQueryIndex);
            }).Schedule(inputDeps);

            bufferSystem.AddJobHandleForProducer(inputDeps);

            return inputDeps;
        }
    }
}