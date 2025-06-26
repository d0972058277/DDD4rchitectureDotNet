# Messaging Correlation Examples

This document demonstrates how to use the messaging correlation functionality with RabbitMQ and Kafka.

## Setup

First, add messaging correlation support to your services:

```csharp
services.AddCorrelation()
    .AddMessagingCorrelation();
```

## RabbitMQ Integration

### Producer (Publishing Messages)

```csharp
public class OrderEventPublisher
{
    private readonly IConnection _connection;
    private readonly RabbitMqCorrelationIntegration _correlationIntegration;

    public OrderEventPublisher(IConnection connection, RabbitMqCorrelationIntegration correlationIntegration)
    {
        _connection = connection;
        _correlationIntegration = correlationIntegration;
    }

    public void PublishOrderCreated(OrderCreatedEvent orderEvent)
    {
        using var channel = _connection.CreateModel();
        
        var properties = channel.CreateBasicProperties();
        
        // Inject correlation ID into message properties
        _correlationIntegration.InjectCorrelationId(properties);
        
        var body = JsonSerializer.SerializeToUtf8Bytes(orderEvent);
        
        channel.BasicPublish(
            exchange: "orders",
            routingKey: "order.created",
            basicProperties: properties,
            body: body);
    }
}
```

### Consumer (Processing Messages)

```csharp
public class OrderEventConsumer
{
    private readonly RabbitMqCorrelationIntegration _correlationIntegration;
    private readonly ICorrelationContextFactory _correlationFactory;

    public OrderEventConsumer(
        RabbitMqCorrelationIntegration correlationIntegration,
        ICorrelationContextFactory correlationFactory)
    {
        _correlationIntegration = correlationIntegration;
        _correlationFactory = correlationFactory;
    }

    public void HandleMessage(BasicDeliverEventArgs args)
    {
        // Extract and create correlation context
        using var correlationContext = _correlationIntegration.CreateCorrelationContext(
            args.BasicProperties, _correlationFactory);
        
        var orderEvent = JsonSerializer.Deserialize<OrderCreatedEvent>(args.Body.Span);
        
        // Process the order with correlation context available
        ProcessOrder(orderEvent);
    }
}
```

## Kafka Integration

### Producer (Publishing Messages)

```csharp
public class PaymentEventProducer
{
    private readonly IProducer<string, PaymentProcessedEvent> _producer;
    private readonly KafkaCorrelationIntegration _correlationIntegration;

    public PaymentEventProducer(
        IProducer<string, PaymentProcessedEvent> producer,
        KafkaCorrelationIntegration correlationIntegration)
    {
        _producer = producer;
        _correlationIntegration = correlationIntegration;
    }

    public async Task PublishPaymentProcessedAsync(PaymentProcessedEvent paymentEvent)
    {
        var message = new Message<string, PaymentProcessedEvent>
        {
            Key = paymentEvent.PaymentId,
            Value = paymentEvent
        };
        
        // Inject correlation ID into message headers
        _correlationIntegration.InjectCorrelationId(message);
        
        await _producer.ProduceAsync("payments", message);
    }
}
```

### Consumer (Processing Messages)

```csharp
public class PaymentEventConsumer
{
    private readonly KafkaCorrelationIntegration _correlationIntegration;
    private readonly ICorrelationContextFactory _correlationFactory;

    public PaymentEventConsumer(
        KafkaCorrelationIntegration correlationIntegration,
        ICorrelationContextFactory correlationFactory)
    {
        _correlationIntegration = correlationIntegration;
        _correlationFactory = correlationFactory;
    }

    public void HandleMessage(ConsumeResult<string, PaymentProcessedEvent> consumeResult)
    {
        // Extract and create correlation context from message
        using var correlationContext = _correlationIntegration.CreateCorrelationContext(
            consumeResult.Message, _correlationFactory);
        
        var paymentEvent = consumeResult.Message.Value;
        
        // Process the payment with correlation context available
        ProcessPayment(paymentEvent);
    }
    
    // Alternative: Extract from headers directly
    public void HandleMessageFromHeaders(ConsumeResult<string, PaymentProcessedEvent> consumeResult)
    {
        using var correlationContext = _correlationIntegration.CreateCorrelationContext(
            consumeResult.Message.Headers, _correlationFactory);
        
        // Process message...
    }
}
```

## Direct Protocol Handler Usage

For advanced scenarios, you can use the protocol handlers directly:

```csharp
public class AdvancedMessagingService
{
    private readonly ICorrelationPropagator _propagator;

    public AdvancedMessagingService(ICorrelationPropagator propagator)
    {
        _propagator = propagator;
    }

    public void HandleRabbitMqMessage(IBasicProperties properties)
    {
        // Direct injection/extraction
        _propagator.Inject(properties);
        var correlationId = _propagator.Extract(properties);
    }

    public void HandleKafkaHeaders(Headers headers)
    {
        // Direct injection/extraction
        _propagator.Inject(headers);
        var correlationId = _propagator.Extract(headers);
    }
    
    public void HandleGenericDictionary(IDictionary<string, string> metadata)
    {
        // Works with any IDictionary implementation (fallback mechanism)
        _propagator.Inject(metadata);
        var correlationId = _propagator.Extract(metadata);
    }
}
```

## Benefits

1. **Automatic Correlation Propagation**: Correlation IDs are automatically injected into outgoing messages and extracted from incoming messages.

2. **Protocol Agnostic**: Same interface works with HTTP, gRPC, RabbitMQ, Kafka, and any IDictionary-based metadata.

3. **Logging Integration**: When correlation context is active, correlation IDs are automatically included in log scopes.

4. **Distributed Tracing**: Enables end-to-end tracing across microservices using different communication protocols.

5. **Type Safety**: Protocol-specific handlers provide compile-time safety while fallback mechanisms ensure compatibility.