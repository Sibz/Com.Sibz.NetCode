﻿using Sibz.NetCode;
using Unity.Entities;
using Unity.NetCode;

namespace Sibz
{
    public static partial class Util
    {
        public static Entity CreateRpcRequest<T>(this World world, T data)
            where T : struct, IRpcCommand =>
            CreateRpcRequestSystem.CreateRpcRequest(world, data);

        public static Entity CreateRpcRequest<T>(this World world)
            where T : struct, IRpcCommand =>
            CreateRpcRequestSystem.CreateRpcRequest<T>(world);



    }
}