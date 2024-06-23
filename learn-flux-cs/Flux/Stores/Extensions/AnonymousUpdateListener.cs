using System;
using System.Threading.Tasks;

namespace LearnFlux.Flux.Stores.Extensions;

public class AnonymousUpdateListener<TPayload>( Func<TPayload, Task> onStoreUpdated ) : IStoreUpdateListener<TPayload>
{
    public async Task OnValueUpdatedAsync( TPayload payload )
        => await onStoreUpdated( payload );
}
