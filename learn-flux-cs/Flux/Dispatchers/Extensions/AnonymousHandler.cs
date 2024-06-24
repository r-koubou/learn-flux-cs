using System;
using System.Threading.Tasks;

using LearnFlux.Flux.Actions;

namespace LearnFlux.Flux.Dispatchers.Extensions;

/// <summary>
/// Func・ラムダ式を利用してハンドラ登録可能な匿名ハンドラ
/// </summary>
/// <param name="handleAsync"><see cref="IDispatchHandler{TPayload}.HandleAsync"/>に対応する処理</param>
public class AnonymousHandler<TAction>( Func<TAction, Task> handleAsync ) : IDispatchHandler<TAction> where TAction : IFluxAction
{
    private readonly Func<TAction, Task> handleAsync = handleAsync;

    ///
    /// <inheritdoc />
    ///
    public async Task HandleAsync( TAction action )
    {
        await handleAsync( action );
    }
}
