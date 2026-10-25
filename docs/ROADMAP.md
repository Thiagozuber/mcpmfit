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
