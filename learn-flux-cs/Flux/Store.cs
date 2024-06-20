using System;
using System.Threading.Tasks;

namespace LearnFlux.Flux;

/// <summary>
/// Storeを表す基底クラス。
/// </summary>
/// <remarks>
/// <see cref="HandleDispatcherAsync"/> で Dispatcher からのコールバックをハンドリングする、という基本的な処理を提供する。
/// View への通知やデータ更新方法の方針は、このStoreクラスの具象クラスで決定・実装をする。
/// </remarks>
public abstract class Store<TPayload> : IDisposable
{
    private readonly IDisposable dispatcherToken;

    // ReSharper disable once ConvertToPrimaryConstructor
    protected Store( IDispatcher<TPayload> dispatcher )
    {
        // Dispatcher からのコールバック登録
        dispatcherToken = dispatcher.Register( HandleDispatcherAsync );
    }

    /// <summary>
    /// <see cref="IDispatcher{TPayload}"/> からのコールバックをハンドリングする。
    /// </summary>
    /// <remarks>
    /// 適宜データ更新や View への通知を行う。
    /// </remarks>
    protected abstract Task HandleDispatcherAsync( TPayload payload );

    ///
    /// <inheritdoc />
    ///
    public virtual void Dispose()
    {
        // Dispatcher からの登録解除
        dispatcherToken.Dispose();
    }
}
