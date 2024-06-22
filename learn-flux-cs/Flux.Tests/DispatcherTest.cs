using System.Threading.Tasks;

using LearnFlux.Flux.Dispatchers;
using LearnFlux.Flux.Dispatchers.Extensions;

using NUnit.Framework;

namespace FluxLearn.FLux.Test;

[TestFixture]
public class DispatcherTest
{
    [Test]
    public async Task DispatcherからPayloadを受信できる()
    {
        var dispatcher = new Dispatcher<string>();

        const string payload = "Hello, world!";
        var receivedPayload = "";

        var token = dispatcher.AddHandler( async p =>
            {
                receivedPayload = p;
                await Task.CompletedTask;
            }
        );

        await dispatcher.DispatchAsync( payload );
        Assert.AreEqual( payload, receivedPayload );

        token.Dispose();
    }
}
