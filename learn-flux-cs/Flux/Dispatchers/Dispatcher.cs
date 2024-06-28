using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;

using LearnFlux.Flux.Actions;

namespace LearnFlux.Flux.Dispatchers;

/// <summary>
/// 既定の Dispatcher 実装
/// </summary>
public class Dispatcher : IDispatcher
{
    private readonly Dictionary<IDisposable, object> callbacks = new();

    private readonly Dictionary<IDisposable, bool> pendings = new();
    private readonly Dictionary<IDisposable, bool> handled = new();
    private object? pendingPayload;

    ///
    /// <inheritdoc />
    ///
    public bool Dispatching { get; private set; }

    ///
    /// <inheritdoc />
    ///
    public IDisposable Register<TAction>( Func<TAction, Task> callback ) where TAction : IFluxAction
    {
        var token = new CallbackToken( this );
        callbacks.Add( token, callback );

        return token;
    }

    ///
    /// <inheritdoc />
    ///
    public async Task DispatchAsync<TAction>( TAction action ) where TAction : IFluxAction
    {
        if( Dispatching )
        {
            throw new InvalidOperationException( $"{nameof(DispatchAsync)}: Must be invoked while not dispatching." );
        }

        var tasks = new List<Task>();

        StartDispatching( action );

        try
        {
            foreach( var token in callbacks.Keys )
            {
                if( pendings.GetValueOrDefault( token ) )
                {
                    continue;
                }

                tasks.Add( InvokeCallbackAsync<TAction>( token ) );
            }

            await Task.WhenAll( tasks );
        }
        finally
        {
            StopDispatching();
        }
    }

    /// <summary>
    /// js 実装の _invokeCallback相当
    /// </summary>
    private async Task InvokeCallbackAsync<TAction>( IDisposable token ) where TAction : IFluxAction
    {
        if( pendingPayload == null )
        {
            throw new InvalidOperationException( $"{nameof(InvokeCallbackAsync)}: {nameof(pendingPayload)} is null." );
        }

        pendings[ token ] = true;

        var callback = (Func<TAction, Task>)callbacks[ token ];
        await callback( (TAction)pendingPayload! );

        handled[ token ] = true;
    }

    public async Task WaitForAsync( IEnumerable<IDisposable> tokens )
    {
        if( !Dispatching )
        {
            throw new InvalidOperationException( $"{nameof(WaitForAsync)}: Must be invoked while dispatching." );
        }

        foreach( var token in tokens )
        {
            await WaitForAsyncImpl( token );
        }
    }

    private async Task WaitForAsyncImpl( IDisposable token )
    {
        if( pendings.GetValueOrDefault( token ) )
        {
            if( !handled.GetValueOrDefault( token ) )
            {
                throw new InvalidOperationException( $"{nameof( WaitForAsync )}: Circular dependency detected while waiting for `{token}`" );
            }

            return;
        }

        if( !callbacks.TryGetValue( token, out var callback ) )
        {
            throw new InvalidOperationException( $"{nameof(WaitForAsync)}: Token `{token}` does not map to a registered callback." );
        }

        // callbacks の Value (object) から Func<TAction, Task> へのキャスト
        var callbackArgument = callback.GetType().GetGenericArguments()[ 0 ];
        var method = GetType().GetMethod( nameof( InvokeCallbackAsync ), BindingFlags.NonPublic | BindingFlags.Instance );
        var genericMethod = method?.MakeGenericMethod( callbackArgument );

        if( genericMethod == null )
        {
            throw new InvalidOperationException( $"{nameof(WaitForAsync)}: Failed to create generic method." );
        }

        await (Task)genericMethod.Invoke( this, [token] )!;
    }

    /// <summary>
    /// js 実装の _startDispatching 相当
    /// </summary>
    private void StartDispatching<TAction>( TAction payload ) where TAction : IFluxAction
    {
        foreach( var token in callbacks.Keys )
        {
            pendings.Remove( token );
            handled.Remove( token );
        }

        pendingPayload = payload;
        Dispatching    = true;
    }

    /// <summary>
    /// js 実装の _stopDispatching 相当
    /// </summary>
    private void StopDispatching()
    {
        pendingPayload = null;
        Dispatching    = false;
    }

    private class CallbackToken( Dispatcher dispatcher ) : IDisposable
    {
        private readonly Dispatcher dispatcher = dispatcher;

        public void Dispose()
        {
            dispatcher.callbacks.Remove( this );
        }
    }
}
