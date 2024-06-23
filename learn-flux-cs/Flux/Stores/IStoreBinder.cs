using System;
using System.Collections.Generic;

namespace LearnFlux.Flux.Stores;

/// <summary>
/// Store の更新通知を管理する
/// </summary>
/// <typeparam name="TBindKey">通知を識別するためのキーの型</typeparam>
/// <typeparam name="TPayload">通知の内容の型</typeparam>
public interface IStoreBinder<in TBindKey, TPayload>
{
    /// <summary>
    /// key に対応する通知先を登録する
    /// </summary>
    /// <returns>登録解除用のトークン</returns>
    IDisposable Bind( TBindKey key, IStoreUpdateListener<TPayload> listener );

    /// <summary>
    /// key に対応する現在の通知先のリストを取得する
    /// </summary>
    /// <returns>通知先のリスト、何も登録されていない場合は空のリスト</returns>
    IEnumerable<IStoreUpdateListener<TPayload>> ListenersOf( TBindKey key );
}
