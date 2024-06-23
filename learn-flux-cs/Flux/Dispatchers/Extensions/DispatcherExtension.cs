using System;
using System.Threading.Tasks;

namespace LearnFlux.Flux.Dispatchers.Extensions;

/// <summary>
/// <see cref="IDispatcher{TPayload}"/> に対する拡張メソッド
/// </summary>
public static class DispatcherExtension
{
    /// <summary>
    /// Func・ラムダ式を利用してハンドラを登録する
    /// </summary>
    public static IDisposable AddHandler<TPayload>( this IDispatcher<TPayload> me, Func<TPayload, Task> handlerAsync )
        => me.AddHandler( new AnonymousHandler<TPayload>( handlerAsync ) );
}
