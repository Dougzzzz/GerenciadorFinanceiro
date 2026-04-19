# Estágio de Build
FROM node:22-alpine AS build
WORKDIR /app/GerenciadorFinanceiro.Web

# Instalar dependências
COPY GerenciadorFinanceiro.Web/package*.json ./
RUN npm install --legacy-peer-deps

# Copiar código e compilar
# Precisamos manter a estrutura de pastas para que o outputPath relativo funcione
WORKDIR /app
COPY GerenciadorFinanceiro.Web/ ./GerenciadorFinanceiro.Web/
WORKDIR /app/GerenciadorFinanceiro.Web
RUN npm run build -- --configuration production

# Estágio de Runtime (Nginx)
FROM nginx:stable-alpine
# O angular.json aponta para ../GerenciadorFinanceiro.Api/wwwroot/browser
COPY --from=build /app/GerenciadorFinanceiro.Api/wwwroot/browser /usr/share/nginx/html

# Configuração customizada do Nginx para suportar as rotas do Angular (SPA)
COPY GerenciadorFinanceiro.Web/nginx.conf /etc/nginx/conf.d/default.conf

EXPOSE 80
CMD ["nginx", "-g", "daemon off;"]
