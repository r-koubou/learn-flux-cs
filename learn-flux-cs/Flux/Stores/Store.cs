using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using LearnFlux.Flux.Dispatchers;

namespace LearnFlux.Flux.Stores;

/// <summary>
/// Store を表現するデフォルト実装
/// </summary>
public abstract class Store : IStoreBinder
{
    protected readonly IDispatcher dispatcher;
    protected readonly StoreBinder storeBinder = new();
    protected Store( IDispatcher dispatcher )
    {
        this.dispatcher = dispatcher;
    }

    protected async Task UpdateAsync<TActionType, TPayload>( TActionType type, TPayload payload )
    {
        try
        {
            var tasks = new List<Task>();

            foreach( var callback in storeBinder.CallbacksOf<TActionType, TPayload>( type ) )
            {
                tasks.Add( callback( payload ) );
            }

            await Task.WhenAll( tasks );
        }
        catch
        {
            // ignored
        }
    }

    ///
    /// <inheritdoc />
    ///
    public IDisposable Bind<TActionType, TPayload>( TActionType actionType, Func<TPayload, Task> callback )
        => storeBinder.Bind( actionType, callback );

    ///
    /// <inheritdoc />
    ///
    public IEnumerable<Func<TPayload, Task>> CallbacksOf<TActionType, TPayload>( TActionType actionType )
        => storeBinder.CallbacksOf<TActionType, TPayload>( actionType );
}
