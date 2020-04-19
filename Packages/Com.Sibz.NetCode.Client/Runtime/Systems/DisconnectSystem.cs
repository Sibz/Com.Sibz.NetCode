using Sibz.CommandBufferHelpers;
using Sibz.EntityEvents;
using Unity.Entities;
using Unity.Jobs;
using Unity.NetCode;

namespace Sibz.NetCode.Client
{
    [ClientSystem]
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    public class DisconnectSystem : SystemBase
    {
        private EndSimCommandBuffer commandBuffer;
        private EntityQuery networkStreamInGameQuery;

        protected override void OnCreate()
        {
            RequireSingletonForUpdate<Disconnect>();
            commandBuffer = new EndSimCommandBuffer(World);
            networkStreamInGameQuery = GetEntityQuery(typeof(NetworkStreamInGame));
        }

        protected override void OnUpdate()
        {
            if (networkStreamInGameQuery.CalculateEntityCount() == 0)
            {
                EntityManager.DestroyEntity(GetSingletonEntity<Disconnect>());
                return;
            }

            DisconnectJob job = new DisconnectJob
            {
                CommandBuffer = commandBuffer.Concurrent,
                Disconnect = GetSingletonEntity<Disconnect>()
            };

            Dependency = Entities.WithAll<NetworkStreamInGame>().ForEach(
                (Entity networkStreamEntity, int entityInQueryIndex) =>
                {
                    job.Execute(entityInQueryIndex, networkStreamEntity);
                }).Schedule(Dependency);

            commandBuffer.AddJobDependency(Dependency);
            World.EventSystemAddJobDependency(Dependency);
        }
    }
}