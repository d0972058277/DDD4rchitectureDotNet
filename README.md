TODO List:
- 重構程式碼當中的 Service Locator Anti Pattern
- 補充架構設計圖
- 嘗試使用 CAP 進行 IntegrationEvent 的 Pub/Sub
- 加入 Specification
- 使 Swagger 配合 Specification 的內容顯示
- 重構 MethodInfo.Invoke 的反射使用
- 加入 Rebox Pattern (用於 inbox 與 outbox 的 Retry Pattern)
- 減少 Outbox/Inbox 中 Message 的狀態變化次數，應該只需要最後一次
- 思考 IntegrationEventHandler 中是不是需要 UnitOfWork 來達到 Domain 範圍的 ACID