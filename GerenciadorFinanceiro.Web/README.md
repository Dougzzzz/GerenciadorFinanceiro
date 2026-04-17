# GerenciadorFinanceiro.Web

Frontend Angular do projeto de controle financeiro.

## Comandos

Para subir o front em desenvolvimento:

```bash
npm start
```

Para gerar build:

```bash
npm run build
```

Para rodar os testes em modo interativo:

```bash
npm test
```

Para rodar os testes uma vez em `ChromeHeadless`:

```bash
npm run test:ci
```

Para rodar os testes com cobertura:

```bash
npm run test:coverage
```

O relatório HTML de cobertura fica em `coverage/gerenciador-financeiro.web/index.html`.

## Estratégia de testes

Hoje o projeto usa `Jasmine + Karma`, que já vem integrado ao Angular CLI e tem o menor atrito para manter a base evoluindo.

Os testes seguem este modelo:

- `services`: validar chamadas HTTP, query params e transformação de dados
- componentes visuais: validar renderização, classes CSS e estados vazios
- componentes de formulário: validar `Input`, `Output`, habilitação de botão e reset
- componentes container: mockar services e validar carregamento, filtros e ações da tela

## Exemplos no projeto

Você pode usar estes arquivos como referência:

- `src/app/core/services/financeiro.service.spec.ts`
- `src/app/features/transacoes/transacoes.component.spec.ts`
- `src/app/features/transacoes/transacoes-form.component.spec.ts`
- `src/app/features/dashboard/dashboard.component.spec.ts`

## Fluxo recomendado para novos testes

1. Criar o arquivo `*.spec.ts` ao lado do componente ou service testado.
2. Cobrir primeiro o comportamento mais importante para o usuário ou regra de negócio.
3. Mockar dependências externas, principalmente `FinanceiroService`.
4. Rodar `npm run test:ci` antes de subir a alteração.
5. Quando quiser avaliar evolução da cobertura, rodar `npm run test:coverage`.
