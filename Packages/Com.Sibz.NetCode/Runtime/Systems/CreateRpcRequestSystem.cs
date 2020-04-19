using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.NetCode;

namespace Sibz.NetCode
{
    [ClientAndServerSystem]
    public class CreateRpcRequestSystem : SystemBase
    {
        private EntityQuery networkIdEntityQuery;

        public Entity CommandTargetComponentEntity
        {
            get
            {
                if (networkIdEntityQuery.CalculateEntityCount() == 0)
                {
                    throw new InvalidOperationException("No active network connection for Rpc request");
                }

                Entity result;
                using (NativeArray<Entity> entities =
                    networkIdEntityQuery.ToEntityArrayAsync(Allocator.TempJob, out JobHandle jh))
                {
                    jh.Complete();
                    result = entities[0];
                }

                return result;
            }
        }

        private static Entity GetTargetConnection(World world)
        {
            return world.GetOrCreateSystem<CreateRpcRequestSystem>().CommandTargetComponentEntity;
        }

        public static Entity CreateRpcRequest<T>(World world, Entity targetConnection = default)
            where T : struct, IRpcCommand
        {
            return CreateRpcRequest<T>(world, default, targetConnection);
        }

        public static Entity CreateRpcRequest<T>(World world, T rpcCommand, Entity targetConnection = default)
            where T : struct, IRpcCommand
        {
            if (world is null)
            {
                throw new ArgumentNullException(nameof(world));
            }

            Entity e = world.EntityManager.CreateEntity();
            world.EntityManager.AddComponentData(e, rpcCommand);
            world.EntityManager.AddComponentData(e,
                new SendRpcCommandRequestComponent
                {
                    TargetConnection = targetConnection.Equals(Entity.Null)
                        ? GetTargetConnection(world)
                        : targetConnection
                });
            return e;
        }

        public static Entity CreateRpcRequest<T>(EntityCommandBuffer buffer, T rpcCommand,
            Entity targetConnection)
            where T : struct, IRpcCommand
        {
            Entity e = buffer.CreateEntity();
            buffer.AddComponent(e, rpcCommand);
            buffer.AddComponent(e,
                new SendRpcCommandRequestComponent { TargetConnection = targetConnection });
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

        protected override void OnUpdate()
        {
            throw new InvalidOperationException($"{nameof(CreateRpcRequestSystem)} should not update");
        }
    }
}