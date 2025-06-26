#if !NETSTANDARD2_0
using Grpc.Core;
using Grpc.Core.Interceptors;

namespace Architecture.Shell.Correlation.Grpc;

/// <summary>
/// gRPC client interceptor that automatically injects correlation IDs into outgoing calls
/// </summary>
public class GrpcCorrelationClientInterceptor : Interceptor
{
    private readonly ICorrelationPropagator _correlationPropagator;

    public GrpcCorrelationClientInterceptor(ICorrelationPropagator correlationPropagator)
    {
        _correlationPropagator = correlationPropagator ?? throw new ArgumentNullException(nameof(correlationPropagator));
    }

    public override TResponse BlockingUnaryCall<TRequest, TResponse>(
        TRequest request,
        ClientInterceptorContext<TRequest, TResponse> context,
        BlockingUnaryCallContinuation<TRequest, TResponse> continuation)
    {
        var headers = context.Options.Headers ?? new Metadata();
        _correlationPropagator.Inject(headers);

        var newContext = new ClientInterceptorContext<TRequest, TResponse>(
            context.Method,
            context.Host,
            context.Options.WithHeaders(headers));

        return continuation(request, newContext);
    }

    public override AsyncUnaryCall<TResponse> AsyncUnaryCall<TRequest, TResponse>(
        TRequest request,
        ClientInterceptorContext<TRequest, TResponse> context,
        AsyncUnaryCallContinuation<TRequest, TResponse> continuation)
    {
        var headers = context.Options.Headers ?? new Metadata();
        _correlationPropagator.Inject(headers);

        var newContext = new ClientInterceptorContext<TRequest, TResponse>(
            context.Method,
            context.Host,
            context.Options.WithHeaders(headers));

        return continuation(request, newContext);
    }

    public override AsyncServerStreamingCall<TResponse> AsyncServerStreamingCall<TRequest, TResponse>(
        TRequest request,
        ClientInterceptorContext<TRequest, TResponse> context,
        AsyncServerStreamingCallContinuation<TRequest, TResponse> continuation)
    {
        var headers = context.Options.Headers ?? new Metadata();
        _correlationPropagator.Inject(headers);

        var newContext = new ClientInterceptorContext<TRequest, TResponse>(
            context.Method,
            context.Host,
            context.Options.WithHeaders(headers));

        return continuation(request, newContext);
    }

    public override AsyncClientStreamingCall<TRequest, TResponse> AsyncClientStreamingCall<TRequest, TResponse>(
        ClientInterceptorContext<TRequest, TResponse> context,
        AsyncClientStreamingCallContinuation<TRequest, TResponse> continuation)
    {
        var headers = context.Options.Headers ?? new Metadata();
        _correlationPropagator.Inject(headers);

        var newContext = new ClientInterceptorContext<TRequest, TResponse>(
            context.Method,
            context.Host,
            context.Options.WithHeaders(headers));

        return continuation(newContext);
    }

    public override AsyncDuplexStreamingCall<TRequest, TResponse> AsyncDuplexStreamingCall<TRequest, TResponse>(
        ClientInterceptorContext<TRequest, TResponse> context,
        AsyncDuplexStreamingCallContinuation<TRequest, TResponse> continuation)
    {
        var headers = context.Options.Headers ?? new Metadata();
        _correlationPropagator.Inject(headers);

        var newContext = new ClientInterceptorContext<TRequest, TResponse>(
            context.Method,
            context.Host,
            context.Options.WithHeaders(headers));

        return continuation(newContext);
    }
}

/// <summary>
/// gRPC server interceptor that extracts correlation IDs from incoming calls
/// </summary>
public class GrpcCorrelationServerInterceptor : Interceptor
{
    private readonly ICorrelationPropagator _correlationPropagator;
    private readonly ICorrelationContextFactory _correlationContextFactory;

    public GrpcCorrelationServerInterceptor(
        ICorrelationPropagator correlationPropagator,
        ICorrelationContextFactory correlationContextFactory)
    {
        _correlationPropagator = correlationPropagator ?? throw new ArgumentNullException(nameof(correlationPropagator));
        _correlationContextFactory = correlationContextFactory ?? throw new ArgumentNullException(nameof(correlationContextFactory));
    }

    public override async Task<TResponse> UnaryServerHandler<TRequest, TResponse>(
        TRequest request,
        ServerCallContext context,
        UnaryServerMethod<TRequest, TResponse> continuation)
    {
        SetupCorrelationContext(context);
        try
        {
            return await continuation(request, context);
        }
        finally
        {
            _correlationContextFactory.Dispose();
        }
    }

    public override async Task<TResponse> ClientStreamingServerHandler<TRequest, TResponse>(
        IAsyncStreamReader<TRequest> requestStream,
        ServerCallContext context,
        ClientStreamingServerMethod<TRequest, TResponse> continuation)
    {
        SetupCorrelationContext(context);
        try
        {
            return await continuation(requestStream, context);
        }
        finally
        {
            _correlationContextFactory.Dispose();
        }
    }

    public override async Task ServerStreamingServerHandler<TRequest, TResponse>(
        TRequest request,
        IServerStreamWriter<TResponse> responseStream,
        ServerCallContext context,
        ServerStreamingServerMethod<TRequest, TResponse> continuation)
    {
        SetupCorrelationContext(context);
        try
        {
            await continuation(request, responseStream, context);
        }
        finally
        {
            _correlationContextFactory.Dispose();
        }
    }

    public override async Task DuplexStreamingServerHandler<TRequest, TResponse>(
        IAsyncStreamReader<TRequest> requestStream,
        IServerStreamWriter<TResponse> responseStream,
        ServerCallContext context,
        DuplexStreamingServerMethod<TRequest, TResponse> continuation)
    {
        SetupCorrelationContext(context);
        try
        {
            await continuation(requestStream, responseStream, context);
        }
        finally
        {
            _correlationContextFactory.Dispose();
        }
    }

    private void SetupCorrelationContext(ServerCallContext context)
    {
        var correlationId = _correlationPropagator.Extract(context.RequestHeaders);
        if (!string.IsNullOrEmpty(correlationId))
        {
            _correlationContextFactory.Create(correlationId, "grpc-metadata");
        }
    }
}
#endif