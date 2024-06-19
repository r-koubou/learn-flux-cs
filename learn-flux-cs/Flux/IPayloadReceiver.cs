using System.Threading.Tasks;

namespace LearnFlux.Flux;

public interface IPayloadReceiver<in TPayload>
{
    Task ReceiveAsync( TPayload payload );
}
