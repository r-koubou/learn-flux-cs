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
    private readonly Dictionary<object, List<object>> bindings = new();

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
    public IDisposable Bind<TActionType, TPayload>( TActionType actionType, Func<TPayload, Task> callback )
    {
        _ = actionType ?? throw new ArgumentNullException( nameof( actionType ) );
        _ = callback ?? throw new ArgumentNullException( nameof( callback ) );

        if( !bindings.TryGetValue( actionType, out var callbacks ) )
        {
            callbacks              = [];
            bindings[ actionType ] = callbacks;
        }

        if( callbacks.Contains( callback ) )
        {
            throw new InvalidOperationException( $"{actionType} is already bound" );
        }

        callbacks.Add( callback );

        return new BindToken( this, actionType, callback );
    }

    ///
    /// <inheritdoc />
    ///
    public IEnumerable<Func<TPayload, Task>> CallbacksOf<TActionType, TPayload>( TActionType actionType )
    {
        _ = actionType ?? throw new ArgumentNullException( nameof( actionType ) );

        if( bindings.TryGetValue( actionType, out var callback ) )
        {
            var result = new List<Func<TPayload, Task>>();

            foreach( var x in callback )
            {
                result.Add( (Func<TPayload, Task>)x );
            }

            return result;
        }

        return Array.Empty<Func<TPayload, Task>>();
    }

    private class BindToken( Store store, object actionType, object callback ) : IDisposable
    {
        private readonly Store store = store;
        private readonly object actionType = actionType;
        private readonly object callback = callback;

        public void Dispose()
        {
            _ = actionType ?? throw new InvalidOperationException( $"{nameof( actionType )} is null" );

            var bindings = store.bindings;

            if( bindings.TryGetValue( actionType, out var callbacks ) )
            {
                callbacks.Remove( callback );
            }
        }
    }
}
