using System;
using System.Threading.Tasks;

using LearnFlux.Flux;

using NUnit.Framework;

namespace FluxLearn.FLux.Test;

[TestFixture]
public class FluxCoreTest
{
    [Test]
    public void Dispatcherに購読と解除ができる()
    {
        var dispatcher = new Dispatcher<string>();

        var token = dispatcher.Register( async _ =>
            {
                await Task.CompletedTask;
            }
        );

        Assert.AreEqual( 1, dispatcher.RegisteredCount );

        token.Dispose();
        Assert.AreEqual( 0, dispatcher.RegisteredCount );
    }

    [Test]
    public async Task DispatcherからPayloadを受信できる()
    {
        var dispatcher = new Dispatcher<string>();

        const string payload = "Hello, world!";
        var receivedPayload = "";

        var token = dispatcher.Register( p =>
            {
                receivedPayload = p;
                return Task.CompletedTask;
            }
        );

        await dispatcher.DispatchAsync( payload );
        Assert.AreEqual( payload, receivedPayload );

        token.Dispose();
    }
}
