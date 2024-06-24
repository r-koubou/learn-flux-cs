using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using LearnFlux.Flux.Actions;

namespace LearnFlux.Flux.Dispatchers;

/// <summary>
/// 既定の Dispatcher 実装
/// </summary>
public class Dispatcher : IDispatcher
{
    private readonly Dictionary<Guid, object> handlers = new();

    ///
    /// <inheritdoc />
    ///
    public IDisposable AddHandler<TAction>( Func<TAction, Task> handle ) where TAction : IFluxAction
    {
        var id = Guid.NewGuid();
        handlers.Add( id, handle );

        return new HandlerToken( this, id );
    }

    ///
    /// <inheritdoc />
    ///
    public async Task DispatchAsync<TAction>( TAction action ) where TAction : IFluxAction
    {
        var tasks = new List<Task>();

        foreach( var callback in handlers.Values )
        {
            tasks.Add( ((Func<TAction, Task>)callback).Invoke( action ) );
        }

        await Task.WhenAll( tasks );
    }

    private class HandlerToken( Dispatcher dispatcher, Guid id ) : IDisposable
    {
        private readonly Dispatcher dispatcher = dispatcher;
        private readonly Guid id = id;

        public void Dispose()
        {
            dispatcher.handlers.Remove( id );
        }
    }
}
