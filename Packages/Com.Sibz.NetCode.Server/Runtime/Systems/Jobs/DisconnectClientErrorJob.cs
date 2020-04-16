using Sibz.EntityEvents;
using Sibz.NetCode.Client;
using Unity.Collections;
using Unity.Jobs;

namespace Sibz.NetCode.Server
{
    public struct DisconnectClientErrorJob : IJobParallelFor
    {
        [DeallocateOnJobCompletion]
        [ReadOnly]
        [NativeDisableParallelForRestriction]
        public NativeArray<DisconnectClient> DisconnectClients;

        public EnqueueEventJobPart<DisconnectClientFailedEvent> EnqueueEventJobPart;

        public void Execute(int index)
        {
            if (DisconnectClients[index].NetworkConnectionId == -1)
            {
                return;
            }

            EnqueueEventJobPart.EventData.Id = DisconnectClients[index].NetworkConnectionId;
            EnqueueEventJobPart.Execute();
        }
    }
}