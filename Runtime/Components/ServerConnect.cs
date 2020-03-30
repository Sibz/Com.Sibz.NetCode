using Unity.Entities;
using Unity.Networking.Transport;

namespace Sibz.NetCode
{
    public struct ServerConnect: IComponentData
    {
        public NetworkEndPoint EndPoint;
        public int Timeout;
        public float InitialTime;

        public float TimeoutTime => InitialTime + Timeout;
    }
}