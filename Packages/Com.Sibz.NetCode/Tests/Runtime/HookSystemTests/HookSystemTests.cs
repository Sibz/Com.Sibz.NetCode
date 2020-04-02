using System;
using System.Collections.Generic;
using NUnit.Framework;
using Sibz.EntityEvents;
using Unity.Entities;

namespace Sibz.NetCode.Tests
{
    public class HookSystemTests : TestBase
    {
        [Test]
        public void ShouldCallActionWhenEventExists()
        {
            bool actionCalled = false;

            void OnAction(IEventComponentData test)
            {
                actionCalled = true;
            }

            World.EntityManager.CreateEntity(typeof(TestEvent));
            var system = World.CreateSystem<HookSystem>();
            system.RegisterHook<TestEvent>(OnAction);
            system.Update();
            Assert.IsTrue(actionCalled);
        }

        [Test]
        public void ShouldCallActionWithData()
        {
            int index = 0;

            void OnAction(IEventComponentData test)
            {
                index = ((TestEventWithData) test).Index;
            }

            var e = World.EntityManager.CreateEntity(typeof(TestEventWithData));
            World.EntityManager.SetComponentData(e, new TestEventWithData {Index = 42});
            var system = World.CreateSystem<HookSystem>();
            system.RegisterHook<TestEventWithData>(OnAction);
            system.Update();
            Assert.AreEqual(42, index);
        }

        [Test]
        public void ShouldNotCallAfterDeregisterHook()
        {
            bool actionCalled = false;

            void OnAction(IEventComponentData test)
            {
                actionCalled = true;
            }

            World.EntityManager.CreateEntity(typeof(TestEvent));
            var system = World.CreateSystem<HookSystem>();
            system.RegisterHook<TestEvent>(OnAction);
            system.UnregisterHook<TestEvent>();
            system.Update();
            Assert.IsFalse(actionCalled);
        }

        [Test]
        public void ShouldCallMultipleActions()
        {
            int index = 0;
            bool actionCalled = false;
            var system = World.CreateSystem<HookSystem>();

            void OnAction1(IEventComponentData test)
            {
                actionCalled = true;
            }

            void OnAction2(IEventComponentData test)
            {
                index = ((TestEventWithData) test).Index;
            }

            World.EntityManager.CreateEntity(typeof(TestEvent));


            var e = World.EntityManager.CreateEntity(typeof(TestEventWithData));
            World.EntityManager.SetComponentData(e, new TestEventWithData {Index = 42});

            system.RegisterHooks(new Dictionary<ComponentType, Action<IEventComponentData>>()
            {
                {typeof(TestEvent), OnAction1},
                {typeof(TestEventWithData), OnAction2},
            });

            system.Update();
            system.Update();

            Assert.IsTrue(actionCalled);
            Assert.AreEqual(42, index);
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