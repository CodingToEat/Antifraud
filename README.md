# 💳 Transactions & Antifraud Microservice Challenge

Este repositorio contiene una solución compuesta por dos proyectos .NET 8 que implementan una arquitectura orientada a microservicios:

* **Transactions.Api**: expone endpoints para registrar, consultar y actualizar transacciones.
* **Antifraud.Worker**: escucha eventos Kafka y aplica lógica antifraude de manera desacoplada.

## 🧱 Estructura del repositorio

```
src/
├── Transactions.Api/        # API REST para registrar y consultar transacciones
├── Antifraud.Worker/        # Servicio de fondo que detecta fraudes
└── docker-compose.yml       # Orquesta base de datos, Kafka, API y Worker
```

## 🚀 Cómo ejecutar el proyecto

### Prerrequisitos

* [.NET 8 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)
* [Docker](https://www.docker.com/products/docker-desktop)

### Ejecución local sin Docker

```bash
# Ejecutar base de datos y Kafka desde Docker
docker-compose up postgres kafka zookeeper

# En terminal 1
cd src/Transactions.Api
dotnet run

# En terminal 2
cd src/Antifraud.Worker
dotnet run
```

### Ejecución con Docker Compose

```bash
docker-compose up --build
```

Esto levanta:

* PostgreSQL en `localhost:5432`
* Kafka en `localhost:9092`
* Transactions API en `http://localhost:5243`
* Antifraud Worker corriendo en background

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

El servicio `Antifraud.Worker` escucha transacciones creadas desde Kafka y aplica dos reglas:

* Rechaza transacciones mayores a **2000**
* Rechaza si el **total diario del usuario excede 20000**
* Si la transacción es aprobada, **registra el acumulado** en una tabla separada `DailyTransactionLimit`

## 🛠 Migraciones y configuración

Ambos proyectos aplican automáticamente las migraciones (`.Migrate()`) al iniciar.

### Configuración de entornos

* `appsettings.Development.json`: usado al depurar localmente
* `appsettings.json`: usado al correr en Docker

## 📦 Tests

En `Antifraud.Worker.Tests` se incluye cobertura de unidad para:

* `FraudDetectionService` (casos aprobados, rechazados, límites)

## 🧾 Dependencias

* PostgreSQL
* Apache Kafka + Zookeeper (confluentinc)
* .NET 8
* EF Core + Npgsql

## 📌 Notas

* El tópico `transactions-created` se crea automáticamente si no existe
* El worker se desacopla completamente del API: no accede a su base directamente, sino que mantiene su propia tabla de acumulado

---

¡Gracias por revisar esta solución!
