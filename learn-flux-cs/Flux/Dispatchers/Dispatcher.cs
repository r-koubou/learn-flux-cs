using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LearnFlux.Flux.Dispatchers;

public class Dispatcher<TPayload> : IDispatcher<TPayload>
{
    private readonly Dictionary<Guid, Func<TPayload, Task>> callbacks = new();

    public int RegisteredCount
        => callbacks.Count;

    public IDisposable Register( Func<TPayload, Task> callback )
    {
        var id = Guid.NewGuid();
        callbacks.Add( id, callback );

        return new CallbackSubscriptionToken( this, id );
    }

    public async Task DispatchAsync( TPayload payload )
    {
        var tasks = new List<Task>();

        foreach( var callback in callbacks.Values )
        {
            tasks.Add( callback( payload ) );
        }

        await Task.WhenAll( tasks );
    }

    private class CallbackSubscriptionToken : IDisposable
    {
        private readonly Dispatcher<TPayload> dispatcher;
        private readonly Guid id;

        // ReSharper disable once ConvertToPrimaryConstructor
        public CallbackSubscriptionToken( Dispatcher<TPayload> dispatcher, Guid id )
        {
            this.dispatcher = dispatcher;
            this.id         = id;
        }

        public void Dispose()
        {
            dispatcher.callbacks.Remove( id );
        }
    }
}
