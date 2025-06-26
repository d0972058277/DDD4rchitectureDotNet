# Correlation ID Support for Microservices

This library provides comprehensive correlation ID support across multiple communication protocols commonly used in microservices architectures.

## Supported Protocols

- **HTTP/REST APIs** (ASP.NET Core, HttpClient)
- **gRPC** (Client & Server interceptors)
- **Generic metadata containers** (IDictionary-based)

## Basic Setup

### 1. Register Core Services

```csharp
// Program.cs or Startup.cs
services.AddCorrelation(options =>
{
    options.RequestHeader = "X-Correlation-ID";
    options.IncludeInResponse = true;
    options.AddToLoggingScope = true;
});

// For HTTP middleware
app.UseCorrelation();
```

### 2. HTTP Client Support

```csharp
// Register HTTP correlation support
services.AddHttpCorrelation();

// Configure named HTTP clients
services.AddHttpClient<MyApiClient>(client =>
{
    client.BaseAddress = new Uri("https://api.example.com");
})
.AddCorrelationPropagation(); // Automatically adds correlation headers

// Or configure all HTTP clients globally
services.ConfigureAll<HttpClientFactoryOptions>(options =>
{
    options.HttpMessageHandlerBuilderActions.Add(builder =>
    {
        builder.AdditionalHandlers.Add(builder.Services.GetRequiredService<HttpCorrelationHandler>());
    });
});
```

### 3. gRPC Support

```csharp
// Register gRPC correlation support
services.AddGrpcCorrelation();

// Server-side: Add interceptor to gRPC services
services.AddGrpc(options =>
{
    options.Interceptors.Add<GrpcCorrelationServerInterceptor>();
});

// Client-side: Configure gRPC client with interceptor
services.AddGrpcClient<MyGrpcService.MyGrpcServiceClient>(options =>
{
    options.Address = new Uri("https://grpc.example.com");
})
.AddInterceptor<GrpcCorrelationClientInterceptor>();

// Or manually configure client
var channel = GrpcChannel.ForAddress("https://grpc.example.com");
var client = new MyGrpcService.MyGrpcServiceClient(channel)
    .Intercept(serviceProvider.GetRequiredService<GrpcCorrelationClientInterceptor>());
```

## Manual Propagation

For custom protocols or scenarios requiring manual control:

```csharp
public class CustomServiceClient
{
    private readonly ICorrelationPropagator _correlationPropagator;
    
    public CustomServiceClient(ICorrelationPropagator correlationPropagator)
    {
        _correlationPropagator = correlationPropagator;
    }
    
    public async Task CallServiceAsync()
    {
        // For custom metadata containers
        var metadata = new Dictionary<string, string>();
        _correlationPropagator.Inject(metadata);
        
        // Send request with metadata...
    }
    
    public void HandleIncomingRequest(IDictionary<string, string> headers)
    {
        // Extract correlation ID from incoming metadata
        var correlationId = _correlationPropagator.Extract(headers);
        
        if (!string.IsNullOrEmpty(correlationId))
        {
            // Set up correlation context for this request
            // This would typically be done by protocol-specific middleware
        }
    }
}
```

## Custom Protocol Support

To add support for a new protocol, implement `ICorrelationProtocolHandler`:

```csharp
public class MyProtocolHandler : ICorrelationProtocolHandler
{
    public Type MetadataType => typeof(MyProtocolMetadata);

    public void Inject(object metadata, string headerName, string correlationId)
    {
        if (metadata is MyProtocolMetadata myMetadata)
        {
            myMetadata.Headers[headerName] = correlationId;
        }
    }

    public string? Extract(object metadata, string headerName)
    {
        if (metadata is MyProtocolMetadata myMetadata)
        {
            return myMetadata.Headers.TryGetValue(headerName, out var value) ? value : null;
        }
        return null;
    }
}

// Register your custom handler
services.AddTransient<ICorrelationProtocolHandler, MyProtocolHandler>();
```

## Advanced Scenarios

### Service-to-Service Communication Chain

```
API Gateway → Service A → Service B → Service C
     ↓           ↓           ↓           ↓
  Generate    Propagate   Propagate   Propagate
     ID      via HTTP    via gRPC    via Custom
```

Each service automatically:
1. Extracts correlation ID from incoming requests
2. Sets up correlation context for the request scope
3. Injects correlation ID into all outgoing calls
4. Includes correlation ID in structured logs

### Error Correlation

```csharp
public class GlobalExceptionHandler
{
    private readonly ICorrelationService _correlationService;
    private readonly ILogger<GlobalExceptionHandler> _logger;

    public async Task HandleAsync(HttpContext context, Exception exception)
    {
        var correlationId = _correlationService.CorrelationId;
        
        _logger.LogError(exception, 
            "Unhandled exception occurred. CorrelationId: {CorrelationId}", 
            correlationId);
            
        // Return correlation ID in error response for client-side correlation
        context.Response.Headers["X-Correlation-ID"] = correlationId.ToString();
    }
}
```

## Configuration Options

```csharp
services.AddCorrelation(options =>
{
    // Header configuration
    options.RequestHeader = "X-Correlation-ID";          // Default: "X-Correlation-ID"
    options.ResponseHeader = "X-Response-Correlation";   // Default: same as RequestHeader
    
    // Behavior options
    options.IgnoreRequestHeader = false;                 // Default: false
    options.EnforceHeader = true;                        // Default: false (returns 400 if missing)
    options.IncludeInResponse = true;                    // Default: true
    options.UpdateTraceIdentifier = true;               // Default: false
    
    // Logging integration
    options.AddToLoggingScope = true;                    // Default: false
    options.LoggingScopeKey = "CorrelationId";          // Default: "CorrelationId"
    
    // Custom ID generation
    options.CorrelationIdGenerator = () => $"custom-{Guid.NewGuid():N}";
});
```

This comprehensive solution ensures correlation IDs flow seamlessly across all your microservice communication boundaries, regardless of the underlying protocol.