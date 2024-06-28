using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using LearnFlux.Flux.Actions;
using LearnFlux.Flux.Dispatchers;

using NUnit.Framework;

namespace FluxLearn.FLux.Test;

[TestFixture]
public class DispatcherTest
{
    [Test]
    public async Task DispatcherからPayloadを受信できる()
    {
        var dispatcher = new Dispatcher();

        const string payload = "Hello, world!";
        var receivedPayload = "";

        var action = new MockAction( MockActionType.Hello, payload );

        var token = dispatcher.Register<MockAction>( async a =>
            {
                receivedPayload = a.Payload;
                await Task.CompletedTask;
            }
        );


        await dispatcher.DispatchAsync( action );
        Assert.AreEqual( payload, receivedPayload );

        token.Dispose();
    }

    [Test]
    public async Task WaitForで順序の制御ができる()
    {
        var dispatcher = new Dispatcher();
        var log = new List<string>();

        var token1 = dispatcher.Register<MockAction>( async action =>
            {
                log.Add( $"Callback1: {action.Payload}" );
                await Task.CompletedTask;
            }
        );

        _ = dispatcher.Register<MockAction>( async action =>
            {
                await dispatcher.WaitForAsync( new[] { token1 } );
                log.Add( $"Callback2: {action.Payload}" );
                await Task.CompletedTask;
            }
        );

        var action = new MockAction( MockActionType.Hello, "TestPayload" );

        await dispatcher.DispatchAsync( action );

        Assert.AreEqual( 2, log.Count );
        Assert.AreEqual( "Callback1: TestPayload", log[ 0 ] );
        Assert.AreEqual( "Callback2: TestPayload", log[ 1 ] );
    }

    [Test]
    public void ディスパッチ処理中にディスパッチを行おうとすると例外がスローされる()
    {
        var dispatcher = new Dispatcher();
        var log = new List<string>();

        _ = dispatcher.Register<MockAction>( async action =>
            {
                log.Add( $"Callback: {action.Payload}" );
                await dispatcher.DispatchAsync( action ); // ここで例外がスローされる
                await Task.CompletedTask;
            }
        );

        var action = new MockAction( MockActionType.Hello, "TestPayload" );

        Assert.ThrowsAsync<InvalidOperationException>( async () =>
            {
                await dispatcher.DispatchAsync( action );
            }
        );
    }

    [Test]
    public void ディスパッチ処理外でWaitForを実行すると例外がスローされる()
    {
        var dispatcher = new Dispatcher();
        var log = new List<string>();

        var token = dispatcher.Register<MockAction>( async action =>
            {
                log.Add( $"Callback: {action.Payload}" );
                await Task.CompletedTask;
            }
        );

        Assert.ThrowsAsync<InvalidOperationException>( async () =>
            {
                await dispatcher.WaitForAsync( new[] { token } );
            }
        );

        Assert.IsEmpty( log );
    }

    private enum MockActionType
    {
        Hello
    }

    private class MockAction( MockActionType type, string payload )
        : FluxAction<MockActionType, string>( type, payload );
}
