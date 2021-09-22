# my-microservices
.NET 5 Microservices project with Docker support.

** Swagger can be used to interact with the app

The app has 
* ApiGateway - Ocelot
* BFF Aggregator
* 4 HTTP Services
* 1 gRPC Service
* Message Broker - RabbitMQ
* Docker - Linux

### ApiGateway
* Port - 8010

### BFF Aggregator
* Port - 8005

## Services

### Catalog
* Port - 8000
* DB - Mongo

### Basket
* Port - 8001
* DB - Redis
* Interacts with 
  * Discount via gRPC to fetch discounts
  * Ordering(Consumer) via RabbitMQ to start order on basket/checkout

### Discount
* Discount.Api - HTTP
  * Port - 8002
* Discount.Grpc - gRPC
  * Port - 8003
* DB - Postgre via Dapper

### Ordering
* Follows - CQRS, Clean Architecture
* Port - 8004
* DB - MS SQL via EF Core
* Interacts with
  * Basket(Producer) via RabbitMQ to start order on basket/checkout
  
## Docker
* Use Docker-Compose for orchestration
* All DBs and DB-Managers are installed via docker
* Start Command
  * docker-compose -f .\docker-compose.yml -f .\docker-compose.override.yml up -d
* Stop Command
  * docker-compose -f .\docker-compose.yml -f .\docker-compose.override.yml down
