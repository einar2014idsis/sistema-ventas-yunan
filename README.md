# Sistema de Ventas YUNAN

## Plataforma web de comercio e inteligencia comercial para una tienda de moda

Sistema web desarrollado con **ASP.NET Core MVC**, **C#**, **Entity Framework Core** y **SQL Server** para gestionar el ciclo completo de ventas de YUNAN Fashion Store.

El proyecto integra una experiencia de compra para clientes y un panel administrativo para controlar productos, inventario, clientes, ventas y reportes. Su dashboard transforma las operaciones registradas en indicadores comerciales útiles para tomar decisiones.

## Funcionalidades principales

- Catálogo de productos y categorías.
- Carrito de compras y flujo de venta.
- Gestión de clientes y administradores con permisos diferenciados.
- Registro de ventas y detalle de productos vendidos.
- Control de inventario y alertas de stock.
- Cupones de descuento y gestión de envíos.
- Dashboard administrativo de ventas con métricas, rankings y análisis visual.
- Filtros por fechas, cliente y método de pago.
- Exportación de reportes en Excel con EPPlus.
- Persistencia de datos con Entity Framework Core y SQL Server.
- Migraciones para versionar la estructura de la base de datos.

## Tecnologías

| Área | Tecnología |
|---|---|
| Backend | ASP.NET Core MVC, C# |
| Datos | Entity Framework Core 9, SQL Server |
| Frontend | Razor Views, HTML, CSS, JavaScript |
| Visualización | Chart.js |
| Reportes | EPPlus |
| Documentos | QuestPDF |
| Seguridad | Autenticación, autorización y filtros por rol |

## Arquitectura

La solución separa responsabilidades en controladores, servicios, modelos de dominio, ViewModels, filtros de autorización y vistas Razor. Las consultas de ventas e inventario se apoyan en agregaciones de Entity Framework Core para evitar duplicar importes al trabajar con detalles de venta.

## Requisitos

- .NET 8 SDK.
- SQL Server o SQL Server Express.
- Visual Studio 2022/2026 con la carga de trabajo de ASP.NET y desarrollo web.

## Configuración local

1. Copia `.github/appsettings.example.json` como `Proyecto_DAS/appsettings.json`.
2. Configura tu cadena de conexión de SQL Server.
3. Configura las credenciales SMTP únicamente mediante secretos locales o variables de entorno.
4. Ejecuta las migraciones si tu base de datos aún no está actualizada:

```bash
dotnet ef database update --project Proyecto_DAS/SistemaVentasYUNAN.csproj
```

5. Inicia la aplicación:

```bash
dotnet run --project Proyecto_DAS/SistemaVentasYUNAN.csproj
```

## Nota de seguridad

No se incluyen credenciales, cadenas de conexión reales ni datos de producción. Usa User Secrets, variables de entorno o un archivo local ignorado por Git.

## Propósito del proyecto

Este proyecto demuestra la construcción de una solución comercial full-stack: modelado de datos, operaciones de venta, control de inventario, autorización administrativa, analítica de negocio y generación de reportes.


## Organización del código y modelo de datos

- `Proyecto_DAS/Controllers/`: acciones MVC para catálogo, carrito, ventas, inventario y administración.
- `Models/` y `Data/AppDbContext.cs`: entidades y persistencia; el dominio incluye usuarios, clientes, productos y variantes, categorías, proveedores, ventas y detalles, cupones, envíos y alertas de stock.
- `ViewModels/`, `Views/` y `wwwroot/`: datos de formularios, vistas Razor y recursos de interfaz.
- `Services/`: carrito, dashboard, reportes Excel, boletas PDF y envío opcional de correo mediante MailKit.
- `Filters/`: restricciones de acceso para administrador y cliente; `Migrations/`: evolución del esquema.

La aplicación guarda identificador y rol en la sesión y los filtros comprueban esos valores. El controlador de acceso aplica SHA-256 directamente a las contraseñas: antes de un despliegue real debe sustituirse por un algoritmo de derivación de contraseñas con sal, como el proporcionado por ASP.NET Core Identity, y revisarse la protección de rutas y sesiones.

Para ejecutar la migración mediante `dotnet ef`, instala o restaura previamente una versión compatible de la herramienta y revisa la cadena `ConnectionStrings:StringSQLServer`. El envío de boletas por correo depende de `EmailSettings` y puede permanecer deshabilitado. Las pruebas JavaScript del dashboard están en `Proyecto_DAS/tests/`.
