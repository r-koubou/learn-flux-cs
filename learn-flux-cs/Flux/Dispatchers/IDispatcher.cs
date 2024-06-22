using System;
using System.Threading.Tasks;

namespace LearnFlux.Flux.Dispatchers;

public interface IDispatcher<TPayload>
{
    int RegisteredCount { get; }
    IDisposable Register( Func<TPayload, Task> callback );
    void Dispatch( TPayload payload )
        => DispatchAsync( payload ).GetAwaiter().GetResult();
    Task DispatchAsync( TPayload payload );
}
