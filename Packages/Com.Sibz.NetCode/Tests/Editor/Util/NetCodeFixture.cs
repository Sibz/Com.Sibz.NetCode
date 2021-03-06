﻿using NUnit.Framework;
using Unity.Entities;
using Unity.NetCode;

namespace Sibz.NetCode.Tests
{
    [SetUpFixture]
    public class NetCodeFixture
    {
        public static World DefaultWorld;

        public static void SetUpDefaultWorld()
        {
            if (!(DefaultWorld is null))
            {
                return;
            }

            foreach (World world in World.All)
            {
                if (world.Name != "DefaultNetWorld")
                {
                    continue;
                }

                DefaultWorld = world;
                break;
            }

            if (!(DefaultWorld is null))
            {
                return;
            }

            new ClientServerBootstrap().Initialize("DefaultNetWorld");
            foreach (World world in World.All)
            {
                if (world.Name != "DefaultNetWorld")
                {
                    continue;
                }

                DefaultWorld = world;
                break;
            }
        }

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            SetUpDefaultWorld();
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            for (int i = World.All.Count - 1; i >= 0; i--)
            {
                if ((World.All[i].Name.StartsWith("Test") || World.All[i].Name == "DefaultNetnonoWorld"
                                                          || World.All[i].Name.StartsWith("Client")
                                                          || World.All[i].Name.StartsWith("Server"))
                    && (World.All[i] is World world && world.IsCreated))
                {
                    world.Dispose();
                }
            }
        }
    }
}