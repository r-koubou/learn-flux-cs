using System;

namespace LearnFlux.Flux.Stores;

public interface IStoreBinder<in TBindKey, out TPayload>
{
    IDisposable Bind( TBindKey key, IStoreUpdateListener<TPayload> callback );
}
