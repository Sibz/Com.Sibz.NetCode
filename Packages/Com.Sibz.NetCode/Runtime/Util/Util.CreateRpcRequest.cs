using Sibz.NetCode;
using Unity.Entities;
using Unity.NetCode;

namespace Sibz
{
    public static partial class Util
    {
        public static Entity CreateRpcRequest<T>(this World world, T data, Entity targetConnection = default)
            where T : struct, IRpcCommand
        {
            return CreateRpcRequestSystem.CreateRpcRequest(world, data,targetConnection);
        }

        public static Entity CreateRpcRequest<T>(this World world, Entity targetConnection = default)
            where T : struct, IRpcCommand
        {
            return CreateRpcRequestSystem.CreateRpcRequest<T>(world, targetConnection);
        }

        public static Entity CreateRpcRequest<T>(this EntityCommandBuffer buffer, T data,
            Entity targetConnection)
            where T : struct, IRpcCommand
        {
            return CreateRpcRequestSystem.CreateRpcRequest(buffer, data, targetConnection);
        }

        public static Entity CreateRpcRequest<T>(this EntityCommandBuffer.Concurrent buffer, int index, T data,
            Entity targetConnection)
            where T : struct, IRpcCommand
        {
            return CreateRpcRequestSystem.CreateRpcRequest(buffer, index, data, targetConnection);
        }
    }
}