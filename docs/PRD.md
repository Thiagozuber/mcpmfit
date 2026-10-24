# PRD — MFIT Personal MCP

## Contexto

O MFIT Personal MCP será um servidor MCP local em C#/.NET 8. A intenção é permitir que um cliente MCP autorizado consulte alunos pelo nome, sem alterar dados no MFIT.

### Fatos observados

- Em uma sessão autenticada real, o DevTools mostrou a interface fazendo uma chamada `GET` para `/v2/user/client/`.
- Naquela sessão, a chamada incluiu os parâmetros `status`, `page`, `pageSize` e `search`, e a interface pareceu consultar novamente conforme o usuário digitava.
- A resposta observada continha uma lista `items`, com campos como `id`, `nome`, `status`, `telefone`, `profileUrl` e `grupo`.
- Esta observação comprova somente o comportamento visto naquela sessão. Não comprova que a rota seja pública, estável, oficialmente suportada ou autorizada para automação.

### Decisões atuais

- O MVP é de leitura e busca por nome.
- A interface inicial da ferramenta retorna somente `id`, `nome` e, se necessário para distinguir resultados, o nome do grupo.
- Telefone e foto não serão usados nem retornados pelo MVP.
- O transporte local será `stdio`, usando o SDK oficial MCP para C#.
- A integração MFIT ficará isolada atrás de uma interface/adaptador substituível.

### Hipóteses e pontos a validar

- A rota observada pode ou não ser adequada para uso pelo servidor.
- O significado e os valores aceitos para `status`, a paginação, limites e ordenação ainda são desconhecidos.
- O ciclo de autenticação, a validade da sessão e as regras aplicáveis à automação ainda precisam de investigação.
- Não há credenciais fornecidas ou documentadas.

## Problema

Usuários que trabalham com o MFIT podem precisar localizar rapidamente um aluno pelo nome usando um cliente compatível com MCP. Antes de construir a integração, é necessário validar um caminho seguro e autorizado para consulta, sem confundir uma observação de DevTools com uma API pública ou estável.

## Objetivos

- Disponibilizar uma ferramenta MCP local para busca de alunos por nome.
- Restringir o MVP a consultas de leitura.
- Retornar apenas os dados mínimos necessários para identificar resultados.
- Manter a integração com o MFIT substituível e testável sem dados reais.
- Validar autenticação, comportamento da consulta e autorização antes de depender da rota observada.

## Fora de escopo

- Criar, editar, excluir ou matricular alunos; alterar grupos ou qualquer outro dado.
- Retornar telefone, foto, URL de perfil ou outros atributos não necessários à busca.
- Contornar autenticação, controles de acesso, limites, proteções ou mecanismos anti-automação.
- Tratar a rota observada como API oficial, pública ou garantida.
- Persistir resultados de alunos ou credenciais como parte do MVP.
- Implementar outras ferramentas, além da busca, antes de validação explícita.

## Usuário

Pessoa autenticada e autorizada a consultar alunos no MFIT, usando um cliente MCP local configurado para se conectar ao servidor. O usuário deve compreender que o servidor apenas consulta e que a disponibilidade da integração depende de validação de uso e autenticação.

## Histórias de usuário

- Como usuário autorizado, quero buscar alunos pelo nome para localizar uma pessoa sem abrir manualmente várias páginas.
- Como usuário, quero receber apenas os campos mínimos para reconhecer os resultados, reduzindo exposição de dados pessoais.
- Como operador, quero saber quando a autenticação expirou ou a consulta não pôde ser concluída, sem que o servidor invente resultados.
- Como mantenedor, quero substituir o adaptador MFIT sem alterar o contrato da ferramenta MCP.

## Critérios de aceite

- O servidor inicia localmente usando o transporte `stdio` do SDK oficial MCP para C#.
- O cliente MCP consegue descobrir e invocar uma ferramenta de busca por nome.
- A busca só executa após validação das premissas de uso e do mecanismo de autenticação.
- Resultados contêm `id`, `nome` e, apenas quando necessário, nome do grupo; não incluem telefone nem foto.
- Entradas inválidas e falhas de autenticação/rede são informadas claramente, sem resposta de sucesso vazia que mascare o erro.
- Não há ferramentas de criação, edição ou exclusão.
- Logs não incluem credenciais nem conteúdo pessoal dos resultados.

## Requisitos funcionais

- **RF-01:** expor uma ferramenta MCP para buscar alunos por nome.
- **RF-02:** validar que a consulta tenha um termo de busca utilizável antes de encaminhá-la ao adaptador.
- **RF-03:** converter os dados recebidos para o contrato mínimo de saída definido pelo servidor.
- **RF-04:** comunicar erros de autenticação, indisponibilidade e respostas incompatíveis de forma explícita.
- **RF-05:** não oferecer operações de escrita no MVP.

## Requisitos não funcionais

- **RNF-01 — Privacidade:** tratar todos os dados de alunos como informações pessoais e aplicar minimização.
- **RNF-02 — Segurança:** manter credenciais fora do código, dos argumentos MCP e dos logs.
- **RNF-03 — Isolamento:** separar contrato MCP, lógica de busca e integração externa.
- **RNF-04 — Transporte:** usar `stdio` local e o SDK oficial MCP para C#; não implementar JSON-RPC manualmente.
- **RNF-05 — Confiabilidade:** validar respostas externas e expor falhas, sem assumir estabilidade da rota observada.
- **RNF-06 — Testabilidade:** permitir testar a lógica e o mapeamento com dados inteiramente fictícios.

## Dados

| Dado | Uso no MVP | Observação |
|---|---|---|
| Termo de busca | Entrada da ferramenta | Evitar registrá-lo em logs por padrão. |
| `id` | Identificação do resultado | Retornar somente se necessário ao cliente MCP. |
| `nome` | Exibição e identificação | Dado pessoal; limitar ao resultado da busca. |
| Nome do grupo | Desambiguação opcional | Incluir apenas se necessário. |
| Status, telefone, foto e URL de perfil | Fora da saída inicial | Não retornar; necessidade futura exige nova validação. |

## Fluxo principal

1. O cliente MCP inicia o servidor local pelo transporte `stdio`.
2. O usuário invoca a ferramenta de busca com um nome ou trecho de nome.
3. O servidor valida a entrada e encaminha a consulta à interface interna de busca.
4. O adaptador MFIT realiza a consulta apenas depois que o método de integração e a autorização forem validados.
5. O servidor valida o formato recebido e mapeia os resultados para `id`, `nome` e grupo opcional.
6. O servidor devolve resultados mínimos ou comunica explicitamente a falha.

## Métricas

- Taxa de buscas concluídas com resposta válida.
- Latência de busca, sem armazenar o termo nem os resultados.
- Frequência de erros por categoria (por exemplo, autenticação, rede ou resposta incompatível), sem payload pessoal.
- Uso de memória e ausência de persistência de resultados.

As métricas devem ser locais e minimizadas; nenhuma telemetria externa é pressuposta.

## Questões abertas

- O uso automatizado da interface ou da rota observada é permitido pelas regras aplicáveis?
- Existe uma integração oficialmente suportada que deva ser preferida?
- Como autenticar sem copiar ou expor credenciais da sessão do navegador?
- O que significa `status` e quais valores são necessários para a busca?
- Como funcionam paginação, ordenação, limites e buscas sem correspondência?
- Qual é a duração da sessão e como expiração/reautenticação deve ser tratada?
- Em que casos o nome do grupo é necessário para desambiguar resultados?
- Qual distribuição e configuração local são esperadas para o servidor?