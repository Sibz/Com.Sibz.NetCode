using Sibz.CommandBufferHelpers;
using Sibz.EntityEvents;
using Sibz.WorldSystemHelpers;
using Unity.Entities;
using Unity.NetCode;

namespace Sibz.NetCode
{
    public abstract partial class WorldBase<TDefaultSystemGroup> : IWorldBase
        where TDefaultSystemGroup : ComponentSystemGroup
    {
        protected class WorldManagerClass : IWorldManager
        {
            private WorldBase<TDefaultSystemGroup> worldBase;

            private World World
            {
                get => worldBase.World;
                set => worldBase.World = value;
            }

            private IWorldOptionsBase Options => worldBase.Options;
            public WorldManagerClass(WorldBase<TDefaultSystemGroup> worldBase)
            {
                this.worldBase = worldBase;
            }

            public void CreateWorld()
            {
                World = worldBase.CreationMethod.Invoke(World.DefaultGameObjectInjectionWorld, Options.WorldName);

                World.ImportSystemsFromList<TDefaultSystemGroup>(worldBase.Systems);

                //worldBase.CommandBuffer = new BeginInitCommandBuffer(World);

                Options.SharedDataPrefabs.Instantiate();

                worldBase.NetworkStreamReceiveSystem = World.GetExistingSystem<NetworkStreamReceiveSystem>();

                worldBase.HookSystem = World.GetExistingSystem<NetCodeHookSystem>();

                World.EnqueueEvent<WorldCreatedEvent>();
            }

            public void DestroyWorld()
            {
                if (World.IsCreated)
                {
                    World.Dispose();
                }
            }
        }
    }
}