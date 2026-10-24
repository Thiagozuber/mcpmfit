# Arquitetura — MFIT Personal MCP

## Princípios

- Servidor local em C#/.NET 8, comunicando-se com clientes MCP por `stdio`.
- Uso do SDK oficial MCP para C#; não implementar JSON-RPC ou framing manualmente.
- Fronteiras claras entre protocolo MCP, lógica da aplicação e integração MFIT.
- Integração externa substituível e isolada atrás de uma abstração interna.
- Somente leitura no MVP e retorno mínimo de dados pessoais.

## Componentes propostos

```text
Cliente MCP
    │ stdio
    ▼
Host MCP (.NET 8 + SDK oficial)
    │
    ▼
Ferramenta de busca ──► Serviço de aplicação
                              │
                              ▼
                    Interface de consulta
                              │
                              ▼
                     Adaptador MFIT
                              │
                              ▼
                 Interface autenticada MFIT
```

### Responsabilidades

- **Host MCP:** configurar o SDK e o transporte local `stdio`, registrar ferramentas e controlar o ciclo de vida.
- **Ferramenta MCP:** definir nome, descrição, esquema de entrada/saída e validação de fronteira. Não deve conter detalhes HTTP ou MFIT.
- **Serviço de aplicação:** aplicar regras da busca e coordenar a consulta e o mapeamento do resultado.
- **Interface de consulta:** declarar a operação necessária à aplicação, sem acoplar o contrato à rota externa.
- **Adaptador MFIT:** encapsular autenticação, comunicação e interpretação da fonte externa, somente após validar método autorizado e suportado.
- **Modelos internos:** representar somente campos necessários à operação e à saída mínima.

## Fluxo de dados

1. O cliente MCP envia a solicitação pelo canal `stdio`.
2. O SDK direciona a invocação para a ferramenta registrada.
3. A ferramenta valida o termo e chama o serviço de aplicação.
4. O serviço usa a interface de consulta; o adaptador realiza a integração validada.
5. A resposta externa é validada e mapeada para `id`, `nome` e grupo opcional.
6. O SDK envia o resultado pelo canal `stdio`.

Falhas de autenticação, rede e formato devem chegar ao cliente como erros explícitos, em vez de resultados vazios com aparência de sucesso. Os logs operacionais devem evitar conteúdo de solicitação e resposta que possa identificar alunos.

## Organização de código proposta

```text
src/
  MfitPersonalMcp/
    Program.cs
    Mcp/
      Tools/
    Application/
    Domain/
    Integrations/
      Mfit/
tests/
  MfitPersonalMcp.Tests/
```

Esta é uma proposta para orientar a implementação futura, não uma estrutura já criada ou uma decisão sobre projetos .NET adicionais. O adaptador deve depender da interface da aplicação, evitando que tipos ou rotas MFIT vazem para o contrato MCP.

## Hipótese de integração observada

Em uma sessão autenticada real, o DevTools mostrou um `GET` para `/v2/user/client/` com os parâmetros `status`, `page`, `pageSize` e `search`. A interface pareceu consultar novamente conforme o usuário digitava; a resposta observada continha `items` e campos como `id`, `nome`, `status`, `telefone`, `profileUrl` e `grupo`.

Isto é evidência limitada ao comportamento observado naquela sessão. **Não** estabelece que a rota seja uma API pública, estável, oficialmente suportada ou autorizada para automação. Não se deve codificar uma dependência dessa rota antes de investigar regras aplicáveis, autenticação, significado dos parâmetros, paginação e alternativa oficialmente suportada. A rota é uma hipótese encapsulada no adaptador, não parte do contrato da aplicação ou MCP.

## Decisões e pendências

- **Decidido:** SDK oficial MCP para C# e transporte local `stdio`.
- **Decidido:** integração MFIT isolada e saída mínima.
- **Pendente:** autenticação, origem autorizada dos dados, semântica de paginação/status, limites e estabilidade da integração.
- **Pendente:** formato final do protocolo de erros e requisitos de distribuição local, a detalhar durante a implementação após validação.
