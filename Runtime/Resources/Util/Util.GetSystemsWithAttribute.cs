﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Sibz
{
    public static partial class Util
    {
        public static List<Type> GetSystemsWithAttribute<T>(List<Type> systemTypes = null)
            where T:Attribute
        {
            systemTypes = systemTypes ?? new List<Type>();
            systemTypes.AddRange(Assembly.GetAssembly(typeof(T)).GetTypes()
                .Where(x => !(x.GetCustomAttribute<T>() is null)));
            return systemTypes;
        }
    }
}