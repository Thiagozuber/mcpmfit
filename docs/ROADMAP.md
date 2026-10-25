<<<<<<< HEAD
# Roadmap — MFIT Personal MCP

As fases são sequenciais. Não avançar para uma integração operacional enquanto autorização e autenticação não estiverem suficientemente compreendidas.


 Fase 2 — Especificar autenticação e comportamento de consulta

**Entregáveis**

- Descrição do ciclo de autenticação autorizado, incluindo expiração e recuperação.
- Semântica confirmada de `status`, `page`, `pageSize`, `search`, ordenação e limites, caso a rota observada seja aprovada para uso.
- Contrato mínimo de dados e erros.

**Conclusão quando**

- Credenciais podem ser fornecidas e protegidas sem serem copiadas para código, logs ou argumentos MCP.
- Paginação, resultado vazio, falhas e limites têm comportamento definido.

## Fase 3 — Implementar o host MCP local

**Entregáveis**

- Aplicação .NET 8 com SDK oficial MCP para C# e transporte `stdio`.
- Registro da ferramenta de busca e validação de entrada.
- Serviço de aplicação e interface de consulta independentes do adaptador concreto.

**Conclusão quando**

- Cliente MCP local descobre e invoca a ferramenta.
- Não há protocolo JSON-RPC implementado manualmente nem ferramentas de escrita.

## Fase 4 — Implementar e validar o adaptador de consulta

**Entregáveis**

- Adaptador somente de leitura para o mecanismo aprovado na Fase 1.
- Validação da resposta externa e mapeamento para `id`, `nome` e grupo opcional.
- Tratamento explícito de autenticação, rede, limites e respostas inesperadas.

**Conclusão quando**

- A integração funciona dentro das regras aplicáveis e respeita o ciclo de autenticação documentado.
- Telefone, foto, URL de perfil e campos desnecessários não são retornados nem persistidos.

## Fase 5 — Entregar o MVP de busca

**Entregáveis**

- Pacote/configuração de execução local e instruções de conexão ao cliente MCP.
- Cobertura de testes com respostas fictícias e documentação operacional mínima.

**Conclusão quando**

- Os critérios de aceite do [PRD](PRD.md) são atendidos.
- Uma busca por nome funciona de ponta a ponta; erros são claros e logs não contêm credenciais ou dados de alunos.
- Nenhuma operação de criação, edição ou exclusão está disponível.
=======
# Roadmap

## Estado atual

O MVP de consulta está em consolidação. `listar_alunos` consulta a API interna usada
pelo painel MFIT e retorna somente `Id`, `Nome` e `Plano` (nome do grupo, com
fallback `Padrão`). A autenticação é obtida da sessão persistente do navegador e
mantida em memória durante o processo. O servidor ainda implementa o protocolo
MCP manualmente; a adoção do SDK oficial permanece pendente.

## Fases

| Fase | Escopo | Estado |
| --- | --- | --- |
| 1–5 | Estrutura do servidor, integração de leitura inicial e validação do MVP | Concluídas conforme o estado do projeto |
| 6 | Consolidar a base: segurança, cliente HTTP, paginação, cache em memória, testes e documentação | Atual |
| 7 | Ler treinos de um aluno, sem escrita | Pós-MVP |
| 8 | Consultar a biblioteca de exercícios, sem escrita | Pós-MVP |
| 9 | Criar treino com prévia e confirmação explícita | Pós-MVP |
| 10 | Edição pontual com backup antes da alteração e sem exclusão | Pós-MVP |

Nenhuma ferramenta de criação, edição ou exclusão faz parte da fase 6. As fases
de escrita dependem de validação de segurança, testes em conta de teste e
decisão sobre a integração de escrita documentada em [SECURITY.md](./SECURITY.md).
>>>>>>> feat/base-consolidada
