using System;
using Unity.Entities;
using Unity.Jobs;
using Unity.NetCode;

namespace Sibz.NetCode
{
    [ClientAndServerSystem]
    public class CreateRpcRequestSystem : JobComponentSystem
    {
        private EntityQuery networkIdEntityQuery;

        public Entity CommandTargetComponentEntity =>
            networkIdEntityQuery.GetSingletonEntity();

        private static Entity GetTargetConnection(World world)
        {
            return world.GetOrCreateSystem<CreateRpcRequestSystem>().CommandTargetComponentEntity;
        }

        public static Entity CreateRpcRequest<T>(World world)
            where T : struct, IRpcCommand
        {
            return CreateRpcRequest<T>(world, default);
        }

        public static Entity CreateRpcRequest<T>(World world, T rpcCommand)
            where T : struct, IRpcCommand
        {
            if (world is null)
            {
                throw new ArgumentNullException(nameof(world));
            }

            Entity e = world.EntityManager.CreateEntity();
            world.EntityManager.AddComponentData(e, rpcCommand);
            world.EntityManager.AddComponentData(e,
                new SendRpcCommandRequestComponent { TargetConnection = GetTargetConnection(world) });
            return e;
        }

        public static Entity CreateRpcRequest<T>(EntityCommandBuffer.Concurrent buffer, int index, T rpcCommand,
            Entity targetConnection)
            where T : struct, IRpcCommand
        {
            Entity e = buffer.CreateEntity(index);
            buffer.AddComponent(index, e, rpcCommand);
            buffer.AddComponent(index, e,
                new SendRpcCommandRequestComponent { TargetConnection = targetConnection });
            return e;
        }

        protected override void OnCreate()
        {
            networkIdEntityQuery = GetEntityQuery(typeof(NetworkIdComponent));
            Enabled = false;
        }

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            throw new InvalidOperationException($"{nameof(CreateRpcRequestSystem)} should not update");
        }
    }
}