using System;
using System.Threading.Tasks;

using LearnFlux.Flux.Actions;

namespace LearnFlux.Flux.Dispatchers.Extensions;

/// <summary>
/// <see cref="IDispatcher{TPayload}"/> に対する拡張メソッド
/// </summary>
public static class DispatcherExtension
{
    /// <summary>
    /// Func・ラムダ式を利用してハンドラを登録する
    /// </summary>
    public static IDisposable AddHandler<TAction>( this IDispatcher<TAction> me, Func<TAction, Task> handlerAsync ) where TAction : IFluxAction
        => me.AddHandler( new AnonymousHandler<TAction>( handlerAsync ) );
}
