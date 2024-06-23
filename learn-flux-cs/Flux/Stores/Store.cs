using System;
using System.Collections.Generic;
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

    /// <summary>
    /// この Store が管理する IStoreBinder インスタンス
    /// </summary>
    protected virtual IStoreBinder<TPayloadKey, TPayload> StoreBinder { get; } = new StoreBinder<TPayloadKey, TPayload>();

    // ReSharper disable once ConvertToPrimaryConstructor
    protected Store( IDispatcher<TPayload> dispatcher )
    {
        // Dispatcher からのコールバック登録
        dispatcherToken = dispatcher.AddHandler( async payload => await HandleAsync( payload ) );
    }

    #region IDispatchHandler<TPayload>
    ///
    /// <inheritdoc />
    ///
    public abstract Task HandleAsync( TPayload payload );
    #endregion ~IDispatchHandler<TPayload>

    #region IStoreBinder<TPayloadKey, TPayload>
    ///
    /// <inheritdoc />
    ///
    public virtual IDisposable Bind( TPayloadKey key, IStoreUpdateListener<TPayload> listener )
        => StoreBinder.Bind( key, listener );

    ///
    /// <inheritdoc />
    ///
    public IEnumerable<IStoreUpdateListener<TPayload>> ListenersOf( TPayloadKey key )
        => throw new NotImplementedException();
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
