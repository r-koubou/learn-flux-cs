using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LearnFlux.Flux.Dispatchers;

/// <summary>
/// 既定の Dispatcher 実装
/// </summary>
public class Dispatcher<TPayload> : IDispatcher<TPayload>
{
    private readonly Dictionary<Guid, IDispatchHandler<TPayload>> handlers = new();

    ///
    /// <inheritdoc />
    ///
    public IDisposable AddHandler( IDispatchHandler<TPayload> handler )
    {
        var id = Guid.NewGuid();
        handlers.Add( id, handler );

        return new HandlerToken( this, id );
    }

    ///
    /// <inheritdoc />
    ///
    public async Task DispatchAsync( TPayload payload )
    {
        var tasks = new List<Task>();

        foreach( var callback in handlers.Values )
        {
            tasks.Add( callback.HandleAsync( payload ) );
        }

        await Task.WhenAll( tasks );
    }

    private class HandlerToken( Dispatcher<TPayload> dispatcher, Guid id ) : IDisposable
    {
        private readonly Dispatcher<TPayload> dispatcher = dispatcher;
        private readonly Guid id = id;

        public void Dispose()
        {
            dispatcher.handlers.Remove( id );
        }
    }
}
