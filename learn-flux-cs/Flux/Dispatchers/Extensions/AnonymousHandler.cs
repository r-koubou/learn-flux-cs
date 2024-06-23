using System;
using System.Threading.Tasks;

namespace LearnFlux.Flux.Dispatchers.Extensions;

/// <summary>
/// Func・ラムダ式を利用してハンドラ登録可能な匿名ハンドラ
/// </summary>
/// <param name="handleAsync"><see cref="IDispatchHandler{TPayload}.HandleAsync"/>に対応する処理</param>
public class AnonymousHandler<TPayload>( Func<TPayload, Task> handleAsync ) : IDispatchHandler<TPayload>
{
    private readonly Func<TPayload, Task> handleAsync = handleAsync;

    ///
    /// <inheritdoc />
    ///
    public async Task HandleAsync( TPayload payload )
    {
        await handleAsync( payload );
    }
}
