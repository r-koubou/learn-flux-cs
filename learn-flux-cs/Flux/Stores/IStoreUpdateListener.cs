using System.Threading.Tasks;

namespace LearnFlux.Flux.Stores;

/// <summary>
/// Store 内で管理するデータの更新通知を受け取るリスナー
/// </summary>
/// <typeparam name="TPayload"></typeparam>
public interface IStoreUpdateListener<in TPayload>
{
    /// <summary>
    /// データ更新時に通知される
    /// </summary>
    /// <remarks>既定では<see cref="OnValueUpdatedAsync"/> をブロッキングして実行する</remarks>
    void OnValueUpdated( TPayload payload )
        => OnValueUpdatedAsync( payload ).GetAwaiter().GetResult();

    /// <summary>
    /// データ更新時に通知される
    /// </summary>
    Task OnValueUpdatedAsync( TPayload payload );
}
