using System.Threading.Tasks;

namespace LearnFlux.Flux.Dispatchers;

public interface IDispatchHandler<in TPayload>
{
    void Handle( TPayload payload )
        => HandleAsync( payload ).GetAwaiter().GetResult();
    Task HandleAsync( TPayload payload );
}
