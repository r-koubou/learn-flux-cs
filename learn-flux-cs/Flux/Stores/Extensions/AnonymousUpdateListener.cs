using System;
using System.Threading.Tasks;

namespace LearnFlux.Flux.Stores.Extensions;

/// <summary>
/// Func・ラムダ式を利用して Store の更新通知を受取可能な匿名リスナー
/// </summary>
/// <param name="onValueUpdatedAsync"><see cref="IStoreUpdateListener{TPayload}.OnValueUpdatedAsync"/>に対応する処理</param>
public class AnonymousUpdateListener<TPayload>( Func<TPayload, Task> onValueUpdatedAsync ) : IStoreUpdateListener<TPayload>
{
    ///
    /// <inheritdoc />
    ///
    public async Task OnValueUpdatedAsync( TPayload payload )
        => await onValueUpdatedAsync( payload );
}
