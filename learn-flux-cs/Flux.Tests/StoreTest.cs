using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using LearnFlux.Flux.Actions;
using LearnFlux.Flux.Dispatchers;
using LearnFlux.Flux.Stores;

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

        var helloToken = store.Bind<MockEventType, MockPayload>( MockEventType.Hello, async payload =>
            {
                helloReceived = true;
                Assert.AreEqual( hello.Message, payload.Message );
                await Task.CompletedTask;
            }
        );

        var goodByeToken = store.Bind<MockEventType, MockPayload>( MockEventType.Goodbye, async payload =>
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

        var helloToken = store.Bind<MockEventType, MockPayload>( MockEventType.Hello, async payload =>
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

        var callback = new Func<MockPayload, Task>( async _ => await Task.CompletedTask );

        store.Bind( MockEventType.Hello, callback );
        Assert.Throws<InvalidOperationException>( () => store.Bind( MockEventType.Hello, callback ) );
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

    private class MockStore : IDisposable, IStoreBinder
    {
        private IDisposable? dispatcherToken;
        private readonly StoreBinder storeBinder = new();

        public MockStore( IDispatcher dispatcher )
        {
            dispatcherToken = dispatcher.Register<MockAction>( async action => await UpdateAsync( action ) );
        }

        private async Task UpdateAsync( MockAction action )
        {
            try
            {
                var tasks = new List<Task>();

                foreach( var listener in storeBinder.CallbacksOf<MockEventType, MockPayload>( action.Type ) )
                {
                    tasks.Add( listener( action.Payload ) );
                }

                await Task.WhenAll( tasks );
            }
            catch
            {
                // ignored
            }
        }

        public IDisposable Bind<TActionType, TPayload>( TActionType actionType, Func<TPayload, Task> callback )
            => storeBinder.Bind( actionType, callback );

        public IEnumerable<Func<TPayload, Task>> CallbacksOf<TActionType, TPayload>( TActionType actionType )
            => storeBinder.CallbacksOf<TActionType, TPayload>( actionType );

        public void Dispose()
        {
            dispatcherToken?.Dispose();
            dispatcherToken = null;
        }
    }

    #endregion ~Store のモック実装
}
