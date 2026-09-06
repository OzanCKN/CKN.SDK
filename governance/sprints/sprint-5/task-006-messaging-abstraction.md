---
Title: "Task 006: Provider-Agnostic Messaging Abstractions"
Module: CKN.Sdk.Messaging
Date: 2026-09-06
Status: TODO
Sprint: Sprint 5
---

# Özellik: EventBus Soyutlaması ve Multi-Provider Mesajlaşma

## Açıklama
Mevcut durumda CKN.SDK mesajlaşma altyapısı olarak sıkı sıkıya MassTransit'e bağlıdır. Yeni Multi-Provider mimarisi gereği, geliştiricilerin direkt RabbitMQ, Kafka veya Azure Service Bus Native SDK'larını kullanabilmesine olanak tanıyacak `IEventBus` soyutlaması `CKN.Sdk.Core` katmanına eklenecektir. Daha sonrasında ilk provider olan `CKN.Sdk.Messaging.RabbitMQ` yazılacaktır.

## Kabul Kriterleri (Acceptance Criteria)
1. `IEventBus`, `IIntegrationEvent`, `IEventHandler<T>` arayüzleri `CKN.Sdk.Core`'a eklenecek.
2. Glossary ve README dokümanlarına eklenecek (Active Maintenance kuralı).
3. Soyutlama (Abstraction) mimarisinin birim testleri (TDD) yazılacak.
4. Yeni modüller için Conventional Commits (`feat: add messaging abstractions`) kullanılacak.
5. SonarQube kuralı (0 Uyarı) devam ettirilecek.

## Alt Görevler (Sub-tasks)
- [x] Core Katmanı: EventBus interfaceleri.
- [x] Core Katmanı: Testlerin yazılması.
- [x] CKN.Sdk.Messaging projesinin (abstraction) oluşturulması.
- [x] Dokümantasyon (Glossary) güncellemeleri.
