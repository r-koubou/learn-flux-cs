using System;
using System.Threading.Tasks;

namespace LearnFlux.Flux.Dispatchers;

/// <summary>
/// Dispatcher を表現するインターフェース
/// </summary>
public interface IDispatcher<TPayload>
{
    /// <summary>
    /// ディスパッチを受け取るハンドラを追加する
    /// </summary>
    /// <returns>ハンドラ登録解除用のトークン</returns>
    IDisposable AddHandler( IDispatchHandler<TPayload> handler );

    /// <summary>
    /// ディスパッチを行う
    /// </summary>
    /// <remarks>既定では<see cref="DispatchAsync"/> をブロッキングして実行する</remarks>
    void Dispatch( TPayload payload )
        => DispatchAsync( payload ).GetAwaiter().GetResult();

    /// <summary>
    /// ディスパッチを行う
    /// </summary>
    Task DispatchAsync( TPayload payload );
}
