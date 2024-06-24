using System.Threading.Tasks;

using LearnFlux.Flux.Actions;
using LearnFlux.Flux.Dispatchers;

using NUnit.Framework;

namespace FluxLearn.FLux.Test;

[TestFixture]
public class DispatcherTest
{
    private enum ActionType
    {
        Hello
    }

    [Test]
    public async Task DispatcherからPayloadを受信できる()
    {
        var dispatcher = new Dispatcher();

        const string payload = "Hello, world!";
        var receivedPayload = "";

        var action = new FluxAction<ActionType, string>( ActionType.Hello, payload );

        var token = dispatcher.AddHandler<FluxAction<ActionType, string>>( async a =>
            {
                receivedPayload = a.Payload;
                await Task.CompletedTask;
            }
        );


        await dispatcher.DispatchAsync( action );
        Assert.AreEqual( payload, receivedPayload );

        token.Dispose();
    }
}
