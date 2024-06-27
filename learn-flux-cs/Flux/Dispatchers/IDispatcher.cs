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

    /// <summary>
    /// 現在のコールバックの実行を継続する前に、指定されたコールバックが呼び出されるのを待つ。
    /// </summary>
    /// <remarks>既定では<see cref="WaitForAsync(System.Collections.Generic.IEnumerable{System.IDisposable})"/> をブロッキングして実行する</remarks>
    void WaitFor( IEnumerable<IDisposable> tokens )
        => WaitForAsync( tokens ).GetAwaiter().GetResult();

    /// <summary>
    /// 現在のコールバックの実行を継続する前に、指定されたコールバックが呼び出されるのを待つ。
    /// </summary>
    Task WaitForAsync( IEnumerable<IDisposable> tokens );

    /// <summary>
    /// 現在のコールバックの実行を継続する前に、指定されたコールバックが呼び出されるのを待つ。
    /// </summary>
    /// <remarks>既定では<see cref="WaitForAsync(System.IDisposable)"/> をブロッキングして実行する</remarks>
    void WaitFor( IDisposable token )
        => WaitForAsync( token ).GetAwaiter().GetResult();

    /// <summary>
    /// 現在のコールバックの実行を継続する前に、指定されたコールバックが呼び出されるのを待つ。
    /// </summary>
    async Task WaitForAsync( IDisposable token )
     => await WaitForAsync( new[] { token } );
}
