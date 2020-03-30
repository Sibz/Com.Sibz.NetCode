using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NUnit.Framework;
using Sibz.NetCode.Internal.Util;
using Sibz.NetCode.Tests.Util;
using Unity.Entities;
using Unity.NetCode;
using UnityEngine;
using Object = UnityEngine.Object;

// ReSharper disable HeapView.BoxingAllocation

namespace Sibz.NetCode.Tests
{
    [TestFixture]
    public class ImportMethodsTests
    {
        private static ImportMethods importMethods = new ImportMethods();

        public class GetDefaultGroupType
        {
            [Test]
            public void ShouldGetClientGroup()
            {
                Assert.AreEqual(typeof(ClientSimulationSystemGroup), ImportMethods.GetDefaultGroupType(true));
            }
            [Test]
            public void ShouldGetServerGroup()
            {
                Assert.AreEqual(typeof(ServerSimulationSystemGroup), ImportMethods.GetDefaultGroupType(false));
            }
        }

        public class ImportSharedDataPrefabs
        {
            [Test]
            public void WhenSharedDataPrefabsIsNull_ShouldThrowArgumentNullException()
            {
                Assert.Catch<ArgumentNullException>(() => importMethods.ImportSharedDataPrefabs(null));
            }

            [Test]
            public void ShouldInstantiateObjects()
            {
                const string name = "Test";
                importMethods.ImportSharedDataPrefabs(new[] {new GameObject(name), new GameObject(name)});
                Assert.AreEqual(2, Object.FindObjectsOfType<GameObject>().Count(x => x.name == name));
            }
        }

        public class ImportSystemsWithAttribute : TestBase
        {
            public void CallClientSimUpdate() => World.GetExistingSystem<ClientSimulationSystemGroup>().Update();
            public void CallServerSimUpdate() => World.GetExistingSystem<ServerSimulationSystemGroup>().Update();
            public void CallTestGroupUpdate() => World.GetExistingSystem<TestGroup>().Update();

            [SetUp]
            public void SetUp_ImportSystemsWithAttribute()
            {
                World.CreateSystem<ClientSimulationSystemGroup>();
                World.CreateSystem<ServerSimulationSystemGroup>();
                World.CreateSystem<TestGroup>();
            }

            [Test]
            public void WhenWorldIsNull_ShouldThrowArgumentNullException()
            {
                Assert.Catch<ArgumentNullException>(() =>
                    importMethods.ImportSystemsWithAttributes(null, new Type[0], true));
            }

            [Test]
            public void WhenAttributesIsNull_ShouldThrowArgumentNullException()
            {
                Assert.Catch<ArgumentNullException>(() => importMethods.ImportSystemsWithAttributes(World, null, true));
            }

            [Test]
            public void WhenIsClientTrue_ShouldCreateInClientSimGroup()
            {
                importMethods.ImportSystemsWithAttributes(World, new[] {typeof(TestIncAttribute)}, true);
                CallClientSimUpdate();
                Assert.IsTrue(SystemInList(World.GetExistingSystem<TestSystem>(), World.GetExistingSystem<ClientSimulationSystemGroup>()));
            }

            [Test]
            public void WhenIsClientFalse_ShouldCreateInServerSimGroup()
            {
                importMethods.ImportSystemsWithAttributes(World, new[] {typeof(TestIncAttribute)}, false);
                CallServerSimUpdate();
                Assert.IsTrue(SystemInList(World.GetExistingSystem<TestSystem>(), World.GetExistingSystem<ServerSimulationSystemGroup>()));
            }

            public bool SystemInList(ComponentSystemBase system, ComponentSystemGroup group)
            {
                var field = typeof(ComponentSystemGroup)
                    .GetField("m_systemsToUpdate", BindingFlags.NonPublic | BindingFlags.Instance);
                if (field is null)
                {
                    throw new Exception("Field was null");
                }
                var value = field.GetValue(group) as List<ComponentSystemBase>;
                if (value is null)
                {
                    throw new Exception("Value was null");
                }

                return value.Contains(system);
            }

            [Test]
            public void WhenSystemHasUpdateInGroupAttribute_ShouldCreateInSpecifiedGroup()
            {
                importMethods.ImportSystemsWithAttributes(World, new[] {typeof(TestIncAttribute)}, true);
                CallTestGroupUpdate();
                Assert.IsTrue(World.GetExistingSystem<TestSystem2>().Called);
            }

            public class TestIncAttribute : Attribute
            {
            }

            [TestInc]
            public class TestSystem : ComponentSystem
            {
                public bool Called;

                protected override void OnUpdate()
                {
                    Called = true;
                }
            }

            [TestInc]
            [UpdateInGroup(typeof(TestGroup))]
            public class TestSystem2 : ComponentSystem
            {
                public bool Called;

                protected override void OnUpdate()
                {
                    Called = true;
                }
            }

            public class TestGroup : ComponentSystemGroup
            {
            }
        }
    }
}