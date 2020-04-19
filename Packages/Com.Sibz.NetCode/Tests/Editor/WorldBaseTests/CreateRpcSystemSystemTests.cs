using System;
using NUnit.Framework;
using Sibz.NetCode.WorldExtensions;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.NetCode;
using Unity.Networking.Transport;
// ReSharper disable HeapView.BoxingAllocation

namespace Sibz.NetCode.Tests.Base
{
    public class CreateRpcSystemSystemTests
    {
        private World world;
        private MyCreateRpcRequestSystem system;
        private int testCount;

        [SetUp]
        public void SetUp()
        {
            world = new World($"Test{nameof(CreateRpcRequestSystem)}{testCount++}");
            system = world.CreateSystem<MyCreateRpcRequestSystem>();
        }

        [Test]
        public void WhenUpdateIsCalled_ShouldNotUpdate()
        {
            system.Update();
            Assert.IsFalse(system.DidUpdate);
        }

        [Test]
        public void WhenUpdateIfForced_ShouldThrow()
        {
            Assert.Catch<InvalidOperationException>(() => system.DoUpdate());
        }

        [Test]
        public void GetCommandTargetComponentEntity_WhenNoNetworkConnection_ShouldThrow()
        {
            Assert.Catch<InvalidOperationException>(() =>
            {
                Entity x = system.CommandTargetComponentEntity;
            });
        }

        [Test]
        public void GetCommandTargetComponentEntity_WhenNetworkConnection_GetEntity()
        {
            world.CreateSingleton<NetworkIdComponent>();
            Entity x = system.CommandTargetComponentEntity;
            Assert.IsFalse(x.Equals(Entity.Null));
        }

        [Test]
        public void CreateRpcRequest_WhenWorldIsNull_ShouldThrow()
        {
            Assert.Catch<ArgumentNullException>(() =>
                CreateRpcRequestSystem.CreateRpcRequest(null, new GoInGameRequest()));
        }

        [Test]
        public void CreateRpcRequest_WhenNoData_ShouldCreateRpc()
        {
            world.CreateSingleton<NetworkIdComponent>();
            world.CreateRpcRequest<TestRpc1>();
            Assert.AreEqual(1,
                world.EntityManager.CreateEntityQuery(typeof(TestRpc1), typeof(SendRpcCommandRequestComponent))
                    .CalculateEntityCount());
        }

        [Test]
        public void CreateRpcRequest_WithData_ShouldCreateRpcWithThatData()
        {
            world.CreateSingleton<NetworkIdComponent>();
            EntityQuery eq =
                world.EntityManager.CreateEntityQuery(typeof(TestRpc2), typeof(SendRpcCommandRequestComponent));
            world.CreateRpcRequest(new TestRpc2{ Data = 42});
            Assert.AreEqual(1, eq.CalculateEntityCount(), "Test should create entity first");
            Assert.AreEqual(42, eq.GetSingleton<TestRpc2>().Data);
        }

        [Test]
        public void CreateRpcRequest_WithMultipleConnections_ShouldTargetThatConnection()
        {
            world.CreateSingleton<NetworkIdComponent>();
            Entity target = world.CreateSingleton<NetworkIdComponent>();
            world.CreateSingleton<NetworkIdComponent>();
            EntityQuery eq =
                world.EntityManager.CreateEntityQuery(typeof(TestRpc2), typeof(SendRpcCommandRequestComponent));
            CreateRpcRequestSystem.CreateRpcRequest(world, new TestRpc2{ Data = 42}, target);
            Assert.AreEqual(1, eq.CalculateEntityCount(), "Test should create entity first");
            Assert.IsFalse(target.Equals(system.CommandTargetComponentEntity),
                "Target should not be return values of CommandTargetComponentEntity for this test");
            Assert.IsTrue(target.Equals(eq.GetSingleton<SendRpcCommandRequestComponent>().TargetConnection));
        }

        [Test]
        public void CreateRpcRequest_WithCommandBuffer_ShouldCreateRpc()
        {
            using (EntityCommandBuffer buffer = new EntityCommandBuffer(Allocator.TempJob))
            {
                Entity targetConnection = world.CreateSingleton<NetworkIdComponent>();
                buffer.CreateRpcRequest(new TestRpc1(), targetConnection);
                buffer.Playback(world.EntityManager);
            }

            Assert.AreEqual(1,
                world.EntityManager.CreateEntityQuery(typeof(TestRpc1), typeof(SendRpcCommandRequestComponent))
                    .CalculateEntityCount());
        }

        [Test]
        public void CreateRpcRequest_WithConcurrentCommandBuffer_ShouldCreateRpc()
        {
            using (EntityCommandBuffer buffer = new EntityCommandBuffer(Allocator.TempJob))
            {
                new MyTestJob()
                {
                    Buffer = buffer.ToConcurrent(),
                    TargetConnection =  world.CreateSingleton<NetworkIdComponent>()
                }.Run();
                buffer.Playback(world.EntityManager);
            }

            Assert.AreEqual(1,
                world.EntityManager.CreateEntityQuery(typeof(TestRpc1), typeof(SendRpcCommandRequestComponent))
                    .CalculateEntityCount());
        }

        private struct MyTestJob : IJob
        {
            public EntityCommandBuffer.Concurrent Buffer;
            public Entity TargetConnection;
            public void Execute()
            {
                Buffer.CreateRpcRequest(0, new TestRpc1(), TargetConnection);
            }
        }


        private struct TestRpc1 : IRpcCommand
        {
            public void Serialize(ref DataStreamWriter writer)
            {
                throw new InvalidOperationException();
            }

            public void Deserialize(ref DataStreamReader reader)
            {
                throw new InvalidOperationException();
            }

            public PortableFunctionPointer<RpcExecutor.ExecuteDelegate> CompileExecute()
            {
                throw new InvalidOperationException();
            }
        }
        private struct TestRpc2 : IRpcCommand
        {
            public int Data;
            public void Serialize(ref DataStreamWriter writer)
            {
                throw new InvalidOperationException();
            }

            public void Deserialize(ref DataStreamReader reader)
            {
                throw new InvalidOperationException();
            }

            public PortableFunctionPointer<RpcExecutor.ExecuteDelegate> CompileExecute()
            {
                throw new InvalidOperationException();
            }
        }

        private class MyCreateRpcRequestSystem : CreateRpcRequestSystem
        {
            public bool DidUpdate;

            public void DoUpdate()
            {
                OnUpdate();
            }

            protected override void OnUpdate()
            {
                DidUpdate = true;
                base.OnUpdate();
            }
        }
    }
}