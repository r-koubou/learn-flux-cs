using System.Threading.Tasks;

using LearnFlux.Flux.Actions;

namespace LearnFlux.Flux.Dispatchers;

public interface IDispatchHandler<in TAction> where TAction : IFluxAction
{
    /// <summary>
    /// <see cref="IDispatcher{TPayload}"/> からのコールバックをハンドリングする。
    /// </summary>
    /// <remarks>既定では<see cref="HandleAsync"/> をブロッキングして実行する</remarks>
    void Handle( TAction action )
        => HandleAsync( action ).GetAwaiter().GetResult();

    /// <summary>
    /// <see cref="IDispatcher{TPayload}"/> からのコールバックをハンドリングする。
    /// </summary>
    /// <remarks>
    /// 適宜データ更新や View への通知を行う。
    /// </remarks>
    Task HandleAsync( TAction action );
}
