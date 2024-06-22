using System;
using System.Threading.Tasks;

namespace LearnFlux.Flux.Stores.Extensions;

public static class StoreBinderExtension
{
    public static IDisposable Bind<TBindKey, TPayload>( this IStoreBinder<TBindKey, TPayload> me, TBindKey key, Func<TPayload, Task> onStoreUpdated )
        => me.Bind( key, new AnonymousUpdateListener<TPayload>( onStoreUpdated ) );
}
