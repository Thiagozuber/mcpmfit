# Estratégia Técnica: MFIT MCP Server

Este documento detalha a abordagem técnica para superar a ausência de uma API pública oficial da plataforma MFIT Personal.

## Contexto
O MFIT Personal não oferece endpoints oficiais para integração de terceiros. Para construir um servidor MCP funcional, precisamos de uma estratégia de captura de dados robusta e modular.

## Pilares da Estratégia

### 1. Arquitetura Modular (Clean Architecture)
A estrutura do projeto foi desenhada para separar o **o que** o sistema faz (Domain/Application) do **como** ele obtém os dados (Infra).
- **Domain:** Define as entidades (Aluno, Treino, Plano) e as interfaces (`IMfitService`).
- **Infra:** Conterá as implementações reais que lidam com a plataforma MFIT.

### 2. Formas de Captura de Dados (Abordagem Evolutiva)

#### Nível A: Interceptação de Requisições (Reverse Engineering)
A aplicação web do MFIT se comunica com um backend via chamadas REST internas. Nossa primeira estratégia será:
- Identificar os endpoints internos usados pelo dashboard.
- Mapear os cabeçalhos de autenticação (Bearer Tokens, Cookies).
- Replicar essas chamadas usando `HttpClient` no .NET.

#### Nível B: Automação via Browser (Playwright)
Caso o MFIT utilize proteções complexas (WAF, JS Challenge) que dificultem chamadas diretas via `HttpClient`:
- Utilizaremos o **Playwright for .NET** em modo *headless*.
- O servidor MCP iniciará uma sessão, fará o login automatizado e extrairá os dados diretamente do DOM ou interceptará o tráfego do browser controlado.

#### Nível C: Mocking e Simulação
Para desenvolvimento inicial e testes do protocolo MCP:
- Implementaremos um `InMemoryMfitRepository` que retorna dados simulados, permitindo validar a lógica de ferramentas do MCP sem depender da estabilidade da extração de dados.

### 3. Gestão de Sessão e Segurança
- Armazenamento seguro de credenciais (Variáveis de Ambiente).
- Cache de tokens de sessão para minimizar a frequência de logins e evitar bloqueios por comportamento suspeito.
- Implementação de *Retries* e *Políticas de Resiliência* (Polly) para lidar com mudanças repentinas na UI do MFIT que possam quebrar o scraper.

## Próximos Passos
1. Implementar o contrato `IMfitClient` na camada de Application.
2. Criar um Protótipo de Scraper na camada de Infra usando `HttpClient` para testar o login.
3. Configurar o servidor MCP na camada Api para expor as ferramentas (tools) de consulta de alunos e treinos.
