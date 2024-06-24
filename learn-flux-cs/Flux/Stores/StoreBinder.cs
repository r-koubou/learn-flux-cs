using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LearnFlux.Flux.Stores;

/// <summary>
/// デフォルトの <see cref="IStoreBinder"/> 実装
/// </summary>
/// <seealso cref="IStoreBinder" />
public sealed class StoreBinder : IStoreBinder
{
    private readonly Dictionary<object, List<object>> bindings = new();

    ///
    /// <inheritdoc />
    ///
    public IDisposable Bind<TActionType, TPayload>( TActionType actionType, Func<TPayload, Task> onUpdatedAsync )
    {
        _ = actionType ?? throw new ArgumentNullException( nameof( actionType ) );
        _ = onUpdatedAsync ?? throw new ArgumentNullException( nameof( onUpdatedAsync ) );

        if( !bindings.TryGetValue( actionType, out var listeners ) )
        {
            listeners       = [];
            bindings[ actionType ] = listeners;
        }

        if( listeners.Contains( onUpdatedAsync ) )
        {
            throw new InvalidOperationException( $"{actionType} is already bound" );
        }

        listeners.Add( onUpdatedAsync );

        return new BindToken( this, actionType, onUpdatedAsync );
    }

    ///
    /// <inheritdoc />
    ///
    public IEnumerable<Func<TPayload, Task>> ListenersOf<TActionType, TPayload>( TActionType actionType )
    {
        _ = actionType ?? throw new ArgumentNullException( nameof( actionType ) );

        if( bindings.TryGetValue( actionType, out var listeners ) )
        {
            var result = new List<Func<TPayload, Task>>();

            foreach( var x in listeners )
            {
                result.Add( (Func<TPayload, Task>)x );
            }
            return result;
        }

        return Array.Empty<Func<TPayload, Task>>();
    }

    private class BindToken( StoreBinder binder, object actionType, object callback ) : IDisposable
    {
        private readonly StoreBinder binder = binder;
        private readonly object actionType = actionType;
        private readonly object callback = callback;

        public void Dispose()
        {
            _ = actionType ?? throw new InvalidOperationException( $"{nameof(actionType)} is null" );

            var bindings = binder.bindings;
            if( bindings.TryGetValue( actionType, out var listeners ) )
            {
                listeners.Remove( callback );
            }
        }
    }
}
