# Guia de Publicação na Rede Doméstica

Este documento explica como disponibilizar o **Gerenciador Financeiro** para outros dispositivos na sua casa.

## 📦 1. Publicação via Docker Compose

O projeto está configurado para rodar de forma isolada e persistente usando Docker.

### Comandos:
```bash
# Iniciar o sistema em segundo plano
docker-compose up -d --build

# Parar o sistema
docker-compose down

# Ver logs da API
docker logs -f gf_api
```

## 🌐 2. Acesso na Rede

Para aceder a partir de outros dispositivos (telemóvel, tablet, outro portátil):

1.  **Descubra o seu IP Local:**
    *   No Windows: `ipconfig` (procure por IPv4 Address).
    *   Exemplo: `192.168.1.15`
2.  **Abra o Firewall:**
    *   Crie uma regra de entrada (Inbound Rule) no Windows para permitir a porta **TCP 80**.
3.  **Aceda pelo Browser:**
    *   URL: `http://<seu-ip-aqui>`

## 💾 3. Persistência de Dados

Os dados da base de dados PostgreSQL são armazenados localmente na pasta:
`./infra/data`

Isto garante que mesmo que apague os contentores, as suas transações financeiras não serão perdidas.

## 🔧 4. Portas Utilizadas
*   **80:** Frontend Angular (Nginx).
*   **5000:** API .NET (mapeado para 8080 interno).
*   **5432:** Base de Dados PostgreSQL.
