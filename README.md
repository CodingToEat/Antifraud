# 💳 Transactions & Antifraud Microservice Challenge

Este repositorio contiene una solución compuesta por dos microservicios desarrollados en .NET 8 organizados en capas:

- **Transactions**: API RESTful para registrar, consultar y actualizar transacciones bancarias.
- **Antifraud**: Worker que escucha eventos desde Kafka y aplica lógica antifraude de forma desacoplada.

## 🧱 Estructura del repositorio

```
src/
├── Transactions/
│   ├── Transactions.Api/             # Capa de presentación
│   ├── Transactions.Application/     # Casos de uso y puertos
│   ├── Transactions.Domain/          # Entidades y lógica de negocio
│   └── Transactions.Infrastructure/  # EF Core, Kafka producer, repositorios
│
├── Antifraud/
│   ├── Antifraud.Worker/             # Worker .NET
│   ├── Antifraud.Application/        # Lógica antifraude y servicios
│   ├── Antifraud.Domain/             # Entidades y mensajes
│   └── Antifraud.Infrastructure/     # Persistencia y adaptadores Kafka
│
└── docker-compose.yml                # Orquesta todos los servicios
```

## 🚀 Cómo ejecutar el proyecto

### Requisitos

- [.NET 8 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)
- [Docker Desktop](https://www.docker.com/products/docker-desktop)

### Opción 1: Solo levantar servicios externos

```bash
docker compose up postgres kafka zookeeper
```
Luego en dos terminales diferentes:

```bash
# Terminal 1
cd src/Transactions/Transactions.Api
dotnet run

# Terminal 2
cd src/Antifraud/Antifraud.Worker
dotnet run
```

### Opción 2: Ejecutar todo con Docker Compose

```bash
docker compose up --build
```

Esto levanta:
- PostgreSQL en `localhost:5432`
- Kafka en `localhost:9092`
- Transactions API en `http://localhost:5243`
- Antifraud Worker en background

## 🧪 Endpoints principales

### Crear transacción

```http
POST http://localhost:5243/transactions
Content-Type: application/json

{
  "sourceAccountId": "GUID",
  "targetAccountId": "GUID",
  "transferTypeId": 1,
  "value": 1000
}
```

### Consultar transacción

```http
GET http://localhost:5243/transactions/{externalId}
```

### Actualizar estado (usado por el Worker)

```http
PUT http://localhost:5243/transactions/{externalId}/status
Content-Type: application/json

{
  "status": "Approved" | "Rejected"
}
```

## 🔐 Lógica antifraude

El servicio `Antifraud.Worker` consume eventos desde Kafka y aplica las siguientes reglas:

- ❌ Rechaza transacciones mayores a 2000
- ❌ Rechaza si el total diario del usuario supera 20000
- ✅ Si aprueba, actualiza el acumulado diario por usuario

## 🛠 Arquitectura

Se sigue una estructura hexagonal / DDD:

- `Application`: casos de uso, interfaces (puertos)
- `Infrastructure`: implementaciones (adaptadores), EF Core, Kafka
- `Domain`: entidades y reglas puras
- `Api` / `Worker`: entrada principal, endpoints o ejecución

## 📦 Migraciones y configuración

- Las migraciones de EF Core se aplican automáticamente con `.Migrate()`
- `Transactions.Api` expone la base de datos principal
- `Antifraud.Worker` mantiene su propia tabla `DailyTransactionLimit`

## ⚙️ Variables de entorno y configuración

- Se usan `appsettings.json` (para producción/docker) y `appsettings.Development.json` (para desarrollo local)
- La URL de `TransactionsApi:BaseUrl` y el `Kafka:BootstrapServers` se resuelven desde configuración

## 🧪 Tests

- `Antifraud.Worker.Tests` incluye pruebas unitarias para `FraudDetectionService`

## 📌 Notas adicionales

- El mensaje enviado al topic `transactions-created` es un `TransactionCreatedMessage`
- El mismo contrato es consumido por `Antifraud.Worker`
- Se evita pasar entidades al bus o entre capas; se usan DTOs y mensajes explícitos

---

¡Gracias por revisar esta solución!
