using Unity.Collections;
using Unity.Entities;

namespace Sibz.NetCode.Client
{
    public struct ConnectionCompleteEvent : IComponentData
    {
        public bool Success;
        public NativeString64 Message;
    }
}