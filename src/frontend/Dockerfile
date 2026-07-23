# Estágio 1: Build da aplicação Angular
FROM node:20-alpine AS build
WORKDIR /app

# Copia as dependências do caminho do frontend a partir da raiz
COPY src/frontend/package*.json ./
RUN npm install --legacy-peer-deps

# Copia todo o código-fonte do frontend
COPY src/frontend/ ./
RUN npm run build -- --configuration production

# Estágio 2: Servidor Nginx para rodar o app compilado
FROM nginx:alpine

# Copia a build do Angular
COPY --from=build /app/dist/kyly-frontend/browser /usr/share/nginx/html

# Copia o arquivo nginx.conf
COPY src/frontend/nginx.conf /etc/nginx/conf.d/default.conf

EXPOSE 80
CMD ["nginx", "-g", "daemon off;"]