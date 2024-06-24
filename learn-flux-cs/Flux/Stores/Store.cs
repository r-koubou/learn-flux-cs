using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using LearnFlux.Flux.Dispatchers;

namespace LearnFlux.Flux.Stores;

/// <summary>
/// <see cref="IStore"/>のデフォルト実装
/// </summary>
public abstract class Store( IDispatcher dispatcher ) : IStore
{
    protected readonly IDispatcher dispatcher = dispatcher;
    private readonly StoreBinder storeBinder = new();

    ///
    /// <inheritdoc />
    ///
    public async Task EmitAsync<TActionType, TPayload>( TActionType type, TPayload payload )
    {
        try
        {
            var tasks = new List<Task>();

            foreach( var callback in CallbacksOf<TActionType, TPayload>( type ) )
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
    public virtual IDisposable Bind<TActionType, TPayload>( TActionType actionType, Func<TPayload, Task> callback )
        => storeBinder.Bind( actionType, callback );

    ///
    /// <inheritdoc />
    ///
    public IEnumerable<Func<TPayload, Task>> CallbacksOf<TActionType, TPayload>( TActionType actionType )
        => storeBinder.CallbacksOf<TActionType, TPayload>( actionType );
}
