using Unity.NetCode;
using Unity.Networking.Transport;
using UnityEngine;

namespace Sibz.NetCode
{
    public class ServerWorldBase : WorldBase<ServerSimulationSystemGroup>
    {
        protected virtual IServerOptionsBase Options { get; set; }

        protected ServerWorldBase(IServerOptionsBase options) : base(options, ClientServerBootstrap.CreateServerWorld)
        {
            Options = options;
            if (options.ConnectOnSpawn)
            {
            }
        }

        public void Listen()
        {
            CreateEventEntity(new ServerConnect
            {
                EndPoint = NetworkEndPoint.Parse(Options.Address, Options.Port, Options.NetworkFamily),
                Timeout = Options.ConnectTimeout,
                InitialTime = Time.time
            });
        }
    }
}