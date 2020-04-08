/*using System;
using System.Collections.Generic;
using NUnit.Framework;
using Sibz.NetCode.WorldExtensions;
using Unity.Collections;
using Unity.Entities;
using Unity.NetCode;
using UnityEngine;

namespace Sibz.NetCode.Tests.UtilTests
{
    public class ImportShareDataPrefabs
    {
        private World testWorld;

        private GameObject gameObject;

        [SetUp]
        public void SetUp()
        {
            testWorld = ClientServerBootstrap.CreateClientWorld(World.DefaultGameObjectInjectionWorld, "TestWorld");
        }

        [TearDown]
        public void TearDown() => testWorld.Dispose();

        [Test]
        public void WhenWorldIsNull_ShouldThrow() =>
            Assert.Catch<ArgumentNullException>(() =>
                ImportGhostCollectionWorldExtension.ImportGhostCollection(null, new List<GameObject>(){ Resources.Load<GameObject>("NetCodeTestCollection")  } ));

        [Test]
        public void WhenPrefabInListIsNull_ShouldThrow() =>
            Assert.Catch<ArgumentNullException>(() =>
                ImportGhostCollectionWorldExtension.ImportGhostCollection(testWorld, new List<GameObject>(){ null } ));

        [Test]
        public void WhenPrefabDoesNotHaveGhostAuthComponent_ShouldThrow()
        {
             Assert.Catch<ArgumentException>(() =>
                            ImportGhostCollectionWorldExtension.ImportGhostCollection(testWorld, new List<GameObject>(){ new GameObject() } ));
        }

        [Test]
        public void ShouldImportPrefabIntoWorld()
        {
            testWorld.ImportGhostCollection(new List<GameObject>
                { Resources.Load<GameObject>("NetCodeTestCollection") });
            NativeArray<Entity> entities = testWorld.EntityManager.GetAllEntities(Allocator.TempJob);
            for (var i = 0; i < entities.Length; i++)
            {
                Debug.Log(testWorld.EntityManager.GetName(entities[i]) + " prefab:" +
                          testWorld.EntityManager.HasComponent<Prefab>(entities[i]));
            }

            int len = entities.Length;
            entities.Dispose();

            Assert.AreEqual(5, len);
        }

        // ReSharper disable ExpressionIsAlwaysNull
        [Test]
        public void WhenPrefabsIsNull_ShouldThrow()
        {
            List<GameObject> prefabs = null;
            Assert.Catch<ArgumentNullException>(() => testWorld.ImportGhostCollection(prefabs));
        }

        [Test]
        public void WhenPrefabIsNull_ShouldThrow()
        {
            GameObject prefab = null;
            Assert.Catch<ArgumentNullException>(() => testWorld.ImportGhostCollection(prefab));
        }
        // ReSharper restore ExpressionIsAlwaysNull
    }
}*/