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
        private static IImportMethods importMethods = TestHelpers.GetInstanceOf<IImportMethods>("ImportMethods", "Sibz.NetCode.Internal.Util");

        public class GetDefaultGroupType
        {
            [Test]
            public void ShouldGetClientGroup()
            {
                Assert.AreEqual(typeof(ClientSimulationSystemGroup), importMethods.GetDefaultGroupType(true));
            }
            [Test]
            public void ShouldGetServerGroup()
            {
                Assert.AreEqual(typeof(ServerSimulationSystemGroup), importMethods.GetDefaultGroupType(false));
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

        public class ImportSystems : TestBase
        {
            private ImportMethodsTest imTest = new ImportMethodsTest();

            [Test]
            public void WhenWorldIsNull_ShouldThrowArgumentNullException()
            {
                Assert.Catch<ArgumentNullException>(()=>
                    importMethods.ImportSystems(null, new Type[0], true));
            }

            [Test]
            public void WhenSystemsIsNull_ShouldThrowArgumentNullException()
            {
                Assert.Catch<ArgumentException>(() =>
                    importMethods.ImportSystems(World, null, true));
            }

            [Test]
            public void ShouldCallImportSystemsFromList()
            {
                importMethods.ImportSystems(World, new Type[0], true, imTest);
                Assert.AreEqual(ImportMethodsTest.CalledMethod.ImportSystemsFromList, imTest.Called);
            }
        }
    }
}