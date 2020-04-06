using Sibz.EntityEvents;
using Unity.Collections;

namespace Sibz.NetCode.Client
{
    public struct ConnectionFailedEvent : IEventComponentData
    {
        public NativeString64 Message;
    }
}