using System;

namespace LearnFlux.Flux.Actions;

/// <summary>
/// Action のマーカーインターフェース
/// </summary>
public interface IFluxAction {}

public class FluxAction<TActionType>( TActionType type, Exception? error = null ) : IFluxAction
{
    public TActionType Type { get; } = type;
    public Exception? Error { get; } = error;
    public bool HasError => Error != null;
}

public class FluxAction<TActionType, TPayload>( TActionType type, TPayload payload, Exception? error = null ) : FluxAction<TActionType>( type, error )
{
    public TPayload Payload { get; } = payload;
}

// ReSharper disable once ClassNeverInstantiated.Global
public class FluxAction<TActionType, TPayload, TMeta>( TActionType type, TPayload payload, TMeta meta, Exception? error = null ) : FluxAction<TActionType, TPayload>( type, payload, error )
{
    public TMeta Meta { get; } = meta;
}
