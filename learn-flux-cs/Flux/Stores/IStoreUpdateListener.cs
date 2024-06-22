using System.Threading.Tasks;

namespace LearnFlux.Flux.Stores;

public interface IStoreUpdateListener<in TPayload>
{
    void OnStoreUpdated( TPayload payload )
        => OnStoreUpdatedAsync( payload ).GetAwaiter().GetResult();
    Task OnStoreUpdatedAsync( TPayload payload );
}
