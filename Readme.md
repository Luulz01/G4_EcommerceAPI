# EcommerceAPI

## Autores

- Grupo 4 — Aplicaciones Web, Escuela Politécnica Nacional
- Benavides Jake
- Casa Fernando
- Casa Antonela
- Hernández Lizeth


API RESTful de e-commerce desarrollada en **C# con ASP.NET Core Web API (.NET 8)**. Este proyecto replica el mismo dominio funcional (Users, Products, Receipts, ReceiptItems) del backend de referencia en Spring Boot, pero implementado en el stack .NET.

## Tecnologías utilizadas

- **.NET 8** — Framework principal
- **ASP.NET Core Web API** — Framework REST con controladores
- **Entity Framework Core 8** — ORM para persistencia
- **PostgreSQL** — Base de datos relacional
- **JWT (JSON Web Tokens)** — Autenticación y protección de rutas
- **BCrypt.Net-Next** — Cifrado de contraseñas
- **Swagger** — Documentación e interfaz de pruebas interactiva
- **Postman** — Colección de pruebas exportada como evidencia

## Arquitectura

El proyecto sigue una arquitectura por capas, separando responsabilidades:

```
EcommerceAPI/
├── Controllers/            # Capa de entrada HTTP (endpoints REST)
├── Services/               # Lógica de negocio (cálculo de totales, validaciones, JWT)
├── Repositories/           # Acceso a datos (Entity Framework Core)
├── Models/                 # Entidades del dominio (User, Product, Receipt, ReceiptItem)
├── DTOs/                   # Objetos de entrada/salida (nunca se exponen las entidades directamente)
├── Data/                   # AppDbContext (configuración de EF Core)
├── Middleware/             # Manejo global de errores + filtro de seguridad para Swagger
├── Migrations/             # Migraciones de EF Core (historial de la base de datos)
├── Program.cs              # Configuración de servicios, middlewares y arranque
└── appsettings.json        # Configuración (cadena de conexión, JWT)
```

**Flujo de una petición:** 

`Controller → Service → Repository → AppDbContext → PostgreSQL`

Los controladores nunca acceden directamente a la base de datos; siempre pasan por un `Service`, que a su vez usa un `Repository`.

## Entidades del dominio

| Entidad | Descripción |
|---|---|
| `User` | Usuario del sistema. Contraseña cifrada con BCrypt, nunca expuesta en respuestas. |
| `Product` | Producto del catálogo, con precio (`decimal`) y stock. |
| `Receipt` | Recibo/orden de compra. El `Total` se calcula en el backend, nunca lo envía el cliente. |
| `ReceiptItem` | Detalle de cada producto comprado dentro de un recibo (cantidad, precio unitario, subtotal). |

## Reglas de negocio implementadas

- El cliente **nunca** envía el total de la compra; se calcula en el backend con el precio real almacenado en la base de datos.
- Al crear un recibo, se **valida stock disponible** antes de confirmar la compra.
- El **stock se descuenta automáticamente** al crear un recibo, dentro de una transacción (si algo falla, no se descuenta nada).
- Las contraseñas **nunca se devuelven** en las respuestas HTTP.
- Las contraseñas se almacenan **cifradas con BCrypt**.
- Los montos monetarios usan tipos `decimal` (nunca `float`/`double`) para evitar errores de precisión.
- Las respuestas de error son centralizadas mediante un middleware global (`ExceptionHandlingMiddleware`).

## Requisitos previos

Antes de clonar el proyecto, instalar en la máquina:

1. **[.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)** — verificar con `dotnet --version` (debe mostrar 8.x).
2. **[PostgreSQL](https://www.postgresql.org/download/)** (con pgAdmin, incluido en el instalador) — anotar usuario y contraseña de PostgreSQL al instalarlo.
3. **Herramienta EF Core CLI**, instalar globalmente con:
   ```bash
   dotnet tool install --global dotnet-ef
   ```
   Si da error de "comando no encontrado" después de instalarla, cerrar y volver a abrir la terminal.

4. **(Opcional)** [Postman](https://www.postman.com/downloads/) para importar la colección de pruebas incluida en el repositorio.

## Instalación y ejecución local

### 1. Clonar el repositorio

```bash
git clone https://github.com/Luulz01/G4_EcommerceAPI.git
cd G4_EcommerceAPI
```

### 2. Restaurar los paquetes NuGet

```bash
dotnet restore
```

### 3. Crear la base de datos en PostgreSQL

Abrir pgAdmin (o `psql`) y crear una base de datos vacía, por ejemplo:

```sql
CREATE DATABASE "EcommerceDB";
```

> No es obligatorio crearla manualmente: el primer `dotnet ef database update` la crea automáticamente si no existe, siempre que el usuario de PostgreSQL tenga permisos.

### 4. Configurar la cadena de conexión y JWT

Abrir `appsettings.json` y reemplazar los valores de `Username` y `Password` con **tus propias credenciales locales de PostgreSQL** (las que configuraste al instalarlo en tu máquina). Ejemplo, usando el usuario por defecto `postgres`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=EcommerceDB;Username=postgres;Password=TU_PASSWORD"
  },
  "Jwt": {
    "Key": "EPN2026_Ecommerce_API_Seguridad_JWT_Clave_Secreta_Grupo4_123456789",
    "Issuer": "EcommerceAPI",
    "Audience": "EcommerceAPIUsers",
    "ExpireMinutes": 60
  }
}
```

### 5. Aplicar las migraciones (crear las tablas)

```bash
dotnet ef database update
```

Esto crea las tablas `Users`, `Products`, `Receipts` y `ReceiptItems` en tu base de datos `EcommerceDB`.

### 6. Ejecutar el proyecto

```bash
dotnet run
```

La consola te mostrará la URL donde corre la API, por ejemplo:

```
Now listening on: http://localhost:5248
```

### 7. Abrir la documentación interactiva (Swagger)

Con el proyecto corriendo, abre en el navegador:

```
http://localhost:5248/swagger
```

Ahí puedes ver y probar los 15 endpoints directamente, incluyendo el botón **"Authorize"** para pegar un token JWT y probar las rutas protegidas.

## Endpoints disponibles

### Users

| Método | Ruta | Descripción | Requiere token |
|---|---|---|---|
| POST | `/api/users/register` | Registra un nuevo usuario | No |
| POST | `/api/users/login` | Inicia sesión y devuelve un JWT | No |
| GET | `/api/users/{id}` | Obtiene un usuario por id | No |
| PUT | `/api/users/{id}` | Actualiza un usuario | No |
| DELETE | `/api/users/{id}` | Elimina un usuario | No |

### Products

| Método | Ruta | Descripción | Requiere token |
|---|---|---|---|
| POST | `/api/products` | Crea un producto | **Sí** |
| GET | `/api/products` | Lista todos los productos | No |
| GET | `/api/products/{id}` | Obtiene un producto por id | No |
| PUT | `/api/products/{id}` | Actualiza un producto | **Sí** |
| DELETE | `/api/products/{id}` | Elimina un producto | **Sí** |

### Receipts

| Método | Ruta | Descripción | Requiere token |
|---|---|---|---|
| POST | `/api/receipts` | Crea un recibo (calcula total y descuenta stock) | **Sí** |
| GET | `/api/receipts` | Lista todos los recibos | No |
| GET | `/api/receipts/{id}` | Obtiene un recibo por id | No |
| GET | `/api/receipts/user/{userId}` | Lista los recibos de un usuario | No |
| DELETE | `/api/receipts/{id}` | Elimina un recibo | **Sí** |

**Para usar un endpoint protegido:**
1. Haz `POST /api/users/register` (o usar un usuario existente) y luego `POST /api/users/login`.
2. Copiar el valor de `token` de la respuesta.
3. En Swagger, hacer clic en **"Authorize"** (arriba a la derecha) y pegar **solo el token sin comillas**


## Pruebas con Postman

El repositorio incluye dos archivos para importar en Postman:

- `EcommerceAPI.postman_collection.json` — colección con los 15 endpoints ya organizados.
- `EcommerceAPI.postman_environment.json` — environment con las variables `baseUrl` y `token`.

**Para usarlos:**
1. Abre Postman → Import → selecciona ambos archivos.
2. Selecciona el environment **"Local"** en la esquina superior derecha.
3. Ejecuta primero `users > login`; el script guarda el token automáticamente en la variable `token`.
4. Ejecuta cualquier otro endpoint protegido; ya usará `{{token}}` automáticamente en la pestaña Authorization.

