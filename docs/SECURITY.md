# Segurança e privacidade

<<<<<<< HEAD
## Princípios

- Dados de alunos são informações pessoais. Consultar e expor somente o estritamente necessário para a busca.
- O MVP é somente de leitura: não registrar ferramentas ou caminhos de escrita.
- A observação de uma rota no DevTools não comprova que seu uso seja autorizado ou oficialmente suportado. Validar as regras aplicáveis antes de integrar.
- Não incluir credenciais, cookies, tokens, cabeçalhos de autorização ou dados reais em código, documentação, testes, exemplos ou issues.

## Autenticação e segredos

- Não há credenciais fornecidas ou documentadas neste projeto.
- Definir o mecanismo de autenticação somente após validar um caminho permitido e o ciclo de sessão.
- Segredos devem ser fornecidos fora do repositório por um mecanismo local apropriado ao sistema operacional; não devem ser aceitos como argumentos de ferramenta MCP, gravados em arquivos versionados, exibidos na saída ou registrados em logs.
- Não copiar cookies ou tokens de sessões do navegador como solução implícita. Qualquer abordagem deve ser avaliada quanto à autorização, expiração, revogação e proteção em repouso.
- Erros de autenticação devem ser explícitos e não devem acionar tentativas de contornar controles ou reautenticar silenciosamente.

## Minimização e retenção

- A saída inicial deve conter apenas `id`, `nome` e, quando necessário para desambiguação, o nome do grupo.
- Não retornar telefone, foto, URL de perfil ou status no MVP.
- Não persistir consultas ou resultados. Manter dados em memória apenas pelo tempo necessário para a resposta.
- Não adicionar cache sem uma justificativa e uma decisão explícita sobre expiração, armazenamento e limpeza.
- Não registrar termos de busca, nomes ou payloads. Métricas devem ser agregadas e não identificáveis.

## Logs e erros

- Nunca registrar segredos, tokens, cookies, cabeçalhos de autorização ou respostas completas da integração.
- Preferir códigos/categorias de erro e metadados operacionais não pessoais.
- Não transformar timeout, falha de autenticação ou resposta inválida em resultado vazio com aparência de sucesso.
- Proteger também mensagens de exceção: elas podem conter URLs, parâmetros ou conteúdo externo.
- Como `stdio` é o canal MCP, mensagens de diagnóstico devem ser direcionadas a um canal apropriado e nunca misturadas ao protocolo.

## Transporte e permissões

- O transporte previsto é local `stdio`; não abrir porta de rede no MVP.
- A integração externa deve usar somente o método aprovado após validação das regras aplicáveis.
- Não contornar controles de acesso, limites, mecanismos anti-automação ou autenticação.
- Validar e limitar a entrada da ferramenta para evitar consultas acidentais ou excessivamente amplas.

## Tratamento de falhas

- Propagar falhas de autenticação, conectividade, limite e formato de resposta de forma clara ao cliente MCP.
- Não repetir indefinidamente chamadas que falharam. Política de repetição, se necessária, deve respeitar limites e ser definida após entender o serviço.
- Não tentar automaticamente um endpoint alternativo ou fluxo de autenticação não documentado.
- Reduzir a exposição de dados em qualquer mensagem de erro.

## Separação entre leitura e escrita

- A ferramenta única do MVP é uma consulta de alunos por nome.
- Não incluir clientes HTTP ou métodos de aplicação de escrita sem um escopo futuro aprovado separadamente.
- Preservar a separação arquitetural para que a integração não permita mutações acidentais; testes e revisão devem verificar a ausência de operações de escrita.
=======
## Leitura e sessão

- A sessão autenticada do navegador permanece no diretório local indicado por
  `MFIT_PROFILE_DIR` ou em `%LOCALAPPDATA%\MfitMcpServer\profile`. Esse diretório
  não deve ser versionado nem compartilhado.
- O token Authorization é capturado do tráfego do próprio painel, guardado
  somente em memória e reutilizado pelo cliente HTTP durante o processo. Não o
  registrar em arquivo, log, argumento de ferramenta ou saída do protocolo.
- Logs de diagnóstico vão para `stderr` e se limitam a status HTTP, página e
  quantidade de itens. Nunca registrar tokens, headers, payloads, nomes,
  telefones, fotos ou outros dados pessoais.
- A ferramenta de consulta retorna somente `Id`, `Nome` e `Plano`. O cache é
  somente em memória, expira após dez minutos e não armazena resultado vazio.
- Erros HTTP, JSON inválido e formatos inesperados devem ser explícitos; não
  devem ser convertidos em respostas vazias com aparência de sucesso.

## Escrita (pós-MVP)

- Toda operação de escrita exige prévia dos dados e confirmação explícita antes
  de alterar qualquer informação.
- Fazer backup dos dados afetados antes de cada alteração.
- Não implementar exclusão de dados.
- Desenvolver e validar operações de escrita somente em conta de teste.
- Não incluir dados pessoais em logs, mensagens de erro, fixtures, documentação
  ou commits.
- Respeitar os limites de acesso do MFIT; evitar chamadas excessivas e retries
  automáticos que possam aumentar a carga ou repetir uma escrita.
- Decisão em aberto para a escrita: usar a API interna observada no painel ou
  contratar o conector oficial pago do MFIT. Não iniciar escrita antes dessa
  decisão e da revisão das salvaguardas acima.
>>>>>>> feat/base-consolidada
