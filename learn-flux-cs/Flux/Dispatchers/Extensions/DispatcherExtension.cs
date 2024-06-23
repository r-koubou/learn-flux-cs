using System;
using System.Threading.Tasks;

namespace LearnFlux.Flux.Dispatchers.Extensions;

public static class DispatcherExtension
{
    public static IDisposable AddHandler<TPayload>( this IDispatcher<TPayload> me, Func<TPayload, Task> handlerAsync )
        => me.AddHandler( new AnonymousHandler<TPayload>( handlerAsync ) );
}
