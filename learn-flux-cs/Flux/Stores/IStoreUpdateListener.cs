using System;

namespace LearnFlux.Flux.Stores;

public interface IStoreUpdateListener<in TPayload>
{
    void OnStoreUpdated( TPayload payload );

    static IStoreUpdateListener<TPayload> Anonymous( Action<TPayload> onStoreUpdated )
        => new UpdateListener( onStoreUpdated );

    private class UpdateListener( Action<TPayload> onStoreUpdated ) : IStoreUpdateListener<TPayload>
    {
        public void OnStoreUpdated( TPayload payload )
            => onStoreUpdated( payload );
    }
}
