# Implementação: Padronização de Endpoints e Testes de Regressão

Este documento detalha o plano de execução para corrigir os erros de comunicação entre o Frontend (Angular) e o Backend (.NET) e estabelecer uma rede de segurança com testes automatizados.

## 🛠️ 1. Padronização de DTOs e Controllers

Para resolver o erro `400 Bad Request` e as falhas de conversão de Enums:

### 1.1 Refatoração de DTOs
Todos os DTOs em `GerenciadorFinanceiro.Application/DTOs` serão convertidos de `record` para `class` com propriedades explícitas:
*   Utilizar `[JsonPropertyName]` para garantir que o JSON do Angular (camelCase) mapeie corretamente para as classes C#.
*   Converter campos de `Enum` para `int` para evitar falhas de parser no .NET.

### 1.2 Atualização de Controllers
Os controllers serão ajustados para:
*   Renomear parâmetros de `dto` para `dados` ou nomes específicos (ex: `dadosCartao`) para evitar conflitos com o Model Binder.
*   Realizar o cast manual de `int` para o `Enum` correspondente dentro dos métodos POST/PUT.

**Arquivos Impactados:**
*   `CartoesController.cs`
*   `CategoriasController.cs`
*   `TransacoesController.cs`
*   `MetasGastosController.cs`

---

## 🛡️ 2. Suite de Testes de Regressão (Integração)

Criaremos uma nova suite de testes para garantir que a API nunca mais retorne `400` por erro de contrato.

### 2.1 Configuração do Ambiente de Teste
*   **Projeto:** `GerenciadorFinanceiro.Tests`
*   **Ferramenta:** `WebApplicationFactory` para subir a API em memória.
*   **Banco de Dados:** Utilização de `SQLite In-Memory` ou transações isoladas para garantir testes rápidos e independentes.

### 2.2 Cenários de Teste Obrigatórios
1.  **POST Contrato Válido:** Enviar JSON com formato Angular e validar `200 OK`.
2.  **Enum Mapping:** Enviar números para campos que representam Enums e validar se a API aceita.
3.  **Validation Error Details:** Validar se o `ExceptionHandlingMiddleware` retorna o StackTrace e detalhes conforme configurado.

---

## 🚀 3. Fluxo de Execução

1.  **Refatoração:** Aplicar as mudanças nos DTOs e Controllers.
2.  **Build Docker:** Reconstruir as imagens para garantir que o ambiente de "produção" local está atualizado.
3.  **Desenvolvimento de Testes:** Criar as classes de teste de integração.
4.  **Validação Final:** Executar `dotnet test` e realizar testes manuais no telemóvel/browser.

---

## ✅ Critérios de Aceitação
*   Cadastro de Cartão de Crédito funcionando sem erro 400.
*   Cadastro de Categoria funcionando sem erro 400.
*   Log da API limpo, sem erros de desserialização.
*   Suite de testes passando com 100% de sucesso nos contratos HTTP.
