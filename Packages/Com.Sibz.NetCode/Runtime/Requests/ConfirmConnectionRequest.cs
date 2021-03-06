﻿using Unity.Burst;
using Unity.NetCode;
using Unity.Networking.Transport;

namespace Sibz.NetCode
{
    [BurstCompile]
    public struct ConfirmConnectionRequest : IRpcCommand
    {
        public void Serialize(ref DataStreamWriter writer)
        {
        }

        public void Deserialize(ref DataStreamReader reader)
        {
        }

        private static readonly PortableFunctionPointer<RpcExecutor.ExecuteDelegate> InvokeExecuteFunctionPointer =
            new PortableFunctionPointer<RpcExecutor.ExecuteDelegate>(InvokeExecute);

        public PortableFunctionPointer<RpcExecutor.ExecuteDelegate> CompileExecute()
        {
            return InvokeExecuteFunctionPointer;
        }

        [BurstCompile]
        private static void InvokeExecute(ref RpcExecutor.Parameters parameters)
        {
            RpcExecutor.ExecuteCreateRequestComponent<ConfirmConnectionRequest>(ref parameters);
        }
    }
}