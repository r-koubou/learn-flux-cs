using System.Threading.Tasks;

using LearnFlux.Flux;

namespace FluxLearn.FLux.Test;

internal class MockPayloadReceiver<TPayload> : IPayloadReceiver<TPayload>
{
    public TPayload ReceivedPayload { get; private set; } = default!;

    public Task ReceiveAsync( TPayload payload )
    {
        ReceivedPayload = payload;
        return Task.CompletedTask;
    }
}
