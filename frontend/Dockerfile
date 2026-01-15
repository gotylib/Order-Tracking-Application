# Build stage
FROM node:18-alpine AS build
WORKDIR /app

# Аргументы сборки (используем относительные пути через nginx)
ARG REACT_APP_API_URL=/api
ARG REACT_APP_WS_URL=

# Установка переменных окружения
ENV REACT_APP_API_URL=$REACT_APP_API_URL
ENV REACT_APP_WS_URL=$REACT_APP_WS_URL

# Копирование package.json и package-lock.json
COPY package*.json ./

# Установка зависимостей
RUN npm install

# Копирование исходного кода (node_modules исключается через .dockerignore)
COPY src ./src
COPY public ./public
COPY tsconfig.json ./

# Сборка приложения
RUN npm run build

# Production stage
FROM nginx:alpine
COPY --from=build /app/build /usr/share/nginx/html
COPY nginx.conf /etc/nginx/conf.d/default.conf
EXPOSE 80
CMD ["nginx", "-g", "daemon off;"]