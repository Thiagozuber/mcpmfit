# Segurança e privacidade

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
