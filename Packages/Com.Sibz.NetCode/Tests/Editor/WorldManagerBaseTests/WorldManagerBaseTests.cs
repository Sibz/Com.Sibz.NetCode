using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using NUnit.Framework;
using Unity.Entities;
using UnityEngine;
using UnityEngine.TestTools;

namespace Sibz.NetCode.Tests.Base
{
    public class WorldManagerBaseTests
    {
        /*
        private MyWorldManagerOptions options;
        private MyWorldManager wm;
        private MyCallbackProvider cbp;

        [SetUp]
        public void SetUp()
        {
            options = MyWorldManagerOptions.Defaults;
            cbp = new MyCallbackProvider();
            wm = new MyWorldManager(options, cbp);
        }

        [TearDown]
        public void TearDown()
        {
            //wm.Dispose();
        }

        [Test]
        public void WhenImportPrefabsIsNull_ShouldLogWarning()
        {
            wm = new MyWorldManager(new MyWorldManagerOptions() { GhostCollectionPrefabs = null });
            wm.ImportPrefabs();
            LogAssert.Expect(LogType.Warning, new Regex(".*"));
        }

        [Test]
        public void ShouldSetCreatWorldOnInstantiateProperty()
        {
            options.CreateWorldOnInstantiate = true;
            wm = new MyWorldManager(options);
            Assert.IsTrue(wm.Options.CreateWorldOnInstantiate);
        }

        [Test]
        public void ShouldCallbackOnCreation()
        {
            var calledBack = false;
            cbp.WorldCreated += () => calledBack = true;
            wm.CreateWorld(new List<Type>());
            Assert.IsTrue(calledBack);
        }

        [Test]
        public void ShouldCallBackPreWorldDestroy()
        {
            var calledBack = false;
            cbp.PreWorldDestroy += () =>
            {
                calledBack = true;
                Assert.IsTrue(wm.WorldIsCreated);
            };
            wm.CreateWorld(new List<Type>());
            wm.DestroyWorld();
            Assert.IsTrue(calledBack);
        }

        [Test]
        public void ShouldCallBackPostWorldDestroy()
        {
            var calledBack = false;
            cbp.WorldDestroyed += () =>
            {
                calledBack = true;
                Assert.IsFalse(wm.WorldIsCreated);
            };
            wm.CreateWorld(new List<Type>());
            wm.DestroyWorld();
            Assert.IsTrue(calledBack);
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
            var wm = new MyWorldManager(options, cbp);
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

        public class MyCallbackProvider : IWorldCallbackProvider
        {
            public Action WorldCreated { get; set; }
            public Action WorldDestroyed { get; set; }
            public Action PreWorldDestroy { get; set; }
        }

        public class MySystem : ComponentSystem
        {
            protected override void OnUpdate()
            {
            }
        }
    */
    }
}