# MFIT Personal MCP

Servidor MCP local, planejado em C#/.NET 8, para oferecer ao usuário ferramentas controladas de consulta à interface autenticada do MFIT.

## Status atual

Documentação inicial. A integração, o ciclo de autenticação e a autorização para automação ainda precisam ser validados. Não há implementação neste estágio.

## Objetivo do MVP

Permitir buscar alunos por nome em modo somente leitura, com transporte local `stdio`. A resposta inicial deve conter apenas `id`, `nome` e, se necessário, o nome do grupo. Telefone e foto ficam fora do MVP.

## Escopo

- Validar as premissas técnicas, de autenticação e de uso antes de integrar.
- Usar o SDK oficial MCP para C# e manter a integração MFIT atrás de um adaptador substituível.
- Minimizar os dados pessoais consultados, retornados e eventualmente mantidos em memória.
- Não oferecer operações para criar, editar ou excluir dados.

## Documentos

- [PRD](docs/PRD.md): problema, requisitos, fluxo e questões abertas.
- [Arquitetura](docs/ARCHITECTURE.md): componentes, limites e hipótese de integração.
- [Roadmap](docs/ROADMAP.md): fases e critérios de conclusão.
- [Segurança](docs/SECURITY.md): credenciais, privacidade e tratamento de falhas.
- [Contribuição](CONTRIBUTING.md): estrutura proposta e práticas para contribuições.
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

## ⚖️ Aviso Legal (Legal Disclaimer)

> **Leia esta seção antes de usar este projeto.**

### 1. Independência e Ausência de Afilição Oficial

Este projeto é **independente** e **não possui qualquer afiliação, endosso, patrocínio ou ligação oficial** com a marca **MFIT Personal** ou com qualquer outra empresa, produto ou serviço mencionado no repositório.

- O código foi desenvolvido de forma autônoma, sem acesso a APIs oficiais fornecidas pela MFIT Personal.
- Qualquer referência a "MFIT Personal" no projeto tem o único propósito de descrever o **escopo de integração** que o software busca atender, e não implica qualquer vínculo comercial, técnico ou legal com a marca.
- O nome "MFIT Personal" é uma marca registrada pertencente a seus respectivos proprietários, e o uso dele aqui é meramente descritivo, conforme permitido pela legislação de marcas registradas.

### 2. Uso por Conta e Risco do Usuário

- O software é fornecido **"como está"** (*as-is*), sem garantias de qualquer espécie, expressas ou implícitas.
- O usuário assume **integral responsabilidade** pelo uso do projeto, inclusive no que diz respeito a:
  - Conformidade com os **termos de serviço** da plataforma MFIT Personal.
  - Respeito às **direitos de propriedade intelectual** e de proteção de dados dos usuários finais.
  - Cumprimento de **legislações locais** (LGPD, GDPR, etc.) aplicáveis ao tratamento de dados.
- O projeto **não coleta, armazena nem transmite dados de usuários finais** sem explícita autorização, mas o usuário é responsável por garantir que qualquer integração ou automação que desenvolva siga as práticas de privacidade adequadas.

### 3. Limitação de Responsabilidade

- Os autores e contribuidores deste projeto **não se responsabilizam** por danos diretos, indiretos, incidentais, especiais ou consequenciais resultantes do uso ou da incapacidade de uso do software.
- O projeto é destinado a **fins de estudo, pesquisa e integração pessoal**. Qualquer uso em ambiente de produção ou comercial é de exclusiva responsabilidade do usuário.

### 4. Integração Não Oficial

- Como o projeto opera sem uma API oficial, a integração pode depender de técnicas de **scraping, automação web ou其它 meios não contemplados pela plataforma**.
- O usuário deve estar ciente de que tais técnicas podem violar os termos de serviço da MFIT Personal, resultando em bloqueio, suspensão de acesso ou ações legais.
- Este projeto **não garante** a disponibilidade, estabilidade ou segurança de qualquer integração com a plataforma MFIT Personal.

### 5. Licença

Este projeto é regido pela licença apresentada no arquivo correspondente, e é destinado **exclusivamente a fins de estudo e integração pessoal**, não se configurando em qualquer produto comercial ou serviço de terceiros.

---

## 📄 Licença
Este projeto é para fins de estudo e integração pessoal.
