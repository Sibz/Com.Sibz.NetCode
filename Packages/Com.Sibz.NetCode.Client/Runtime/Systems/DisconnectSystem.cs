using Sibz.CommandBufferHelpers;
using Sibz.EntityEvents;
using Unity.Entities;
using Unity.Jobs;
using Unity.NetCode;

namespace Sibz.NetCode.Client
{
    [ClientSystem]
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    public class DisconnectSystem : JobComponentSystem
    {
        private EndSimCommandBuffer commandBuffer;
        private EntityQuery networkStreamInGameQuery;

        protected override void OnCreate()
        {
            RequireSingletonForUpdate<Disconnect>();
            commandBuffer = new EndSimCommandBuffer(World);
            networkStreamInGameQuery = GetEntityQuery(typeof(NetworkStreamInGame));
        }

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            if (networkStreamInGameQuery.CalculateEntityCount() == 0)
            {
                EntityManager.DestroyEntity(GetSingletonEntity<Disconnect>());
                return inputDeps;
            }

            DisconnectJob job = new DisconnectJob
            {
                CommandBuffer = commandBuffer.Concurrent,
                Disconnect = GetSingletonEntity<Disconnect>()
            };

            inputDeps = Entities.WithAll<NetworkStreamInGame>().ForEach(
                (Entity networkStreamEntity, int entityInQueryIndex) =>
                {
                    job.Execute(entityInQueryIndex, networkStreamEntity);
                }).Schedule(inputDeps);

            commandBuffer.AddJobDependency(inputDeps);
            World.EventSystemAddJobDependency(inputDeps);

            return inputDeps;
        }


    }
}