# Event-Driven Financial Transactions Platform

MoneyFellows hiring quest: two .NET microservices (**Transactions**, **Payments**) with Clean Architecture, DDD, and event-driven workflow over RabbitMQ.

## Flow (high level)

1. **Create transaction** → **Add items** → **Submit** (Transactions API).
2. **Submit** publishes **TransactionSubmitted** → Payments service consumes it and **starts payment** (creates payment intent). In production the user would then be sent to the payment gateway.
3. **Confirm** and **Fail** (Payments API) **simulate the payment gateway callback** (success / failure). In production these would be invoked by the real gateway (webhook or redirect).

## Repository structure

| Path | Description |
|------|-------------|
| `src/Services/Transactions/` | Transactions service (create, add items, submit, cancel) |
| `src/Services/Payments/` | Payments service (start, confirm, fail) |
| `src/BuildingBlocks/` | Shared abstractions, messaging contracts, correlation |
| `docker-compose.yml` | SQL Server, RabbitMQ, both APIs |

## Documentation

- **[DOMAIN.md](DOMAIN.md)** — Bounded contexts, aggregates, invariants, event flow (with diagrams).
- **[ARCHITECTURE.md](ARCHITECTURE.md)** — Clean Architecture, cross-service events, reliability (outbox, correlation), observability.

## Run locally

1. **Docker Compose** (recommended):

   ```bash
   docker-compose up -d
   ```

   Then open Transactions API and Payments API (ports per `docker-compose.override.yml`).

2. **Or run APIs from IDE** with SQL Server and RabbitMQ already running (e.g. from compose).

## Main endpoints

**Transactions**

- `POST /transactions` — create
- `POST /transactions/{id}/items` — add items
- `POST /transactions/{id}/submit` — submit (publishes **TransactionSubmitted** → payment is started by Payments service)
- `POST /transactions/{id}/cancel` — cancel (draft or submitted)

**Payments**

- `POST /payments/start` — start payment for a transaction (also triggered automatically when Transactions submits)
- `POST /payments/{id}/confirm` — **simulates payment gateway success callback** (publishes **PaymentConfirmed**)
- `POST /payments/{id}/fail` — **simulates payment gateway failure callback** (publishes **PaymentFailed**)

## Tech stack

- .NET 10 / ASP.NET Core  
- SQL Server, EF Core  
- RabbitMQ, MassTransit  
- Outbox pattern (Transactions), CorrelationId (HTTP → events → logs)  
- Serilog, Grafana Loki (optional), health checks, OpenAPI  

## Video submission

See **[VIDEO_SCRIPT.md](VIDEO_SCRIPT.md)** for the 10-minute video structure (intro + one backend challenge + live demo + event reliability).
