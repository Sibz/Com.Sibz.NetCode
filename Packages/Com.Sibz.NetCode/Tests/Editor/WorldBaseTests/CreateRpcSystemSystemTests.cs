using System;
using NUnit.Framework;
using Sibz.NetCode.WorldExtensions;
using Unity.Entities;
using Unity.Jobs;
using Unity.NetCode;

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


        private class MyCreateRpcRequestSystem : CreateRpcRequestSystem
        {
            public bool DidUpdate;

            public void DoUpdate()
            {
                OnUpdate(default);
            }

            protected override JobHandle OnUpdate(JobHandle inputDeps)
            {
                DidUpdate = true;
                return base.OnUpdate(inputDeps);
            }
        }
    }
}