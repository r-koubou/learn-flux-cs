using System;
using System.Threading.Tasks;

namespace LearnFlux.Flux.Stores.Extensions;

/// <summary>
/// <see cref="IStoreBinder{TBindKey, TPayload}"/> に対する拡張メソッド
/// </summary>
public static class StoreBinderExtension
{
    /// <summary>
    /// Func・ラムダ式を利用してリスナーを登録する
    /// </summary>
    /// <returns>リスナー登録解除用のトークン</returns>
    /// <seealso cref="IStoreBinder{TBindKey, TPayload}.Bind(TBindKey, IStoreUpdateListener{TPayload})"/>
    public static IDisposable Bind<TBindKey, TPayload>( this IStoreBinder<TBindKey, TPayload> me, TBindKey key, Func<TPayload, Task> onValueUpdatedAsync )
        => me.Bind( key, new AnonymousUpdateListener<TPayload>( onValueUpdatedAsync ) );
}
