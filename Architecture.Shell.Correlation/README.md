# Architecture.Shell.Correlation

微服務關聯性識別碼 (Correlation ID) 支援函式庫，提供跨多種通訊協議的端到端分散式追蹤功能。

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

### ✅ 主要功能
- **錯誤關聯**: 在錯誤追蹤中包含關聯性 ID
- **效能監控**: 最小化效能影響
- **可擴展性**: 支援自訂協議和處理邏輯
- **可觀察性**: 與 OpenTelemetry 等工具整合

## 📦 安裝

```bash
dotnet add package Architecture.Shell.Correlation
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

## 📖 完整文檔

- **[詳細使用指南](src/關聯性識別碼文檔.md)** - 完整的架構說明與使用範例
- **[訊息佇列範例](src/Messaging/訊息佇列使用範例.md)** - RabbitMQ 與 Kafka 詳細範例
- **[API 文檔](src/README.md)** - 核心 API 與設定選項

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

```bash
# 執行測試
dotnet test

# 執行測試並產生覆蓋率報告
dotnet test /p:CollectCoverage=true
```

## 🏗️ 開發與建置

```bash
# 克隆專案
git clone https://github.com/your-repo/DDD4rchitectureDotNet.git
cd Architecture.Shell.Correlation

# 還原依賴項
dotnet restore

# 建置專案
dotnet build

# 執行測試
dotnet test

# 建立 NuGet 套件
dotnet pack --configuration Release
```

## 📄 授權

本專案使用 MIT 授權條款 - 詳見 [LICENSE](LICENSE) 檔案。