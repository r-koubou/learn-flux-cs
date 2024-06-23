using System;
using System.Collections.Generic;

namespace LearnFlux.Flux.Stores;

/// <summary>
/// デフォルトの <see cref="IStoreBinder{TBindKey, TPayload}"/> 実装
/// </summary>
/// <seealso cref="IStoreBinder{TBindKey, TPayload}" />
public sealed class StoreBinder<TBindKey, TPayload> : IStoreBinder<TBindKey, TPayload>
{
    private readonly Dictionary<TBindKey, List<IStoreUpdateListener<TPayload>>> bindings = new();

    ///
    /// <inheritdoc />
    ///
    public IDisposable Bind( TBindKey key, IStoreUpdateListener<TPayload> listener )
    {
        if( !bindings.TryGetValue( key, out var listeners ) )
        {
            listeners       = [];
            bindings[ key ] = listeners;
        }

        if( listeners.Contains( listener ) )
        {
            throw new InvalidOperationException( $"{key} is already bound" );
        }

        listeners.Add( listener );

        return new BindToken( bindings, key, listener );
    }

    ///
    /// <inheritdoc />
    ///
    public IEnumerable<IStoreUpdateListener<TPayload>> ListenersOf( TBindKey key )
    {
        if( bindings.TryGetValue( key, out var listeners ) )
        {
            return listeners;
        }

        return Array.Empty<IStoreUpdateListener<TPayload>>();
    }

    private class BindToken : IDisposable
    {
        private readonly Dictionary<TBindKey, List<IStoreUpdateListener<TPayload>>> bindings;
        private readonly TBindKey key;
        private readonly IStoreUpdateListener<TPayload> listener;

        // ReSharper disable once ConvertToPrimaryConstructor
        public BindToken( Dictionary<TBindKey, List<IStoreUpdateListener<TPayload>>> bindings, TBindKey key, IStoreUpdateListener<TPayload> listener )
        {
            this.bindings = bindings;
            this.key      = key;
            this.listener = listener;
        }

        public void Dispose()
        {
            if( bindings.TryGetValue( key, out var listeners ) )
            {
                listeners.Remove( listener );
                bindings[ key ] = listeners;
            }
        }
    }
}
