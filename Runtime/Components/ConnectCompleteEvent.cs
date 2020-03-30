using Unity.Collections;
using Unity.Entities;

namespace Sibz.NetCode
{
    public struct ConnectCompleteEvent : IComponentData
    {
        public bool Success;
        public NativeString64 Message;
    }
}