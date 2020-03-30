using System;
using System.Diagnostics;
using System.Reflection;
using NUnit.Framework;
using Sibz.NetCode.Internal;
using Sibz.NetCode.Tests.Util;
using Unity.Entities;
using Unity.NetCode;

// ReSharper disable HeapView.BoxingAllocation

namespace Sibz.NetCode.Tests
{
    [TestFixture]
    public class WorldBaseInternalTests
    {
        public class CreateWorld : TestBase
        {
            [Test]
            public void ShouldCreateWorldWithName()
            {
                string name = MakeTestWorldName();
                WorldBaseInternal.CreateWorld(out World world, name, true);

                Assert.AreEqual(name, world.Name);

                world.Dispose();
            }

            [Test]
            public void WorldShouldBeInWorldDotAll()
            {
                WorldBaseInternal.CreateWorld(out World world, MakeTestWorldName(), true);

                Assert.IsTrue(World.All.Contains(world));

                world.Dispose();
            }

            [Test]
            public void ShouldCreateCorrectWorld([Values(true, false)] bool isClient)
            {
                WorldBaseInternal.CreateWorld(out World world, MakeTestWorldName(), isClient);
                Assert.IsNotNull(
                    isClient
                        ? world.GetExistingSystem<ClientSimulationSystemGroup>() as ComponentSystemGroup
                        : world.GetExistingSystem<ServerSimulationSystemGroup>()
                );
            }
        }

        public class ImportSystems : TestBase
        {
            private ImportMethodsTest importMethods = new ImportMethodsTest();

            [Test]
            public void WhenWorldIsNull_ShouldThrowArgumentNullException()
            {
                Assert.Catch<ArgumentNullException>(
                    () => WorldBaseInternal.ImportSystems(null, new Type[0], null, true));
            }

            [Test]
            public void WhenBothEnumerableParamsAreNull_ShouldThrowArgumentException()
            {
                Assert.Catch<ArgumentException>(() =>
                    WorldBaseInternal.ImportSystems(World, null, null, true));
            }

            [Test]
            public void WhenSystemsNull_ShouldImportWithAttributes()
            {
                WorldBaseInternal.ImportSystems(World, new Type[0], null, true, importMethods);
                Assert.AreEqual(ImportMethodsTest.CalledMethod.ImportSystemsWithAttributes, importMethods.Called);
            }

            [Test]
            public void WhenSystemsNull_ShouldImportSystemsFromList()
            {
                WorldBaseInternal.ImportSystems(World, null, new Type[0], true, importMethods);
                Assert.AreEqual(ImportMethodsTest.CalledMethod.ImportSystemsFromList, importMethods.Called);
            }
        }
    }
}