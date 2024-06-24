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

        var token = dispatcher.AddHandler<MockAction>( async a =>
            {
                receivedPayload = a.Payload;
                await Task.CompletedTask;
            }
        );


        await dispatcher.DispatchAsync( action );
        Assert.AreEqual( payload, receivedPayload );

        token.Dispose();
    }

    private enum MockActionType
    {
        Hello
    }

    private class MockAction( MockActionType type, string payload )
        : FluxAction<MockActionType, string>( type, payload );
}
