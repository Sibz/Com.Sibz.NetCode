﻿using System;
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
        private MyWorldOptions options;
        private MyWorldCreator wm;

        [SetUp]
        public void SetUp()
        {
            options = MyWorldOptions.Defaults;
            wm = new MyWorldCreator(options);
        }

        [TearDown]
        public void TearDown()
        {
            //wm.Dispose();
        }

        [Test]
        public void WhenImportPrefabsIsNull_ShouldLogWarning()
        {
            wm = new MyWorldCreator(new MyWorldOptions() { GhostCollectionPrefabs = null });
            wm.ImportPrefabs();
            LogAssert.Expect(LogType.Warning, new Regex(".*"));
        }

        [Test]
        public void ShouldCallbackOnCreation()
        {
            var calledBack = false;
            wm.WorldCreated += () => calledBack = true;
            wm.CreateWorld();
            Assert.IsTrue(calledBack);
        }

        [Test]
        public void ShouldCallCalledBootStrapCreateWorld()
        {
            wm.CreateWorld();
            Assert.IsTrue(wm.CalledBootStrapCreateWorld);
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