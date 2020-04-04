using System;
using System.Collections.Generic;
using NUnit.Framework;
using Unity.Entities;

namespace Sibz.NetCode.Tests
{
    public class WorldManagerBaseTests
    {
        private MyWorldManagerOptions options;
        private MyWorldManager wm;

        [SetUp]
        public void SetUp()
        {
            options = MyWorldManagerOptions.Defaults;
            wm = new MyWorldManager(options);
        }

        [TearDown]
        public void TearDown()
        {
            //wm.Dispose();
        }

        [Test]
        public void ShouldSetCreatWorldOnInstantiateProperty()
        {
            options.CreateWorldOnInstantiate = true;
            wm = new MyWorldManager(options);
            Assert.IsTrue(wm.Options.CreateWorldOnInstantiate);
        }

        [Test]
        public void InstantiateProperty_ShouldDefaultToFalse() => Assert.IsFalse(wm.Options.CreateWorldOnInstantiate);

        [Test]
        public void ShouldCallCalledBootStrapCreateWorld()
        {
            wm.CreateWorld(new List<Type>());
            Assert.IsTrue(wm.CalledBootStrapCreateWorld);
        }

        [Test]
        public void CreateWorld_WhenSystemsIsNull_ShouldThrowError() =>
            Assert.Catch<ArgumentNullException>(() => wm.CreateWorld(null));

        [Test]
        public void CreateWorld_WithItemsInList_ShouldAddSystems()
        {
            var wm = new MyWorldManager(options);
            wm.CreateWorld(new List<Type> { typeof(MySystem) });
            Assert.IsNotNull(wm.World.GetExistingSystem<MySystem>());
        }

        [Test]
        public void ShouldCall_ImportPrefabs()
        {
        }

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
        }

        public class MySystem : ComponentSystem
        {
            protected override void OnUpdate()
            {
            }
        }
    }
}