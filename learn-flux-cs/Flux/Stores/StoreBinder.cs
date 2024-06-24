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

    private class BindToken( StoreBinder binder, object actionType, object callback ) : IDisposable
    {
        private readonly StoreBinder binder = binder;
        private readonly object actionType = actionType;
        private readonly object callback = callback;

        public void Dispose()
        {
            _ = actionType ?? throw new InvalidOperationException( $"{nameof(actionType)} is null" );

            var bindings = binder.bindings;
            if( bindings.TryGetValue( actionType, out var callbacks ) )
            {
                callbacks.Remove( callback );
            }
        }
    }
}
