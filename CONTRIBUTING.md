# Contribuindo

O projeto está em fase de documentação e ainda não contém implementação. As convenções abaixo orientam contribuições futuras.

## Estrutura proposta

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
docs/
```

- `Mcp/Tools`: contratos e registro das ferramentas MCP; sem detalhes HTTP do MFIT.
- `Application`: casos de uso, validação e coordenação.
- `Domain`: modelos internos mínimos e regras independentes da integração.
- `Integrations/Mfit`: adaptador externo substituível e seus mapeamentos.
- `tests`: testes com dados inventados, sem credenciais ou respostas reais.

A estrutura é uma proposta, não uma obrigação de criar projetos ou camadas desnecessárias.

## Convenções C#

- Alvo previsto: .NET 8; usar o SDK oficial MCP para C# e transporte `stdio`.
- Seguir o estilo e os analisadores configurados no repositório quando forem adicionados.
- Preferir tipos explícitos, nomes claros e métodos pequenos com responsabilidades únicas.
- Manter o contrato MCP independente de detalhes internos da integração MFIT.
- Evitar dependências de bibliotecas sem necessidade demonstrada.
- Não implementar manualmente JSON-RPC ou o framing do transporte.

## Fluxo de contribuição

1. Explique o problema e confirme que a alteração está dentro do escopo aprovado.
2. Para mudanças na integração, registre a evidência e valide autorização, autenticação e comportamento antes de depender de uma rota.
3. Faça alterações pequenas, preservando documentação e decisões já válidas.
4. Adicione testes usando fixtures totalmente fictícias quando houver código.
5. Atualize os documentos afetados quando requisitos, fluxo, segurança ou decisões mudarem.

## Dados e credenciais

- Nunca inclua nomes, telefones, IDs, fotos, URLs de perfil, respostas reais, cookies, tokens ou cabeçalhos de autorização em commits, testes, capturas, logs ou issues.
- Use somente exemplos sintéticos claramente fictícios, sem derivá-los de registros reais.
- Não persista dados de alunos sem requisito aprovado e análise de privacidade.
- Não adicione ferramentas de criação, edição ou exclusão ao MVP.
- Não trate uma chamada observada no navegador como API oficial, estável ou autorizada sem evidência independente.
