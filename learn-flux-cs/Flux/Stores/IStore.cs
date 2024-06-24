using System.Threading.Tasks;

namespace LearnFlux.Flux.Stores;

/// <summary>
/// Store を表現する基底インターフェース
/// </summary>
public interface IStore : IStoreBinder
{
    /// <summary>
    /// Store 内のデータ更新イベントの発火を行う
    /// </summary>
    void Emit<TActionType, TPayload>( TActionType type, TPayload payload )
        => EmitAsync( type, payload ).GetAwaiter().GetResult();

    /// <summary>
    /// Store 内のデータ更新イベントの発火を行う
    /// </summary>
    Task EmitAsync<TActionType, TPayload>( TActionType type, TPayload payload );
}
