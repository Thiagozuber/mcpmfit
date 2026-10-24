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
