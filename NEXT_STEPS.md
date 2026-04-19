# 🚀 Roteiro de Desenvolvimento - Gerenciador Financeiro

Este documento detalha os próximos passos para a evolução do **Gerenciador Financeiro**, focado em completar o fluxo de importação, visualização de dados e gestão de limites.

---

## 1. Finalização da Importação de Extratos (Integração & UX)
O backend já possui a lógica de parsing (C6 e Genérico), mas precisamos garantir uma experiência fluida no frontend.

- [ ] **[Frontend] Refinar Componente `transacoes-import`**
    - Implementar seletor de arquivo com validação de extensão (.csv).
    - Adicionar seletor de **Conta Bancária** ou **Cartão de Crédito** (obrigatório conforme o Use Case).
    - Exibir um "Preview" das transações lidas antes de confirmar a persistência.
- [ ] **[Backend] Revisar Lógica de Criação de Categorias na Importação**
    - **Problema:** O sistema cria categorias automáticas baseadas apenas no nome, o que gera duplicatas e inconsistências.
    - **Solução Planeada:**
        - Implementar sistema de **Categorias Similares/Mapeamento**.
        - Utilizar algoritmos de similaridade para sugerir categorias existentes.
        - Criar interface para o utilizador aprovar/corrigir o mapeamento antes da persistência.
- [ ] **[Backend] Melhorar Resiliência do `ImportarExtratoUseCase`**
    - Adicionar log de transações duplicadas (evitar importar o mesmo extrato duas vezes baseado em Hash/ID Único).
    - Implementar rollback automático em caso de erro no meio do lote (Transaction Scope).
- [ ] **[Testes] Testes de Integração de Fluxo Completo**
    - Criar teste que simula o upload de um CSV real e verifica se as categorias automáticas e transações foram criadas corretamente no DB.

## 2. Dashboard & Inteligência Financeira
Transformar os dados brutos em informação útil para o utilizador.

- [ ] **[Backend] Endpoint de Sumário Mensal**
    - Criar DTO `ResumoMensalDto` (Total Receitas, Total Despesas, Saldo, % de Metas Atingidas).
    - Implementar lógica que respeite o **Padrão de Sinais** (- para gastos, + para ganhos).
- [ ] **[Frontend] Implementação de Gráficos (Dashboard)**
    - Integrar biblioteca de gráficos (Ex: Chart.js ou Ngx-charts).
    - Gráfico de Pizza: Distribuição de gastos por **Categoria**.
    - Gráfico de Linha: Evolução do saldo vs. Gastos ao longo do mês.

## 3. Gestão Avançada de Cartões de Crédito
O sistema precisa lidar com as particularidades de faturas.

- [x] **[Fullstack] Implementar CRUD Completo de Cartões de Crédito**
    - Backend: Endpoints para Listar, Criar, Editar e Excluir cartões.
    - Frontend: Tela de listagem e formulário de cadastro de cartões.
- [ ] **[Domain/Infra] Lógica de Faturas**
    - Implementar cálculo automático de data de fechamento e vencimento baseado nas configurações do cartão.
    - Permitir filtrar transações por "Fatura Aberta" vs "Fatura Fechada".
- [ ] **[Frontend] UI de Detalhe do Cartão**
    - Criar visualização de "Limite Disponível" (Limite Total - Soma das Despesas Negativas).

## 4. Gestão de Categorias e Metas
Melhorar a organização e o monitoramento de orçamentos.

- [x] **[Frontend] Adequar CRUD de Categorias**
    - Implementar funcionalidade de "Selecionar Todas" na listagem para ações em lote ou filtros.
    - Melhorar a navegação e visualização hierárquica (Pesquisa e Filtros adicionados).
- [ ] **[Application] Validação de Metas em Tempo Real**
    - No `ValidarMetaGastoUseCase`, adicionar lógica para enviar notificações ou alertas quando um gasto excede 80% ou 100% da meta da categoria.
- [ ] **[Frontend] Barra de Progresso de Metas**
    - Adicionar indicadores visuais na lista de categorias ou no dashboard mostrando o quão perto o utilizador está do limite definido para a `Meta de Gasto`.

## 5. Infraestrutura, Qualidade & DevOps
Garantir que o sistema seja robusto e fácil de manter.

- [x] **[Check] Verificação de Cobertura e Formatação (dotnet format)**
- [ ] **[Backend] Documentação Swagger Avançada**
    - Adicionar descrições XML e exemplos de Request/Response para facilitar o uso da API.
- [ ] **[Frontend] Implementar Linter (ESLint)**
    - Configurar ESLint com regras recomendadas para Angular e TypeScript.
    - Adicionar script `npm run lint` ao package.json.
- [ ] **[Infra] Dockerização do Ambiente**
    - Criar `Dockerfile` para API e Web.
    - Criar `docker-compose.yml` incluindo o banco de dados PostgreSQL para execução local rápida.
- [ ] **[CI/CD] Refinar Pipeline do GitHub Actions**
    - Adicionar passo de verificação de cobertura de testes (falhar se cair abaixo de 80%).

---

## 📝 Notas de Arquitetura
- Manter o uso de **Injeção de Dependências** rigorosa.
- Seguir o padrão **TDD**: Escrever o teste unitário na `GerenciadorFinanceiro.Tests` antes de implementar a lógica na `Application`.
- Todas as novas entidades devem herdar de uma `BaseEntity` (caso aplicável) para manter rastreabilidade de IDs.
