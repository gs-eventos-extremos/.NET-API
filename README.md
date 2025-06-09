# Weather Emergency API

[![.NET 8](https://img.shields.io/badge/.NET-8.0-blue.svg)](https://dotnet.microsoft.com/download)
[![Oracle](https://img.shields.io/badge/Database-Oracle-red.svg)](https://www.oracle.com/)
[![OpenWeatherMap](https://img.shields.io/badge/API-OpenWeatherMap-orange.svg)](https://openweathermap.org/)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)

Uma API REST robusta para gerenciamento de alertas climáticos e contatos de emergência, desenvolvida em .NET 8 com integração a serviços meteorológicos, machine learning para predição de desastres naturais e sistema de mensageria assíncrona.

## 📋 Índice

- [Sobre o Projeto](#sobre-o-projeto)
- [Características Principais](#características-principais)
- [Tecnologias Utilizadas](#tecnologias-utilizadas)
- [Arquitetura](#arquitetura)
- [Instalação](#instalação)
- [Configuração](#configuração)
- [Uso da API](#uso-da-api)
- [Endpoints](#endpoints)
- [Machine Learning](#machine-learning)
- [Rate Limiting](#rate-limiting)
- [Mensageria](#mensageria)
- [Autenticação](#autenticação)
- [Documentação da API](#documentação-da-api)
- [Testes](#testes)
- [Deploy](#deploy)
- [Contribuindo](#contribuindo)
- [Licença](#licença)

## 🌟 Sobre o Projeto

A **Weather Emergency API** é uma solução completa para monitoramento meteorológico e gestão de emergências climáticas. O sistema permite que usuários:

- Monitorem condições climáticas em tempo real
- Recebam alertas automáticos sobre condições meteorológicas perigosas
- Gerenciem localizações favoritas
- Cadastrem contatos de emergência
- Utilizem predições baseadas em machine learning para avaliar riscos de desastres naturais

## ✨ Características Principais

### 🌤️ Monitoramento Meteorológico
- **Clima Atual**: Dados em tempo real via OpenWeatherMap API
- **Previsão Estendida**: Previsão para os próximos 6 dias
- **Alertas Automáticos**: Notificações baseadas em condições críticas
- **Geocoding**: Conversão de endereços em coordenadas geográficas

### 🤖 Machine Learning
- **Predição de Desastres**: Algoritmos de ML para avaliar riscos meteorológicos
- **Detecção de Anomalias**: Identificação de padrões anômalos em dados históricos
- **Modelo Adaptativo**: Sistema que aprende com novos dados

### 🔐 Segurança e Performance
- **Autenticação JWT**: Sistema seguro de autenticação
- **Rate Limiting**: Controle de taxa de requisições por IP e cliente
- **Caching**: Cache inteligente para otimização de performance
- **HATEOAS**: Implementação completa de REST com hipermídia

### 📱 Integração e Escalabilidade
- **Mensageria Assíncrona**: RabbitMQ para processamento em background
- **Oracle Database**: Banco de dados enterprise-ready
- **Arquitetura Limpa**: Separação clara de responsabilidades
- **AutoMapper**: Mapeamento automático entre DTOs e Models

## 🛠️ Tecnologias Utilizadas

### Backend
- **.NET 8** - Framework principal
- **ASP.NET Core Web API** - Framework web
- **Entity Framework Core** - ORM
- **Oracle Database** - Banco de dados principal
- **AutoMapper** - Mapeamento objeto-objeto
- **BCrypt.NET** - Hash de senhas

### Machine Learning
- **Microsoft ML.NET** - Framework de machine learning
- **ML.NET TimeSeries** - Análise de séries temporais

### Infraestrutura
- **RabbitMQ** - Message broker
- **MassTransit** - Framework de mensageria
- **AspNetCoreRateLimit** - Rate limiting
- **MemoryCache** - Cache em memória

### APIs Externas
- **OpenWeatherMap API** - Dados meteorológicos
- **Geocoding API** - Conversão de endereços

### Documentação e Testes
- **Swagger/OpenAPI** - Documentação da API
- **Swashbuckle** - Geração automática de documentação

## 🏗️ Arquitetura

A aplicação segue os princípios da **Clean Architecture** com separação clara de responsabilidades:

```
WeatherEmergencyAPI/
├── Controllers/          # Controladores da API
├── Services/            # Lógica de negócio
├── Repositories/        # Acesso a dados
├── Models/              # Entidades do banco
├── DTOs/                # Data Transfer Objects
├── ML/                  # Machine Learning
├── Consumers/           # Consumidores de mensagens
├── Middleware/          # Middlewares customizados
├── Data/                # Contexto do banco
├── Configurations/      # Configurações
├── Utils/               # Utilitários
└── Messages/            # Mensagens do sistema
```

### Padrões Utilizados
- **Repository Pattern** - Abstração do acesso a dados
- **Service Layer** - Lógica de negócio encapsulada
- **Dependency Injection** - Inversão de controle
- **Unit of Work** - Controle de transações
- **CQRS** - Separação de comandos e consultas

## 🚀 Instalação

### Pré-requisitos
- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Oracle Database](https://www.oracle.com/database/)
- [RabbitMQ](https://www.rabbitmq.com/) (opcional para desenvolvimento)
- [Visual Studio 2022](https://visualstudio.microsoft.com/) ou [VS Code](https://code.visualstudio.com/)

### Clonando o Repositório
```bash
git clone https://github.com/seu-usuario/weather-emergency-api.git
cd weather-emergency-api
```

### Instalando Dependências
```bash
dotnet restore
```

### Configurando o Banco de Dados
```bash
# Executar migrations
dotnet ef database update
```

## ⚙️ Configuração

### appsettings.json

Configure o arquivo `appsettings.json` com suas credenciais:

```json
{
  "ConnectionStrings": {
    "OracleConnection": "User Id=SEU_USUARIO;Password=SUA_SENHA;Data Source=SEU_HOST:1521/ORCL"
  },
  "JwtSettings": {
    "SecretKey": "sua-chave-super-secreta-com-pelo-menos-256-bits-aqui",
    "Issuer": "WeatherEmergencyAPI",
    "Audience": "WeatherEmergencyApp",
    "ExpirationInHours": 24
  },
  "WeatherApi": {
    "OpenWeatherMapKey": "SUA_API_KEY_OPENWEATHERMAP",
    "OpenWeatherMapUrl": "https://api.openweathermap.org/data/2.5"
  },
  "RabbitMQ": {
    "Host": "localhost",
    "Port": 5672,
    "Username": "guest",
    "Password": "guest"
  }
}
```

### Variáveis de Ambiente (Produção)
```bash
export ConnectionStrings__OracleConnection="sua_connection_string"
export JwtSettings__SecretKey="sua_chave_jwt"
export WeatherApi__OpenWeatherMapKey="sua_api_key"
```

## 📊 Uso da API

### Iniciando a Aplicação
```bash
dotnet run
```

A API estará disponível em:
- **HTTP**: `http://localhost:5165`
- **HTTPS**: `https://localhost:7013`
- **Swagger**: `http://localhost:5165/swagger`

### Fluxo Básico de Uso

1. **Registrar Usuário**
```bash
POST /api/auth/register
{
  "name": "João Silva",
  "email": "joao@example.com",
  "password": "MinhaSenh@123"
}
```

2. **Fazer Login**
```bash
POST /api/auth/login
{
  "email": "joao@example.com",
  "password": "MinhaSenh@123"
}
```

3. **Adicionar Localização**
```bash
POST /api/users/{userId}/locations
Authorization: Bearer {token}
{
  "name": "Minha Casa",
  "latitude": -23.5505,
  "longitude": -46.6333,
  "city": "São Paulo",
  "state": "SP",
  "country": "Brasil",
  "isFavorite": true
}
```

4. **Obter Clima**
```bash
POST /api/weather/current
{
  "latitude": -23.5505,
  "longitude": -46.6333
}
```

## 🛠️ Endpoints

### 🔐 Autenticação
| Método | Endpoint | Descrição |
|--------|----------|-----------|
| `POST` | `/api/auth/register` | Registrar novo usuário |
| `POST` | `/api/auth/login` | Login do usuário |
| `PUT` | `/api/auth/update-password` | Atualizar senha |

### 👤 Usuários
| Método | Endpoint | Descrição |
|--------|----------|-----------|
| `GET` | `/api/users/{userId}` | Obter dados do usuário |
| `PUT` | `/api/users/{userId}` | Atualizar dados do usuário |
| `DELETE` | `/api/users/{userId}` | Deletar conta do usuário |

### 📍 Localizações
| Método | Endpoint | Descrição |
|--------|----------|-----------|
| `GET` | `/api/users/{userId}/locations` | Listar localizações |
| `POST` | `/api/users/{userId}/locations` | Criar localização |
| `GET` | `/api/users/{userId}/locations/{locationId}` | Obter localização |
| `PUT` | `/api/users/{userId}/locations/{locationId}` | Atualizar localização |
| `DELETE` | `/api/users/{userId}/locations/{locationId}` | Deletar localização |
| `GET` | `/api/users/{userId}/locations/favorites` | Localizações favoritas |
| `POST` | `/api/users/{userId}/locations/by-address` | Criar por endereço |

### 🌤️ Clima
| Método | Endpoint | Descrição |
|--------|----------|-----------|
| `POST` | `/api/weather` | Informações completas do clima |
| `POST` | `/api/weather/current` | Clima atual |
| `POST` | `/api/weather/forecast` | Previsão do tempo |
| `POST` | `/api/weather/alerts` | Alertas meteorológicos |
| `GET` | `/api/weather/location/{locationId}` | Clima por localização salva |

### 📞 Contatos de Emergência
| Método | Endpoint | Descrição |
|--------|----------|-----------|
| `GET` | `/api/users/{userId}/emergency-contacts` | Listar contatos |
| `POST` | `/api/users/{userId}/emergency-contacts` | Criar contato |
| `GET` | `/api/users/{userId}/emergency-contacts/{contactId}` | Obter contato |
| `PUT` | `/api/users/{userId}/emergency-contacts/{contactId}` | Atualizar contato |
| `DELETE` | `/api/users/{userId}/emergency-contacts/{contactId}` | Deletar contato |

### 🤖 Machine Learning
| Método | Endpoint | Descrição |
|--------|----------|-----------|
| `POST` | `/api/mlprediction/predict-disaster` | Predição de desastres |
| `POST` | `/api/mlprediction/detect-anomaly` | Detecção de anomalias |
| `GET` | `/api/mlprediction/model-info` | Informações do modelo |

### 📊 Sistema
| Método | Endpoint | Descrição |
|--------|----------|-----------|
| `GET` | `/api/health` | Status da API |
| `GET` | `/api/health/info` | Informações da API |
| `GET` | `/api/ratelimit/status` | Status do rate limit |
| `GET` | `/api/ratelimit/rules` | Regras de rate limit |

## 🤖 Machine Learning

### Predição de Desastres Naturais

O sistema utiliza **Microsoft ML.NET** para prever riscos de desastres naturais baseado em:

- **Temperatura** (-50°C a 60°C)
- **Umidade** (0% a 100%)
- **Pressão Atmosférica** (950 a 1050 hPa)
- **Velocidade do Vento** (0 a 300 km/h)
- **Precipitação** (0 a 500mm)

#### Exemplo de Uso:
```json
POST /api/mlprediction/predict-disaster
{
  "temperature": 42.5,
  "humidity": 85.0,
  "pressure": 995.0,
  "windSpeed": 90.0,
  "precipitation": 120.0
}
```

#### Resposta:
```json
{
  "hasHighRisk": true,
  "riskProbability": 0.87,
  "riskLevel": "Crítico",
  "recommendation": "ALERTA CRÍTICO: Procure abrigo imediatamente...",
  "potentialHazards": ["Calor extremo", "Ventos fortes", "Chuva intensa"],
  "predictionTime": "2024-01-15T10:30:00Z"
}
```

### Detecção de Anomalias

Sistema de detecção de anomalias em séries temporais meteorológicas:

```json
POST /api/mlprediction/detect-anomaly
{
  "historicalData": [
    {"timestamp": "2024-01-01T00:00:00Z", "value": 25.5},
    {"timestamp": "2024-01-02T00:00:00Z", "value": 24.8}
  ],
  "currentValue": 45.2
}
```

## 🛡️ Rate Limiting

O sistema implementa rate limiting em dois níveis:

### Por IP
- **30 requisições/minuto**
- **500 requisições/hora**
- **5000 requisições/dia**

### Por Cliente (Header X-ClientId)
- **60 requisições/minuto**
- **1000 requisições/hora**

### Endpoints Específicos
- **Login**: 10 tentativas/15 minutos
- **Registro**: 5 tentativas/hora
- **Clima**: 20 requisições/minuto

### Headers de Resposta
```
X-Rate-Limit-Limit: 30
X-Rate-Limit-Remaining: 25
X-Rate-Limit-Reset: 1640995200
```

## 📨 Mensageria

O sistema utiliza **RabbitMQ** com **MassTransit** para processamento assíncrono:

### Filas Implementadas

#### UserRegisteredMessage
Processamento após registro de usuário:
- Envio de email de boas-vindas
- Configurações iniciais
- Log de eventos

#### WeatherAlertMessage
Alertas meteorológicos:
- Notificação SMS/WhatsApp
- Push notifications
- Log de alertas

#### LogEventMessage
Sistema de logs centralizados:
- Agregação de logs
- Monitoramento de eventos
- Debugging distribuído

### Configuração do RabbitMQ

```json
{
  "RabbitMQ": {
    "Host": "localhost",
    "Port": 5672,
    "Username": "guest",
    "Password": "guest",
    "VirtualHost": "/"
  }
}
```

## 🔐 Autenticação

### JWT (JSON Web Tokens)

O sistema utiliza JWT para autenticação stateless:

#### Estrutura do Token
```json
{
  "sub": "123",
  "email": "user@example.com",
  "UserId": "123",
  "exp": 1640995200,
  "iss": "WeatherEmergencyAPI",
  "aud": "WeatherEmergencyApp"
}
```

#### Headers de Autenticação
```
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
```

#### Configuração de Segurança
- **Algoritmo**: HMAC SHA-256
- **Expiração**: 24 horas (configurável)
- **Refresh**: Implementação manual via re-login

## 📚 Documentação da API

### Swagger/OpenAPI

A documentação completa está disponível em:
- **Desenvolvimento**: `http://localhost:5165/swagger`
- **Produção**: `https://sua-api.com/swagger`

### Características da Documentação
- **Exemplos de Requisição/Resposta**
- **Esquemas de Dados Completos**
- **Teste Interativo de Endpoints**
- **Autenticação JWT Integrada**
- **Códigos de Status HTTP Documentados**

### HATEOAS (Hypermedia as the Engine of Application State)

Todas as respostas incluem links relacionados:

```json
{
  "id": 1,
  "name": "João Silva",
  "links": {
    "self": {"href": "/api/users/1", "method": "GET"},
    "update": {"href": "/api/users/1", "method": "PUT"},
    "locations": {"href": "/api/users/1/locations", "method": "GET"}
  }
}
```

## 🧪 Testes

### Executando Testes
```bash
# Testes unitários
dotnet test

# Testes com cobertura
dotnet test --collect:"XPlat Code Coverage"
```

### Estrutura de Testes
```
Tests/
├── Unit/
│   ├── Services/
│   ├── Controllers/
│   └── Repositories/
├── Integration/
│   ├── API/
│   └── Database/
└── Performance/
    └── LoadTests/
```

### Ferramentas de Teste
- **xUnit** - Framework de testes
- **Moq** - Mocking framework
- **FluentAssertions** - Assertions fluentes
- **TestContainers** - Testes de integração

## 🚀 Deploy

### Docker

```dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["WeatherEmergencyAPI.csproj", "."]
RUN dotnet restore
COPY . .
RUN dotnet publish -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "WeatherEmergencyAPI.dll"]
```

### Docker Compose

```yaml
version: '3.8'
services:
  api:
    build: .
    ports:
      - "5165:80"
    environment:
      - ASPNETCORE_ENVIRONMENT=Production
    depends_on:
      - rabbitmq
      
  rabbitmq:
    image: rabbitmq:3-management
    ports:
      - "5672:5672"
      - "15672:15672"
```

### Azure App Service

```bash
# Publicar para Azure
az webapp up --name weather-emergency-api --resource-group rg-weather
```

### Configurações de Produção

#### appsettings.Production.json
```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Warning",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "ConnectionStrings": {
    "OracleConnection": "#{ConnectionString}#"
  }
}
```

