using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LearnFlux.Flux.Stores;

/// <summary>
/// Store を表現する基底インターフェース
/// </summary>
public interface IStore
{
    /// <summary>
    /// actionType に対応する通知先を登録する
    /// </summary>
    /// <typeparam name="TActionType">通知を識別するためのキーの型</typeparam>
    /// <typeparam name="TPayload">通知の内容の型</typeparam>
    /// <returns>登録解除用のトークン</returns>
    IDisposable Bind<TActionType, TPayload>( TActionType actionType, Func<TPayload, Task> callback );

    /// <summary>
    /// actionType に対応する現在の通知先のリストを取得する
    /// </summary>
    /// <typeparam name="TActionType">通知を識別するためのキーの型</typeparam>
    /// <typeparam name="TPayload">通知の内容の型</typeparam>
    /// <returns>通知先のリスト、何も登録されていない場合は空のリスト</returns>
    IEnumerable<Func<TPayload, Task>> CallbacksOf<TActionType, TPayload>( TActionType actionType );

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
