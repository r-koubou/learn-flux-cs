using System;
using System.Threading.Tasks;

namespace LearnFlux.Flux.Dispatchers.Extensions;

public static class DispatcherExtension
{
    public static IDisposable AddHandler<TPayload>( this IDispatcher<TPayload> me, Func<TPayload, Task> handlerAsync )
        => me.AddHandler( new AnonymousHandler<TPayload>( handlerAsync ) );

    private class AnonymousHandler<TPayload>( Func<TPayload, Task> handleAsync ) : IDispatchHandler<TPayload>
    {
        private readonly Func<TPayload, Task> handleAsync = handleAsync;

        public async Task HandleAsync( TPayload payload )
        {
            await handleAsync( payload );
        }
    }
}
