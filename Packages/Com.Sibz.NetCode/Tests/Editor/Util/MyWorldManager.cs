﻿using Sibz.EntityEvents;
using Unity.Entities;
using Unity.NetCode;

namespace Sibz.NetCode.Tests
{
    public class MyWorldCreator : WorldCreatorBase
    {
        public bool CalledBootStrapCreateWorld;
        public bool CalledImportPrefabs;

        public MyWorldCreator(IWorldCreatorOptions options) : base(
            options)
        {
        }

        protected override World BootStrapCreateWorld(string name)
        {
            CalledBootStrapCreateWorld = true;
            return ClientServerBootstrap.CreateClientWorld(NetCodeFixture.DefaultWorld, name);
        }

        public override void ImportPrefabs()
        {
            CalledImportPrefabs = true;
            base.ImportPrefabs();
        }
    }
}