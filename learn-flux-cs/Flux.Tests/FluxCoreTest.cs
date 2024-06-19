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
        var receiver = new MockPayloadReceiver<string>();

        using( dispatcher.Register( receiver ) )
        {
            Assert.AreEqual( 1, dispatcher.SubscriberCount );
        }

        Assert.AreEqual( 0, dispatcher.SubscriberCount );
    }

    [Test]
    public async Task DispatcherからPayloadを受信できる()
    {
        var dispatcher = new Dispatcher<string>();
        var receiver = new MockPayloadReceiver<string>();

        const string payload = "Hello, world!";

        using( dispatcher.Register( receiver ) )
        {
            await dispatcher.DispatchAsync( payload );
            Assert.AreEqual( payload, receiver.ReceivedPayload );
        }
    }
}
