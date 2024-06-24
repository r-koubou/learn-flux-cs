using System;
using System.Threading.Tasks;

using LearnFlux.Flux.Actions;

namespace LearnFlux.Flux.Dispatchers;

/// <summary>
/// Dispatcher を表現するインターフェース
/// </summary>
public interface IDispatcher<TAction> where TAction : IFluxAction
{
    /// <summary>
    /// ディスパッチを受け取るハンドラを追加する
    /// </summary>
    /// <returns>ハンドラ登録解除用のトークン</returns>
    IDisposable AddHandler( IDispatchHandler<TAction> handler );

    /// <summary>
    /// ディスパッチを行う
    /// </summary>
    /// <remarks>既定では<see cref="DispatchAsync"/> をブロッキングして実行する</remarks>
    void Dispatch( TAction action )
        => DispatchAsync( action ).GetAwaiter().GetResult();

    /// <summary>
    /// ディスパッチを行う
    /// </summary>
    Task DispatchAsync( TAction action );
}
