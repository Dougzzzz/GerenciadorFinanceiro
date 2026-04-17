# Gerenciador Financeiro 💰

O **Gerenciador Financeiro** é uma aplicação completa para controle de finanças pessoais, permitindo a gestão de transações, contas bancárias, cartões de crédito e metas de gastos. O sistema foca em automação através da importação de extratos bancários e oferece uma visão consolidada via dashboards.

## 🏗️ Arquitetura

O projeto segue o modelo de **Camadas (Layered Architecture)** com forte inspiração em **Clean Architecture** no backend e uma estrutura baseada em **Features** no frontend.

### Estrutura do Projeto
- **Frontend:** Angular 19+ com arquitetura orientada a features.
- **Backend (.NET 9):**
  - `Domain`: Entidades, interfaces e regras de negócio.
  - `Application`: Casos de uso (Use Cases) e DTOs.
  - `Infrastructure`: Implementação de repositórios (EF Core), leitores de arquivos e persistência.
  - `Api`: Controllers, Middlewares e configuração do Swagger.

## 🛠️ Tecnologias e Ferramentas

- **Backend:** .NET 9, Entity Framework Core, PostgreSQL, SQLite (para testes).
- **Frontend:** Angular 19, Jasmine/Karma para testes, Vanilla CSS para estilização.
- **Testes:**
  - **Backend:** xUnit, NSubstitute, Moq (parcial), Coverlet para cobertura.
  - **Frontend:** Jasmine, Karma, Karma-Coverage.
- **CI/CD:** GitHub Actions (validação automática de builds e testes em cada Push/PR).

## 🚀 Como Executar o Projeto

### Pré-requisitos
- .NET 9 SDK
- Node.js 20+ e npm
- PostgreSQL (opcional para execução local se usar In-Memory/SQLite)

### Backend (.NET)
1. Navegue até a raiz do projeto.
2. Restaure as dependências:
   ```bash
   dotnet restore
   ```
3. Execute a aplicação:
   ```bash
   dotnet run --project GerenciadorFinanceiro.Api
   ```
   A API estará disponível em `http://localhost:5000` (ou na porta configurada em `launchSettings.json`).

### Frontend (Angular)
1. Navegue até a pasta do frontend:
   ```bash
   cd GerenciadorFinanceiro.Web
   ```
2. Instale as dependências:
   ```bash
   npm install
   ```
3. Execute o servidor de desenvolvimento:
   ```bash
   npm start
   ```
   Acesse `http://localhost:4200` no seu navegador.

---

## 🧪 Testes e Qualidade

O projeto preza por uma alta cobertura de testes para garantir a integridade das regras financeiras.

### Backend
Para rodar todos os testes unitários:
```bash
dotnet test
```
Para rodar com coleta de cobertura:
```bash
dotnet test --collect:"XPlat Code Coverage"
```

### Frontend
Para rodar os testes uma única vez (modo CI):
```bash
npm run test:ci --prefix GerenciadorFinanceiro.Web
```
Para rodar com relatório de cobertura HTML:
```bash
npm run test:coverage --prefix GerenciadorFinanceiro.Web
```
O relatório será gerado em `GerenciadorFinanceiro.Web/coverage/gerenciador-financeiro.web/index.html`.

---

## 📈 Regras de Negócio e Padrões

- **Padrão de Sinais:** Gastos (Saídas) são sempre armazenados como **valores negativos**. Rendimentos (Entradas) são **valores positivos**.
- **Metas de Gastos:** Limites de orçamento definidos por categoria. Podem ser recorrentes ou específicas para um mês/ano.
- **Importação:** O sistema suporta leitura flexível de CSVs, identificando automaticamente colunas de data, descrição e valor, mesmo com diferentes separadores decimais.

## 📄 Licença
Este projeto está sob a licença MIT. Veja o arquivo [LICENSE.txt](LICENSE.txt) para detalhes.
