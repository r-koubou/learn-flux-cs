using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using LearnFlux.Flux.Actions;

namespace LearnFlux.Flux.Dispatchers;

/// <summary>
/// 既定の Dispatcher 実装
/// </summary>
public class Dispatcher<TAction> : IDispatcher<TAction> where TAction : IFluxAction
{
    private readonly Dictionary<Guid, IDispatchHandler<TAction>> handlers = new();

    ///
    /// <inheritdoc />
    ///
    public IDisposable AddHandler( IDispatchHandler<TAction> handler )
    {
        var id = Guid.NewGuid();
        handlers.Add( id, handler );

        return new HandlerToken( this, id );
    }

    ///
    /// <inheritdoc />
    ///
    public async Task DispatchAsync( TAction action )
    {
        var tasks = new List<Task>();

        foreach( var callback in handlers.Values )
        {
            tasks.Add( callback.HandleAsync( action ) );
        }

        await Task.WhenAll( tasks );
    }

    private class HandlerToken( Dispatcher<TAction> dispatcher, Guid id ) : IDisposable
    {
        private readonly Dispatcher<TAction> dispatcher = dispatcher;
        private readonly Guid id = id;

        public void Dispose()
        {
            dispatcher.handlers.Remove( id );
        }
    }
}
