# Guia de Publicação na Rede Doméstica

Este documento explica como disponibilizar o **Gerenciador Financeiro** para outros dispositivos na sua casa.

## 📦 1. Publicação via Docker Compose

O projeto está configurado para rodar de forma isolada e persistente usando Docker.

### Comandos:
```bash
# Iniciar o sistema em segundo plano
docker compose up -d --build

# Parar o sistema
docker compose down

# Ver logs da API (útil para diagnóstico)
docker logs -f gf_api
```

## ⚙️ 2. Preparação da Base de Dados (Primeira Execução)

Após subir os contentores pela primeira vez, é necessário aplicar as migrações para criar as tabelas no PostgreSQL:

```bash
dotnet ef database update --project GerenciadorFinanceiro.Infrastructure --startup-project GerenciadorFinanceiro.Api
```

## 🌐 3. Acesso na Rede

Para aceder a partir de outros dispositivos (telemóvel, tablet, outro portátil):

1.  **Descubra o seu IP Local:**
    *   No Windows: `ipconfig` (procure por IPv4 Address).
    *   Exemplo: `192.168.1.15`
2.  **Configure a Rede no Windows:**
    *   Garanta que a sua ligação Wi-Fi está definida como **Rede Privada** (Definições > Rede e Internet > Propriedades). Se estiver como Pública, o Windows bloqueará o acesso.
3.  **Abra o Firewall:**
    *   Crie uma regra de entrada (Inbound Rule) no Windows para permitir a porta **TCP 80**.
4.  **Aceda pelo Browser:**
    *   URL: `http://<seu-ip-aqui>`

## 💾 4. Persistência de Dados

Os dados da base de dados PostgreSQL são armazenados localmente na pasta:
`./infra/data` (mapeada automaticamente pelo Docker).

Isto garante que mesmo que apague os contentores, as suas transações financeiras não serão perdidas.

## 🔧 5. Portas Utilizadas
*   **80:** Frontend Angular (Nginx) - **Única porta necessária para acesso externo.**
*   **5000:** API .NET (Mapeado para 8080 interno).
*   **5432:** Base de Dados PostgreSQL.

---
*Nota: O tempo de build inicial foi otimizado através do arquivo `.dockerignore`.*
