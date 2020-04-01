using System;
using NUnit.Framework;
using Sibz.CommandBufferHelpers;
using Unity.Entities;
using UnityEngine;

// ReSharper disable HeapView.BoxingAllocation

namespace Sibz.EntityEvents.Tests
{
    public class EventComponentSystemTests
    {
        private static World TestWorld { get; set; }

        private static EntityCommandBufferSystem BufferSystem =>
            TestWorld.GetExistingSystem<BeginInitializationEntityCommandBufferSystem>();

        private static EventComponentSystem EventComponentSystem =>
            TestWorld.GetExistingSystem<EventComponentSystem>();

        private static Entity GetSingletonEntity<T>()
            where T : struct, IComponentData
            => GetSingletonQuery<T>().GetSingletonEntity();

        public static T GetSingleton<T>()
            where T : struct, IComponentData
            => GetSingletonQuery<T>().GetSingleton<T>();

        private static EntityQuery GetSingletonQuery<T>()
            where T : struct, IComponentData => TestWorld.EntityManager.CreateEntityQuery(typeof(T));

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            DefaultWorldInitialization.Initialize("Default", true);
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            //  World.DefaultGameObjectInjectionWorld.Dispose();
        }

        [SetUp]
        public void SetUp()
        {
            TestWorld = new World("EntityEventsTestWorld");
            DefaultWorldInitialization.AddSystemsToRootLevelSystemGroups(TestWorld,
                new[] {typeof(BeginInitializationEntityCommandBufferSystem)});
            TestWorld.CreateSystem<EventComponentSystem>();
        }

        [TearDown]
        public void TearDown()
        {
            TestWorld.Dispose();
        }

        [Test]
        public void WhenNoBeginInitBufferSystem_ShouldThrow()
        {
            TestWorld.DestroySystem(EventComponentSystem);
            TestWorld.DestroySystem(BufferSystem);
            Assert.Catch(() => TestWorld.CreateSystem<EventComponentSystem>());
        }

        [Test]
        public void ShouldCreateEventEntity()
        {
            TestWorld.EnqueueEvent<TestEvent>();
            EventComponentSystem.Update();
            BufferSystem.Update();
            Assert.AreEqual(1, GetSingletonQuery<TestEvent>().CalculateEntityCount());
        }

        [Test]
        public void EventEntity_ShouldLastOnlyOneFrame()
        {
            TestWorld.EnqueueEvent<TestEvent>();
            EventComponentSystem.Update();
            BufferSystem.Update();
            Assert.AreEqual(1, GetSingletonQuery<TestEvent>().CalculateEntityCount());
            EventComponentSystem.Update();
            BufferSystem.Update();
            Assert.AreEqual(0, GetSingletonQuery<TestEvent>().CalculateEntityCount());
        }

        [Test]
        public void WorldExtension_WhenSystemDoesNotExist_ShowThrowNullReferenceException()
        {
            TestWorld.DestroySystem(EventComponentSystem);
            Assert.Catch<NullReferenceException>(() => TestWorld.EnqueueEvent<TestEvent>());
        }

        [Test]
        public void EventTypes_ShouldContainDefinedEvents()
        {
            Assert.Contains((ComponentType) typeof(TestEvent), EventComponentSystem.EventTypes);
        }

        [Test]
        public void ShouldBeAbleToQueueMultipleSameEvents()
        {
            TestWorld.EnqueueEvent<TestEvent>();
            TestWorld.EnqueueEvent<TestEvent>();
            EventComponentSystem.Update();
            BufferSystem.Update();
            Assert.AreEqual(2, GetSingletonQuery<TestEvent>().CalculateEntityCount());
        }

        [Test]
        public void ShouldBeAbleToEnqueueEventWithData()
        {
            TestWorld.EnqueueEvent<TestEventWithData>();
            EventComponentSystem.Update();
            BufferSystem.Update();
            Assert.AreEqual(1, GetSingletonQuery<TestEventWithData>().CalculateEntityCount());
        }

        [Test]
        public void WhenEnqueuingWithData_ShouldHaveCorrectData ()
        {
            TestWorld.EnqueueEvent(new TestEventWithData{ Index = 5});
            EventComponentSystem.Update();
            BufferSystem.Update();
            Assert.AreEqual(5, GetSingletonQuery<TestEventWithData>().GetSingleton<TestEventWithData>().Index);
        }

        public struct TestEvent : IEventComponentData
        {
        }

        public struct TestEventWithData : IEventComponentData
        {
            public int Index;
        }
    }
}