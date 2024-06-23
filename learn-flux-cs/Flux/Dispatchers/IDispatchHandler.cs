using System.Threading.Tasks;

namespace LearnFlux.Flux.Dispatchers;

public interface IDispatchHandler<in TPayload>
{
    /// <summary>
    /// <see cref="IDispatcher{TPayload}"/> からのコールバックをハンドリングする。
    /// </summary>
    /// <remarks>既定では<see cref="HandleAsync"/> をブロッキングして実行する</remarks>
    void Handle( TPayload payload )
        => HandleAsync( payload ).GetAwaiter().GetResult();

    /// <summary>
    /// <see cref="IDispatcher{TPayload}"/> からのコールバックをハンドリングする。
    /// </summary>
    /// <remarks>
    /// 適宜データ更新や View への通知を行う。
    /// </remarks>
    Task HandleAsync( TPayload payload );
}
