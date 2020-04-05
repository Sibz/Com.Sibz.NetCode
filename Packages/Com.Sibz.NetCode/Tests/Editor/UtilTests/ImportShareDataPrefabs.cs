using System;
using System.Collections.Generic;
using NUnit.Framework;
using Unity.Collections;
using Unity.Entities;
using Unity.NetCode;
using UnityEngine;

namespace Sibz.NetCode.Tests.UtilTests
{
    public class ImportShareDataPrefabs
    {
        private World testWorld;

        [SetUp]
        public void SetUp() => testWorld = ClientServerBootstrap.CreateClientWorld(World.DefaultGameObjectInjectionWorld, "TestWorld");

        [TearDown]
        public void TearDown() => testWorld.Dispose();

        [Test]
        public void WhenWorldIsNull_ShouldThrow() =>
            Assert.Catch<ArgumentNullException>(() => Util.ImportGhostCollections(null, new List<GameObject>()));

        [Test]
        public void WhenPrefabsIsNull_ShouldThrow() =>
            Assert.Catch<ArgumentNullException>(() => testWorld.ImportGhostCollections(null));

        [Test]
        public void ShouldImportPrefabIntoWorld()
        {
            testWorld.ImportGhostCollections(new List<GameObject> { Resources.Load<GameObject>("NetCodeTestCollection")});
            NativeArray<Entity> entities = testWorld.EntityManager.GetAllEntities(Allocator.TempJob);
            for (var i = 0; i < entities.Length; i++)
            {
                Debug.Log(testWorld.EntityManager.GetName(entities[i]) + " prefab:" + testWorld.EntityManager.HasComponent<Prefab>(entities[i]));

            }
            int len = entities.Length;
            entities.Dispose();

            Assert.AreEqual(5, len);

        }
    }
}