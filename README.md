# TaskBoard Realtime App

Una aplicación de lista de tareas colaborativa en tiempo real desarrollada con **.NET 8**, **Redis**, **Vue 3** y **SignalR**, 100% dockerizada.

---

## Características

- CRUD de tareas (crear, actualizar, eliminar, marcar como completadas)
- Actualización en tiempo real con WebSockets (SignalR)
- Almacenamiento en memoria con Redis
- Frontend moderno en Vue 3 + Vite
- API documentada con Swagger
- Backend y frontend corriendo en Docker

---

## Tecnologías utilizadas

### Backend (.NET 8 + Redis)

- ASP.NET Core Web API
- SignalR para comunicación en tiempo real
- Redis (vía `StackExchange.Redis`) como almacenamiento
- Swagger para documentación y pruebas

### Frontend (Vue 3 + Vite)

- Vue 3 con Composition API
- Axios para requests HTTP
- @microsoft/signalr para WebSockets
- Docker para entorno aislado

---

## Cómo levantar la aplicación

### Requisitos

- Docker Desktop instalado (Windows/Linux/Mac)

### Instrucciones

1. Cloná el repositorio:

```bash
git clone https://github.com/tuusuario/taskboard.git
cd taskboard
```

2. Levantá todo con Docker Compose:

```bash
docker-compose up --build
```

3. Accedé a
API Swagger: http://localhost:8000/swagger
1. Frontend Vue: http://localhost:5173

---

## Funcionamiento interno
Al iniciar, el frontend carga todas las tareas desde la API.

Cuando un usuario agrega/modifica/borra una tarea:

El backend actualiza los datos en Redis.

Luego, emite un evento por SignalR.

Todos los clientes conectados actualizan su UI automáticamente, sin recargar.

---

## CORS y red interna
El backend permite solicitudes desde el frontend mediante CORS y utiliza tasknet como red Docker compartida entre servicios.

```bash
En el frontend (.env o desde Docker):
VITE_API_URL=http://api:5000
```

En el backend (ya seteadas en docker-compose.yml):
```bash
ASPNETCORE_URLS=http://+:5000
DOTNET_ENVIRONMENT=Development
```