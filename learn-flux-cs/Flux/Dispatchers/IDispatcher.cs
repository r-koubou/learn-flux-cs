using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using LearnFlux.Flux.Actions;

namespace LearnFlux.Flux.Dispatchers;

/// <summary>
/// Dispatcher を表現するインターフェース
/// </summary>
public interface IDispatcher
{
    /// <summary>
    /// ディパッチ実行中かどうか
    /// </summary>
    public bool Dispatching { get; }

    /// <summary>
    /// ディスパッチを受け取るハンドラを追加する
    /// </summary>
    /// <returns>ハンドラ登録解除用のトークン</returns>
    IDisposable Register<TAction>( Func<TAction, Task> callback ) where TAction : IFluxAction;

    /// <summary>
    /// ディスパッチを行う
    /// </summary>
    /// <remarks>既定では<see cref="DispatchAsync{TAction}"/> をブロッキングして実行する</remarks>
    void Dispatch<TAction>( TAction action ) where TAction : IFluxAction
        => DispatchAsync( action ).GetAwaiter().GetResult();

    /// <summary>
    /// ディスパッチを行う
    /// </summary>
    Task DispatchAsync<TAction>( TAction action ) where TAction : IFluxAction;

    void WaitFor( IDisposable tokens )
        => WaitForAsync( tokens ).GetAwaiter().GetResult();

    async Task WaitForAsync( IDisposable tokens )
        => await WaitForAsync( [ tokens ] );

    void WaitFor( IEnumerable<IDisposable> tokens )
        => WaitForAsync( tokens ).GetAwaiter().GetResult();

    Task WaitForAsync( IEnumerable<IDisposable> tokens );
}
