using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LearnFlux.Flux;

public class Dispatcher<TPayload>
{
    private readonly Dictionary<Guid, IPayloadReceiver<TPayload>> callbacks = new();

    public int SubscriberCount
        => callbacks.Count;

    public IDisposable Register( IPayloadReceiver<TPayload> callback )
    {
        var id = Guid.NewGuid();
        callbacks.Add( id, callback );

        return new DispatchToken( this, id );
    }

    public async Task DispatchAsync( TPayload payload )
    {
        var tasks = new List<Task>();

        foreach( var callback in callbacks.Values )
        {
            tasks.Add( callback.ReceiveAsync( payload ) );
        }

        await Task.WhenAll( tasks );
    }

    private class DispatchToken : IDisposable
    {
        private readonly Dispatcher<TPayload> dispatcher;
        private readonly Guid id;

        // ReSharper disable once ConvertToPrimaryConstructor
        public DispatchToken( Dispatcher<TPayload> dispatcher, Guid id )
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
