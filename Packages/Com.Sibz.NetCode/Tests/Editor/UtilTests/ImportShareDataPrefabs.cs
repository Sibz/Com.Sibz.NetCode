using System;
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
        public void TearDown()
        {
            testWorld.Dispose();
        }

        [Test]
        public void WhenWorldIsNull_ShouldThrow()
        {
            Assert.Catch<ArgumentNullException>(() =>
                ImportGhostCollectionWorldExtension.ImportGhostCollection(null,
                    Resources.Load<GameObject>("NetCodeTestCollection")));
        }

        [Test]
        public void WhenPrefabIsNull_ShouldThrow()
        {
            Assert.Catch<ArgumentNullException>(() =>
                testWorld.ImportGhostCollection(null));
        }

        [Test]
        public void WhenPrefabDoesNotHaveGhostAuthComponent_ShouldThrow()
        {
            Assert.Catch<ArgumentException>(() =>
                testWorld.ImportGhostCollection(new GameObject()));
        }

        [Test]
        public void ShouldImportPrefabIntoWorld()
        {
            testWorld.ImportGhostCollection(Resources.Load<GameObject>("NetCodeTestCollection"));
            NativeArray<Entity> entities = testWorld.EntityManager.GetAllEntities(Allocator.TempJob);
            for (int i = 0; i < entities.Length; i++)
            {
                Debug.Log(testWorld.EntityManager.GetName(entities[i]) + " prefab:" +
                          testWorld.EntityManager.HasComponent<Prefab>(entities[i]));
            }

            int len = entities.Length;
            entities.Dispose();

            Assert.AreEqual(5, len);
        }
    }
}