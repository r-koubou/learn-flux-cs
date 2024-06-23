using System;
using System.Collections.Generic;
using System.Threading.Tasks;

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
        var dispatcher = new Dispatcher<MockPayload>();
        var store     = new MockStore( dispatcher );

        var hello = new MockPayload( MockEventType.Hello, "Hello, world!" );
        var goodbye = new MockPayload( MockEventType.Goodbye, "Goodbye, world!" );

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

        await dispatcher.DispatchAsync( hello );
        Assert.IsTrue( helloReceived );

        await dispatcher.DispatchAsync( goodbye );
        Assert.IsTrue( goodbyeReceived );

        helloToken.Dispose();
        goodByeToken.Dispose();
    }

    [Test]
    public async Task バインドを解除してDispatcherからペイロードを受信できない()
    {
        var dispatcher = new Dispatcher<MockPayload>();
        var store     = new MockStore( dispatcher );

        var hello = new MockPayload( MockEventType.Hello, "Hello, world!" );
        var helloReceived = false;

        var helloToken = store.Bind( MockEventType.Hello, async payload =>
            {
                helloReceived = true;
                Assert.AreEqual( hello.Message, payload.Message );
                await Task.CompletedTask;
            }
        );

        await dispatcher.DispatchAsync( hello );
        Assert.IsTrue( helloReceived );

        helloToken.Dispose();
        helloReceived = false;

        await dispatcher.DispatchAsync( hello );
        Assert.IsFalse( helloReceived );
    }

    [Test]
    public void 同じ参照のリスナーを多重バインドしようとしたら例外がスローされる()
    {
        var dispatcher = new Dispatcher<MockPayload>();
        var store     = new MockStore( dispatcher );

        async Task Callback( MockPayload _ )
            => await Task.CompletedTask;

        IStoreUpdateListener<MockPayload> listener = new AnonymousUpdateListener<MockPayload>( Callback );

        store.Bind( MockEventType.Hello, listener );
        Assert.Throws<InvalidOperationException>( () => store.Bind( MockEventType.Hello, listener ) );
    }

    [Test]
    public void Funcを用いて匿名リスナーを多重バインドしたはスローされない()
    {
        var dispatcher = new Dispatcher<MockPayload>();
        var store     = new MockStore( dispatcher );

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

    private class MockPayload( MockEventType type, string message )
    {
        public MockEventType Type { get; } = type;
        public string Message { get; } = message;
    }

    private class MockStore( IDispatcher<MockPayload> dispatcher ) : Store<MockEventType, MockPayload>( dispatcher )
    {
        public override async Task HandleAsync( MockPayload payload )
        {
            try
            {
                var tasks = new List<Task>();

                foreach( var listener in StoreBinder.ListenersOf( payload.Type ) )
                {
                    tasks.Add( listener.OnValueUpdatedAsync( payload ) );
                }

                await Task.WhenAll( tasks );
            }
            catch
            {
                // ignored
            }
        }
    }
    #endregion ~Store のモック実装
}
