using Unity.Entities;

namespace Sibz.NetCode.WorldExtensions
{
    public static class CreateSingletonWorldExtension
    {
        public static Entity CreateSingleton<T>(this World world, T data)
            where T : struct, IComponentData
        {
            var entity = world.EntityManager.CreateEntity();
            world.EntityManager.AddComponentData(entity, data);
            return entity;
        }

        public static Entity CreateSingleton<T>(this World world)
            where T : struct, IComponentData =>
            world.EntityManager.CreateEntity(typeof(T));
    }
}