# MfitMcpServer

Servidor MCP (Model Context Protocol) escrito em C# .NET 8/10 para integração com a plataforma MFIT Personal.

## 🚀 Objetivo
Permitir que agentes de IA (como o Claude) interajam com dados da plataforma MFIT Personal para consultar alunos, planos e treinos, mesmo diante da ausência de uma API oficial.

## 🏗️ Arquitetura
O projeto segue os princípios da **Clean Architecture**:

- **MfitMcpServer.Domain:** Entidades de negócio, exceções e interfaces base.
- **MfitMcpServer.Application:** Casos de uso, DTOs e lógica de orquestração.
- **MfitMcpServer.Infra:** Implementação do acesso aos dados (Scrapers, HTTP Clients, Automação).
- **MfitMcpServer.Api:** Ponto de entrada do Servidor MCP (Console Application).

## 🛠️ Tecnologias
- .NET 8/10
- MCP SDK (Protocolo de comunicação com a IA)
- Playwright (Futura implementação para automação web)
- Microsoft.Extensions.DependencyInjection

## 📂 Estratégia de Integração
Para detalhes sobre como lidamos com a falta de API oficial, consulte o arquivo [ESTRATEGIA_MFIT.md](./ESTRATEGIA_MFIT.md).

## ⚙️ Como executar (Em breve)
```bash
dotnet run --project src/MfitMcpServer.Api
```

## 📄 Licença
Este projeto é para fins de estudo e integração pessoal.
