using System;
using System.Threading.Tasks;

namespace LearnFlux.Flux.Dispatchers;

public interface IDispatcher<TPayload>
{
    IDisposable AddHandler( IDispatchHandler<TPayload> handler );
    void Dispatch( TPayload payload )
        => DispatchAsync( payload ).GetAwaiter().GetResult();
    Task DispatchAsync( TPayload payload );
}
