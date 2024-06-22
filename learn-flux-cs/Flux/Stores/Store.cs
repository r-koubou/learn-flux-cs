using System;
using System.Threading.Tasks;

using LearnFlux.Flux.Dispatchers;
using LearnFlux.Flux.Dispatchers.Extensions;

namespace LearnFlux.Flux.Stores;

/// <summary>
/// Storeを表す基底クラス。
/// </summary>
public abstract class Store<TPayloadKey, TPayload>
    : IDisposable,
      IDispatchHandler<TPayload>,
      IStoreBinder<TPayloadKey, TPayload>
{
    private readonly IDisposable dispatcherToken;

    // ReSharper disable once ConvertToPrimaryConstructor
    protected Store( IDispatcher<TPayload> dispatcher )
    {
        // Dispatcher からのコールバック登録
        dispatcherToken = dispatcher.AddHandler( async payload => await HandleAsync( payload ) );
    }

    #region IDispatchHandler<TPayload>
    /// <summary>
    /// <see cref="IDispatcher{TPayload}"/> からのコールバックをハンドリングする。
    /// </summary>
    /// <remarks>
    /// 適宜データ更新や View への通知を行う。
    /// </remarks>
    public abstract Task HandleAsync( TPayload payload );
    #endregion ~IDispatchHandler<TPayload>

    #region IStoreBinder<TPayloadKey, TPayload>
    ///
    /// <inheritdoc />
    ///
    public abstract IDisposable Bind( TPayloadKey key, IStoreUpdateListener<TPayload> callback );
    #endregion ~IStoreBinder<TPayloadKey, TPayload>

    #region IDisposable
    ///
    /// <inheritdoc />
    ///
    public virtual void Dispose()
    {
        // Dispatcher からの登録解除
        dispatcherToken.Dispose();
    }
    #endregion ~IDisposable
}
