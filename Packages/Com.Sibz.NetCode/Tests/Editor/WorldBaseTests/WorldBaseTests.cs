using System;
using NUnit.Framework;
using Sibz.EntityEvents;
using Unity.Entities;

namespace Sibz.NetCode.Tests.Base
{
    public class WorldBaseTests
    {
        private Func<World, string, World> creationMethod;
        private MyWorldBaseImpl current;
        private MyWorldOptions worldOptions;

        [SetUp]
        public void SetUp()
        {
            worldOptions = MyWorldOptions.Defaults;
        }

        [Test]
        public void ShouldBindCallbacksToWorldManager()
        {
            int calledCount = 0;
            MyWorldCreator wm = new MyWorldCreator(worldOptions);
            current = new MyWorldBaseImpl(worldOptions, wm);
            wm.CreateWorld();
            current.WorldCreated += () => calledCount++;
            current.WorldDestroyed += () => calledCount++;
            current.PreWorldDestroy += () => calledCount++;
            wm.InvokeAllCallbacks();
            Assert.AreEqual(1, calledCount);
        }

        [Test]
        public void ShouldHaveCorrectSystems()
        {
            MyWorldCreator wm = new MyWorldCreator(worldOptions);
            current = new MyWorldBaseImpl(worldOptions, wm);
            wm.CreateWorld();
            Assert.IsNotNull(current.World.GetExistingSystem<EventComponentSystem>());
        }

        [Test]
        public void WhenWorldManagerIsNull_ShouldThrow()
        {
            Assert.Catch<ArgumentNullException>(() => new MyWorldBaseImpl(worldOptions, null));
        }

        [Test]
        public void WhenOptionsIsNull_ShouldTrow()
        {
            Assert.Catch<ArgumentNullException>(() => new MyWorldBaseImpl(null, new MyWorldCreator(worldOptions)));
        }

        [Test]
        public void WhenOptionSpecified_ShouldCreateWorldFromConstructor()
        {
            worldOptions.CreateWorldOnInstantiate = true;
            MyWorldCreator worldManager = new MyWorldCreator(worldOptions);
            current = new MyWorldBaseImpl(worldOptions, worldManager);
            Assert.IsTrue(worldManager.CalledBootStrapCreateWorld);
        }

        [Test]
        public void WhenOptionNotSpecified_ShouldNotCreateWorldFromConstructor()
        {
            worldOptions.CreateWorldOnInstantiate = false;
            MyWorldCreator worldManager = new MyWorldCreator(worldOptions);
            current = new MyWorldBaseImpl(worldOptions, worldManager);
            Assert.IsFalse(worldManager.CalledBootStrapCreateWorld);
        }

        [Test]
        public void DestroyWorld_ShouldCreateSingleton()
        {
            worldOptions.CreateWorldOnInstantiate = true;
            MyWorldCreator worldManager = new MyWorldCreator(worldOptions);
            current = new MyWorldBaseImpl(worldOptions, worldManager);
            current.DestroyWorld();
            Assert.AreEqual(1,
                current.World.EntityManager.CreateEntityQuery(typeof(DestroyWorld)).CalculateEntityCount());
        }
    }

    public class MyWorldBaseImpl : WorldBase
    {
        public MyWorldBaseImpl(IWorldOptions options, IWorldCreator worldCreator) : base(options, worldCreator)
        {
        }
    }
}