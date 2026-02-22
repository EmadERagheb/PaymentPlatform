# Domain Model — Event-Driven Financial Transactions

Two **bounded contexts**: **Transactions** and **Payments**. They communicate only via **integration events** over the message broker.

---

## Bounded contexts (high level)

```mermaid
flowchart LR
  subgraph Transactions["Transactions Context"]
    T[Transaction Aggregate]
  end
  subgraph Payments["Payments Context"]
    P[Payment Aggregate]
  end
  T -->|TransactionSubmitted| MQ[Message Broker]
  MQ -->|consumed by| Payments
  P -->|PaymentConfirmed / PaymentFailed| MQ
  MQ -->|consumed by| Transactions
```

---

## A) Transactions context

**Aggregate:** `Transaction` (root) + `TransactionItem` (entity).

| Invariant | Rule |
|----------|------|
| Submit | Only **Draft**; must have **≥1 items**. |
| Cancel | Not allowed if **Completed**; **idempotent** if already Cancelled. |
| Complete | Only from **Submitted** (after payment confirmed). |

**State machine:**

```mermaid
stateDiagram-v2
  [*] --> Draft
  Draft --> Submitted: Submit (emit TransactionSubmitted)
  Draft --> Cancelled: Cancel
  Submitted --> Cancelled: Cancel
  Submitted --> Completed: Complete (on PaymentConfirmed)
  Cancelled --> [*]
  Completed --> [*]
```

**Domain events (in-process):** `TransactionSubmitDomainEvent`.  
**Integration event (to broker):** `TransactionSubmittedEvent(TransactionId, Currency, Amount, CorrelationId)`.

---

## B) Payments context

**Aggregate:** `Payment` (root only).

| Invariant | Rule |
|----------|------|
| Start | Valid `TransactionId` and `Amount > 0`. |
| Confirm | Only from **Pending**. |
| Fail | Only from **Pending**; requires **reason**. |

**State machine:**

```mermaid
stateDiagram-v2
  [*] --> Pending
  Pending --> Confirmed: Confirm (emit PaymentConfirmed)
  Pending --> Failed: Fail (emit PaymentFailed)
  Confirmed --> [*]
  Failed --> [*]
```

**Domain events (in-process):** `PaymentConfirmedDomainEvent`, `PaymentFailedDomainEvent`.  
**Integration events (to broker):** `PaymentConfirmedEvent`, `FailedPaymentEvent` (with `CorrelationId` for tracing).

---

## Event flow (cross-context)

**Flow:** Create transaction → Add items → Submit → **Payment starts automatically** (event) → (in production: user goes to payment gateway) → **Confirm / Fail** = simulation of gateway callback (success / failure).

```mermaid
sequenceDiagram
  participant Client
  participant Transactions as Transactions API
  participant MQ as RabbitMQ
  participant Payments as Payments API

  Client->>Transactions: POST /transactions
  Client->>Transactions: POST /transactions/{id}/items
  Client->>Transactions: POST /transactions/{id}/submit
  Transactions->>Transactions: Transaction.Submit() → domain event
  Transactions->>MQ: TransactionSubmittedEvent
  MQ->>Payments: consume → StartPaymentCommand
  Payments->>Payments: Payment.Start() → Pending
  Note over Payments: In production: redirect to gateway
  Client->>Payments: POST /payments/{id}/confirm (simulates gateway success callback)
  Payments->>Payments: Payment.Confirm() → domain event
  Payments->>MQ: PaymentConfirmedEvent
  MQ->>Transactions: consume → CompleteTransactionCommand
  Transactions->>Transactions: Transaction.Complete() → Completed
```

- **Confirm** and **Fail** endpoints simulate the **payment gateway callback** (success vs failure). In production these would be called by the real gateway (e.g. webhook or redirect).

---

## Design decisions (short)

- **One aggregate per context** for this scope: clear boundaries and simple consistency.
- **Value objects:** `TransactionId`, `PaymentId`, `Money`, `Currency` to keep the model explicit and type-safe.
- **Integration events** carry only IDs and needed data (no internal entities); **CorrelationId** for end-to-end tracing.
- **Cancel** is explicit and idempotent; **Completed** is only set when Payments confirms, keeping payment as source of truth for “paid”.
