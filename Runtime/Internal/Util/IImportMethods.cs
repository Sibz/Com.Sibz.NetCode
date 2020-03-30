using System;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

namespace Sibz.NetCode.Internal.Util
{
    public interface IImportMethods
    {
        void ImportSystemsFromList(World world, IEnumerable<Type> systems, bool isClient);
    }
}