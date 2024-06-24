using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using LearnFlux.Flux.Actions;
using LearnFlux.Flux.Dispatchers;
using LearnFlux.Flux.Stores;
using LearnFlux.Flux.Stores.Extensions;

using NUnit.Framework;

namespace FluxLearn.FLux.Test;

[TestFixture]
public class StoreTest
{
    [Test]
    public async Task バインドしてDispatcherからペイロードを受信できる()
    {
        var dispatcher = new Dispatcher();
        var store = new MockStore( dispatcher );

        var hello = new MockPayload( "Hello, world!" );
        var goodbye = new MockPayload( "Goodbye, world!" );

        var helloAction = new MockAction( MockEventType.Hello, hello );
        var goodbyeAction = new MockAction( MockEventType.Goodbye, goodbye );

        var helloReceived = false;
        var goodbyeReceived = false;

        var helloToken = store.Bind( MockEventType.Hello, async payload =>
            {
                helloReceived = true;
                Assert.AreEqual( hello.Message, payload.Message );
                await Task.CompletedTask;
            }
        );

        var goodByeToken = store.Bind( MockEventType.Goodbye, async payload =>
            {
                goodbyeReceived = true;
                Assert.AreEqual( goodbye.Message, payload.Message );
                await Task.CompletedTask;
            }
        );

        await dispatcher.DispatchAsync( helloAction );
        Assert.IsTrue( helloReceived );

        await dispatcher.DispatchAsync( goodbyeAction );
        Assert.IsTrue( goodbyeReceived );

        helloToken.Dispose();
        goodByeToken.Dispose();
    }

    [Test]
    public async Task バインドを解除してDispatcherからペイロードを受信できない()
    {
        var dispatcher = new Dispatcher();
        var store = new MockStore( dispatcher );

        var hello = new MockPayload( "Hello, world!" );
        var helloAction = new MockAction( MockEventType.Hello, hello );

        var helloReceived = false;

        var helloToken = store.Bind( MockEventType.Hello, async payload =>
            {
                helloReceived = true;
                Assert.AreEqual( hello.Message, payload.Message );
                await Task.CompletedTask;
            }
        );

        await dispatcher.DispatchAsync( helloAction );
        Assert.IsTrue( helloReceived );

        helloToken.Dispose();
        helloReceived = false;

        await dispatcher.DispatchAsync( helloAction );
        Assert.IsFalse( helloReceived );
    }

    [Test]
    public void 同じ参照のリスナーを多重バインドしようとしたら例外がスローされる()
    {
        var dispatcher = new Dispatcher();
        var store = new MockStore( dispatcher );

        async Task Callback( MockPayload _ )
            => await Task.CompletedTask;

        IStoreUpdateListener<MockPayload> listener = new AnonymousUpdateListener<MockPayload>( Callback );

        store.Bind( MockEventType.Hello, listener );
        Assert.Throws<InvalidOperationException>( () => store.Bind( MockEventType.Hello, listener ) );
    }

    [Test]
    public void Funcを用いて匿名リスナーを多重バインドしたはスローされない()
    {
        var dispatcher = new Dispatcher();
        var store = new MockStore( dispatcher );

        async Task Callback( MockPayload _ )
            => await Task.CompletedTask;

        store.Bind( MockEventType.Hello, Callback );
        Assert.DoesNotThrow( () => store.Bind( MockEventType.Hello, Callback ) );
    }

    #region Store のモック実装

    private enum MockEventType
    {
        Hello,
        Goodbye
    }

    private class MockAction( MockEventType type, MockPayload payload, Exception? error = null )
        : FluxAction<MockEventType, MockPayload>( type, payload, error ) {}

    private class MockPayload( string message )
    {
        public string Message { get; } = message;
    }

    private class MockStore :
        IDisposable,
        IStoreBinder<MockEventType, MockPayload>
    {
        private IDisposable? dispatcherToken;
        private readonly StoreBinder<MockEventType, MockPayload> storeBinder = new();

        public MockStore( IDispatcher dispatcher )
        {
            dispatcherToken = dispatcher.AddHandler<MockAction>( async action => await HandleAsync( action ) );
        }

        public async Task HandleAsync( MockAction action )
        {
            try
            {
                var tasks = new List<Task>();

                foreach( var listener in storeBinder.ListenersOf( action.Type ) )
                {
                    tasks.Add( listener.OnValueUpdatedAsync( action.Payload ) );
                }

                await Task.WhenAll( tasks );
            }
            catch
            {
                // ignored
            }
        }

        public IDisposable Bind( MockEventType actionType, IStoreUpdateListener<MockPayload> listener )
            => storeBinder.Bind( actionType, listener );

        public IEnumerable<IStoreUpdateListener<MockPayload>> ListenersOf( MockEventType actionType )
            => storeBinder.ListenersOf( actionType );

        public void Dispose()
        {
            dispatcherToken?.Dispose();
            dispatcherToken = null;
        }
    }

    #endregion ~Store のモック実装
}
