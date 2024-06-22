using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using LearnFlux.Flux;
using LearnFlux.Flux.Dispatchers;

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
    public void 多重バインドしようとしたら例外がスローされる()
    {
        var dispatcher = new Dispatcher<MockPayload>();
        var store     = new MockStore( dispatcher );

        store.Bind( MockEventType.Hello, async _ =>
            {
                await Task.CompletedTask;
            }
        );

        Assert.Throws<InvalidOperationException>( () =>
            {
                store.Bind( MockEventType.Hello, async _ =>
                    {
                        await Task.CompletedTask;
                    }
                );
            }
        );

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

    private class MockStore( IDispatcher<MockPayload> dispatcher ) : Store<MockPayload>( dispatcher )
    {
        private readonly Dictionary<MockEventType, Func<MockPayload, Task>> bindings = new();

        protected override async Task HandleDispatcherAsync( MockPayload payload )
        {
            if( !bindings.TryGetValue( payload.Type, out var binding ) )
            {
                await Task.CompletedTask;
                return;
            }

            try
            {
                await binding( payload );
            }
            catch
            {
                // ignored
            }
        }

        public IDisposable Bind( MockEventType type, Func<MockPayload, Task> callback )
        {
            if( !bindings.TryAdd( type, callback ) )
            {
                throw new InvalidOperationException( $"{type} is already bound" );
            }

            return new BindToken( bindings, type );
        }

        private class BindToken( Dictionary<MockEventType, Func<MockPayload, Task>> bindings, MockEventType type ) : IDisposable
        {
            private readonly Dictionary<MockEventType, Func<MockPayload, Task>> bindings = bindings;
            private readonly MockEventType type = type;

            public void Dispose()
            {
                bindings.Remove( type );
            }
        }
    }
    #endregion ~Store のモック実装
}
