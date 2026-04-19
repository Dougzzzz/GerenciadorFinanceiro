# Estágio de Build
FROM node:22-alpine AS build
WORKDIR /app

# Instalar dependências
COPY GerenciadorFinanceiro.Web/package*.json ./
RUN npm install

# Copiar código e compilar
COPY GerenciadorFinanceiro.Web/ ./
RUN npm run build -- --configuration production

# Estágio de Runtime (Nginx)
FROM nginx:stable-alpine
COPY --from=build /app/dist/gerenciador-financeiro.web/browser /usr/share/nginx/html

# Configuração customizada do Nginx para suportar as rotas do Angular (SPA)
COPY GerenciadorFinanceiro.Web/nginx.conf /etc/nginx/conf.d/default.conf

EXPOSE 80
CMD ["nginx", "-g", "daemon off;"]
