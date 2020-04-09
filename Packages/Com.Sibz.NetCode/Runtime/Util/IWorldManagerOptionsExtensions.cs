using System;
using Sibz.NetCode;

namespace Sibz
{
    public static class WorldManagerOptionsExtensions
    {
        public static IWorldManagerOptions GetOptionsWithImportedSystems<T>(this IWorldManagerOptions options)
        where T: Attribute
        {
            options.Systems.AppendTypesWithAttribute<T>();
            return options;
        }
    }
}