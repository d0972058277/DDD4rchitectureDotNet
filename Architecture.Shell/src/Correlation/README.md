# Correlation 關聯性識別碼

本目錄包含微服務架構中關聯性識別碼 (Correlation ID) 的完整實作，支援多種通訊協議的端到端分散式追蹤。

## 📁 目錄結構

```
Correlation/
├── 關聯性識別碼文檔.md          # 完整的中文使用指南
├── README.md                    # 本文件
├── 核心檔案/
│   ├── CorrelationContext.cs           # 關聯性上下文
│   ├── ICorrelationService.cs          # 關聯性服務介面
│   ├── CorrelationService.cs           # 關聯性服務實作
│   ├── ICorrelationPropagator.cs       # 傳播器介面
│   ├── CorrelationPropagator.cs        # 傳播器實作
│   ├── ICorrelationProtocolHandler.cs  # 協議處理器介面
│   ├── ICorrelationContextAccessor.cs  # 上下文存取器介面
│   ├── CorrelationContextAccessor.cs   # 上下文存取器實作
│   ├── ICorrelationContextFactory.cs   # 上下文工廠介面
│   ├── CorrelationContextFactory.cs    # 上下文工廠實作
│   ├── CorrelationIdOptions.cs         # 配置選項
│   ├── CorrelationMiddleware.cs         # ASP.NET Core 中介軟體
│   ├── CorrelationServiceCollectionExtensions.cs  # DI 擴展方法
│   └── CorrelationApplicationBuilderExtensions.cs # 應用程式建構器擴展
├── Http/
│   ├── HttpCorrelationProtocolHandler.cs  # HTTP 協議處理器
│   └── HttpCorrelationHandler.cs          # HTTP 客戶端處理器
├── Grpc/
│   ├── GrpcCorrelationProtocolHandler.cs  # gRPC 協議處理器
│   └── GrpcCorrelationInterceptor.cs      # gRPC 攔截器
└── Messaging/
    ├── RabbitMqCorrelationProtocolHandler.cs  # RabbitMQ 協議處理器
    ├── RabbitMqCorrelationIntegration.cs      # RabbitMQ 整合輔助類別
    ├── KafkaCorrelationProtocolHandler.cs     # Kafka 協議處理器
    ├── KafkaCorrelationIntegration.cs         # Kafka 整合輔助類別
    └── 訊息佇列使用範例.md                     # 訊息佇列使用範例
```

## 🚀 快速開始

### 1. 基本設定

```csharp
// 註冊核心服務
services.AddCorrelation(options =>
{
    options.RequestHeader = "X-Correlation-ID";
    options.AddToLoggingScope = true;
});

// 註冊 ASP.NET Core 中介軟體
app.UseCorrelation();
```

### 2. 通訊協議支援

```csharp
// HTTP 支援
services.AddHttpCorrelation();
services.AddHttpClient<MyApiClient>()
    .AddCorrelationPropagation();

// gRPC 支援
services.AddGrpcCorrelation();
services.AddGrpc(options => 
{
    options.Interceptors.Add<GrpcCorrelationServerInterceptor>();
});

// 訊息佇列支援 (RabbitMQ + Kafka)
services.AddMessagingCorrelation();
```

## 📖 詳細文檔

請參閱 **[關聯性識別碼文檔.md](./關聯性識別碼文檔.md)** 獲取：

- 完整的架構說明
- 所有支援協議的使用範例
- 配置選項詳解
- 高級功能與最佳實踐
- 效能考量與疑難排解

## 🎯 主要特性

### ✅ 多協議支援

- **HTTP/REST APIs**: ASP.NET Core middleware、HttpClient 自動注入
- **gRPC**: Client & Server interceptors
- **RabbitMQ**: IBasicProperties.Headers 處理
- **Kafka**: Headers 與 Message 支援，byte[] 轉換
- **IceRPC**: 透過 IDictionary fallback 機制
- **自訂協議**: 實作 ICorrelationProtocolHandler 介面

### ✅ 自動化功能

- **自動提取**: 從傳入請求中提取關聯性 ID
- **自動注入**: 將關聯性 ID 注入到外發請求
- **上下文管理**: 執行緒安全的 AsyncLocal 儲存
- **日誌整合**: 自動加入到結構化日誌作用域

### ✅ 企業級功能

- **錯誤關聯**: 在錯誤追蹤中包含關聯性 ID
- **效能監控**: 最小化效能影響
- **可擴展性**: 支援自訂協議和處理邏輯
- **可觀察性**: 與 OpenTelemetry 等工具整合

## 🔄 工作流程

```
┌─────────────┐    HTTP/gRPC/MQ     ┌─────────────┐
│   服務 A    │ ──── Correlation ──→ │   服務 B    │
│             │      ID Inject      │             │
│ ┌─────────┐ │                     │ ┌─────────┐ │
│ │ Request │ │                     │ │ Extract │ │
│ │ Handler │ │                     │ │ Context │ │
│ └─────────┘ │                     │ └─────────┘ │
└─────────────┘                     └─────────────┘
       │                                   │
       ▼                                   ▼
┌─────────────┐                   ┌─────────────┐
│ Logging     │                   │ Logging     │
│ Scope       │                   │ Scope       │
│ + Corr ID   │                   │ + Corr ID   │
└─────────────┘                   └─────────────┘
```

## 📊 測試覆蓋率

本實作包含 **133 個單元測試**，涵蓋：

- 核心功能測試：52 個
- HTTP 協議測試：12 個
- gRPC 協議測試：5 個
- RabbitMQ 協議測試：14 個
- Kafka 協議測試：12 個
- 整合測試：38 個

## 🤝 貢獻指南

1. 閱讀完整文檔了解架構
2. 確保所有測試通過
3. 遵循既有的程式碼風格
4. 為新功能添加對應測試
5. 更新相關文檔

---