using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Proyecto_DAS.Migrations
{
    /// <inheritdoc />
    public partial class Inicial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Categorias",
                columns: table => new
                {
                    IdCategoria = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Estado = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categorias", x => x.IdCategoria);
                });

            migrationBuilder.CreateTable(
                name: "CuponDescuento",
                columns: table => new
                {
                    IdCuponDescuento = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CodigoCupon = table.Column<string>(type: "nvarchar(12)", maxLength: 12, nullable: false),
                    PorcentajeDescuento = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    FechaInicio = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaVencimiento = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Estado = table.Column<bool>(type: "bit", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CuponDescuento", x => x.IdCuponDescuento);
                });

            migrationBuilder.CreateTable(
                name: "Proveedores",
                columns: table => new
                {
                    IdProveedor = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NombreEmpresa = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ContactoNombre = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Celular = table.Column<string>(type: "nvarchar(9)", maxLength: 9, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Direccion = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Ruc = table.Column<string>(type: "nvarchar(11)", maxLength: 11, nullable: false),
                    Estado = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Proveedores", x => x.IdProveedor);
                });

            migrationBuilder.CreateTable(
                name: "Usuarios",
                columns: table => new
                {
                    IdUsuario = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombres = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Apellidos = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Contrasenia = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Rol = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Celular = table.Column<string>(type: "nvarchar(9)", maxLength: 9, nullable: false),
                    FechaRegistro = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Estado = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Usuarios", x => x.IdUsuario);
                });

            migrationBuilder.CreateTable(
                name: "Productos",
                columns: table => new
                {
                    IdProducto = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    PrecioCompra = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PrecioVenta = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ImagenUrl = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Estado = table.Column<bool>(type: "bit", nullable: false),
                    FechaRegistro = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CategoriaId = table.Column<int>(type: "int", nullable: false),
                    ProveedorId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Productos", x => x.IdProducto);
                    table.ForeignKey(
                        name: "FK_Productos_Categorias_CategoriaId",
                        column: x => x.CategoriaId,
                        principalTable: "Categorias",
                        principalColumn: "IdCategoria",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Productos_Proveedores_ProveedorId",
                        column: x => x.ProveedorId,
                        principalTable: "Proveedores",
                        principalColumn: "IdProveedor",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Administradores",
                columns: table => new
                {
                    IdAdministrador = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Cargo = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    FechaContratacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UsuarioId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Administradores", x => x.IdAdministrador);
                    table.ForeignKey(
                        name: "FK_Administradores_Usuarios_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "Usuarios",
                        principalColumn: "IdUsuario",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Clientes",
                columns: table => new
                {
                    IdCliente = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Direccion = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Ciudad = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CodigoPostal = table.Column<string>(type: "nvarchar(6)", maxLength: 6, nullable: false),
                    DniRuc = table.Column<string>(type: "nvarchar(11)", maxLength: 11, nullable: false),
                    FechaNacimiento = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UsuarioId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Clientes", x => x.IdCliente);
                    table.ForeignKey(
                        name: "FK_Clientes_Usuarios_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "Usuarios",
                        principalColumn: "IdUsuario",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProductoVariantes",
                columns: table => new
                {
                    IdProductoVariante = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProductoId = table.Column<int>(type: "int", nullable: false),
                    Talla = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Color = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    StockActual = table.Column<int>(type: "int", nullable: false),
                    StockMinimo = table.Column<int>(type: "int", nullable: false),
                    Estado = table.Column<bool>(type: "bit", nullable: false),
                    FechaRegistro = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductoVariantes", x => x.IdProductoVariante);
                    table.ForeignKey(
                        name: "FK_ProductoVariantes_Productos_ProductoId",
                        column: x => x.ProductoId,
                        principalTable: "Productos",
                        principalColumn: "IdProducto",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Ventas",
                columns: table => new
                {
                    IdVenta = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FechaVenta = table.Column<DateTime>(type: "datetime2", nullable: false),
                    SubtotalSinDescuento = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    IGV = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TotalVenta = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    MetodoPago = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    EstadoPago = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    ClienteId = table.Column<int>(type: "int", nullable: false),
                    CuponDescuentoId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ventas", x => x.IdVenta);
                    table.ForeignKey(
                        name: "FK_Ventas_Clientes_ClienteId",
                        column: x => x.ClienteId,
                        principalTable: "Clientes",
                        principalColumn: "IdCliente",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Ventas_CuponDescuento_CuponDescuentoId",
                        column: x => x.CuponDescuentoId,
                        principalTable: "CuponDescuento",
                        principalColumn: "IdCuponDescuento",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "AlertaStock",
                columns: table => new
                {
                    IdAlertaStock = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FechaAlerta = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Estado = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    FechaResolucion = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ProductoVarianteId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AlertaStock", x => x.IdAlertaStock);
                    table.ForeignKey(
                        name: "FK_AlertaStock_ProductoVariantes_ProductoVarianteId",
                        column: x => x.ProductoVarianteId,
                        principalTable: "ProductoVariantes",
                        principalColumn: "IdProductoVariante",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DetallesVentas",
                columns: table => new
                {
                    IdDetalleVenta = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Cantidad = table.Column<int>(type: "int", nullable: false),
                    PrecioUnitario = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    SubTotal = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Talla = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    Color = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    VentaId = table.Column<int>(type: "int", nullable: false),
                    ProductoId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DetallesVentas", x => x.IdDetalleVenta);
                    table.ForeignKey(
                        name: "FK_DetallesVentas_Productos_ProductoId",
                        column: x => x.ProductoId,
                        principalTable: "Productos",
                        principalColumn: "IdProducto");
                    table.ForeignKey(
                        name: "FK_DetallesVentas_Ventas_VentaId",
                        column: x => x.VentaId,
                        principalTable: "Ventas",
                        principalColumn: "IdVenta",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Envios",
                columns: table => new
                {
                    IdEnvio = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DireccionEnvio = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Ciudad = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CodigoPostal = table.Column<string>(type: "nvarchar(6)", maxLength: 6, nullable: false),
                    FechaEnvio = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaEntregaEstimada = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaEntregaReal = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EstadoEnvio = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    EmpresaTransporte = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    NumeroSeguimiento = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    VentaId = table.Column<int>(type: "int", nullable: false),
                    AdministradorId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Envios", x => x.IdEnvio);
                    table.ForeignKey(
                        name: "FK_Envios_Administradores_AdministradorId",
                        column: x => x.AdministradorId,
                        principalTable: "Administradores",
                        principalColumn: "IdAdministrador");
                    table.ForeignKey(
                        name: "FK_Envios_Ventas_VentaId",
                        column: x => x.VentaId,
                        principalTable: "Ventas",
                        principalColumn: "IdVenta",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Categorias",
                columns: new[] { "IdCategoria", "Descripcion", "Estado", "Nombre" },
                values: new object[,]
                {
                    { 1, "Combina short y falda, estilo casual", true, "Short Falda" },
                    { 2, "Mini faldas clásicas y modernas, variadas", true, "Faldas" },
                    { 3, "Chaquetas ideales para un look", true, "Casacas" },
                    { 4, "Pitillo, Baggy o Cargo, para todo estilo", true, "Pantalones" },
                    { 5, "Shorts que resaltan tu belleza", true, "Short" }
                });

            migrationBuilder.InsertData(
                table: "Proveedores",
                columns: new[] { "IdProveedor", "Celular", "ContactoNombre", "Direccion", "Email", "Estado", "NombreEmpresa", "Ruc" },
                values: new object[,]
                {
                    { 1, "963256369", "Roberto Sánchez", "Av. Industrial 456, Trujillo", "ventas@textilesnorte.com", true, "Textiles del Norte SAC", "20123456789" },
                    { 2, "985632159", "Patricia Mendoza", "Jr. Gamarra 1234, La Victoria", "contacto@confeccioneslima.pe", true, "Confecciones Lima EIRL", "20234567890" },
                    { 3, "915935746", "Carlos Vega", "Av. Argentina 567, Callao", "pedidos@denimperu.com", true, "Denim Perú SA", "20345678901" }
                });

            migrationBuilder.InsertData(
                table: "Usuarios",
                columns: new[] { "IdUsuario", "Apellidos", "Celular", "Contrasenia", "Email", "Estado", "FechaRegistro", "Nombres", "Rol" },
                values: new object[,]
                {
                    { 1, "Principal", "999888777", "931351bc4cc867cfbace04e0131451d0e2231049a2ba9bd4a2b0b398d73e6fe6", "admin@yunan.com", true, new DateTime(2025, 1, 3, 0, 0, 0, 0, DateTimeKind.Unspecified), "Admin", "Administrador" },
                    { 2, "García López", "987654321", "54803ff75c237cd234118278e12a76979002cb298e18a088ef9cfbc63fdb3a3c", "sentopasword@gmail.com", true, new DateTime(2025, 1, 3, 0, 0, 0, 0, DateTimeKind.Unspecified), "María", "Cliente" },
                    { 3, "Rodríguez Pérez", "976543210", "54803ff75c237cd234118278e12a76979002cb298e18a088ef9cfbc63fdb3a3c", "einar2014idsis@gmail.com", true, new DateTime(2025, 1, 3, 0, 0, 0, 0, DateTimeKind.Unspecified), "Carlos", "Cliente" }
                });

            migrationBuilder.InsertData(
                table: "Administradores",
                columns: new[] { "IdAdministrador", "Cargo", "FechaContratacion", "UsuarioId" },
                values: new object[] { 1, "Gerente General", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1 });

            migrationBuilder.InsertData(
                table: "Clientes",
                columns: new[] { "IdCliente", "Ciudad", "CodigoPostal", "Direccion", "DniRuc", "FechaNacimiento", "UsuarioId" },
                values: new object[,]
                {
                    { 1, "Trujillo", "13001", "Av. Larco 123", "12345678", new DateTime(1990, 5, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), 2 },
                    { 2, "Trujillo", "13002", "Jr. Pizarro 456", "23456789", new DateTime(1985, 8, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), 3 }
                });

            migrationBuilder.InsertData(
                table: "Productos",
                columns: new[] { "IdProducto", "CategoriaId", "Descripcion", "Estado", "FechaRegistro", "ImagenUrl", "Nombre", "PrecioCompra", "PrecioVenta", "ProveedorId" },
                values: new object[,]
                {
                    { 1, 4, "Wide Leg-Palazo Cargo.", true, new DateTime(2025, 1, 2, 0, 0, 0, 0, DateTimeKind.Unspecified), "~/img/Img_Productos/pa_car.jpg", "Pantalón Cargo", 20.0m, 44.00m, 1 },
                    { 2, 4, "Baggy con elastico y rayas blancas.", true, new DateTime(2025, 1, 2, 0, 0, 0, 0, DateTimeKind.Unspecified), "~/img/Img_Productos/pa_ba.jpg", "Pantalón Baggy", 20.15m, 38.00m, 1 },
                    { 3, 1, "Short falda que te acompaña a cualquier lado.", true, new DateTime(2025, 1, 2, 0, 0, 0, 0, DateTimeKind.Unspecified), "~/img/Img_Productos/sh_fa_casu.jpg", "Short Falda Casual", 24.50m, 35.00m, 2 },
                    { 4, 1, "Short falda para tus salidas casuales.", true, new DateTime(2025, 1, 2, 0, 0, 0, 0, DateTimeKind.Unspecified), "~/img/Img_Productos/sh_fa_urb.jpg", "Short Falda Urbana", 15.80m, 29.00m, 2 },
                    { 5, 1, "Short falda para tus fiestas imperdibles.", true, new DateTime(2025, 1, 2, 0, 0, 0, 0, DateTimeKind.Unspecified), "~/img/Img_Productos/sh_fa_fies.jpg", "Short Falda 4 Botones", 26.60m, 38.00m, 2 },
                    { 6, 3, "Casaca fresh terry.", true, new DateTime(2025, 1, 2, 0, 0, 0, 0, DateTimeKind.Unspecified), "~/img/Img_Productos/ca_nor.jpg", "Casaca Diseñador", 20.60m, 35.00m, 3 },
                    { 7, 3, "Casaca jeans strech.", true, new DateTime(2025, 1, 2, 0, 0, 0, 0, DateTimeKind.Unspecified), "~/img/Img_Productos/ca_ras.jpg", "Casaca Mini Rasgada", 15.80m, 28.00m, 3 },
                    { 8, 5, "Short estilo cargo único.", true, new DateTime(2025, 1, 2, 0, 0, 0, 0, DateTimeKind.Unspecified), "~/img/Img_Productos/sh_estre.jpg", "Short Cargo", 15.60m, 32.00m, 1 },
                    { 9, 5, "Short jean strech.", true, new DateTime(2025, 1, 2, 0, 0, 0, 0, DateTimeKind.Unspecified), "~/img/Img_Productos/sh_1_bo.jpg", "Short 1 Botón", 10.00m, 23.00m, 1 },
                    { 10, 5, "Short de jeans drill strech.", true, new DateTime(2025, 1, 2, 0, 0, 0, 0, DateTimeKind.Unspecified), "~/img/Img_Productos/sh_3_bo.jpg", "Short 3 Botones", 14.30m, 24.00m, 1 },
                    { 11, 2, "Falda con estilo cargo y 9 botones.", true, new DateTime(2025, 1, 2, 0, 0, 0, 0, DateTimeKind.Unspecified), "~/img/Img_Productos/fa_9_bo.jpg", "Falda Cargo", 10.00m, 22.00m, 2 },
                    { 12, 2, "Falda con 3 botones para el día a día.", true, new DateTime(2025, 1, 2, 0, 0, 0, 0, DateTimeKind.Unspecified), "~/img/Img_Productos/fa_3_bo.jpg", "Falda 3 Botones", 10.00m, 22.00m, 2 }
                });

            migrationBuilder.InsertData(
                table: "ProductoVariantes",
                columns: new[] { "IdProductoVariante", "Color", "Estado", "FechaRegistro", "ProductoId", "StockActual", "StockMinimo", "Talla" },
                values: new object[,]
                {
                    { 1, "Marrón", true, new DateTime(2025, 1, 2, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 15, 5, "28" },
                    { 2, "Beige", true, new DateTime(2025, 1, 2, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 12, 5, "28" },
                    { 3, "Marrón", true, new DateTime(2025, 1, 2, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 18, 5, "30" },
                    { 4, "Beige", true, new DateTime(2025, 1, 2, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 15, 5, "30" },
                    { 5, "Celeste", true, new DateTime(2025, 1, 2, 0, 0, 0, 0, DateTimeKind.Unspecified), 2, 6, 2, "28" },
                    { 6, "Verde", true, new DateTime(2025, 1, 2, 0, 0, 0, 0, DateTimeKind.Unspecified), 2, 5, 1, "28" },
                    { 7, "Celeste", true, new DateTime(2025, 1, 2, 0, 0, 0, 0, DateTimeKind.Unspecified), 2, 5, 1, "30" },
                    { 8, "Verde", true, new DateTime(2025, 1, 2, 0, 0, 0, 0, DateTimeKind.Unspecified), 2, 4, 1, "30" },
                    { 9, "Verde", true, new DateTime(2025, 1, 2, 0, 0, 0, 0, DateTimeKind.Unspecified), 3, 8, 2, "28" },
                    { 10, "Negro", true, new DateTime(2025, 1, 2, 0, 0, 0, 0, DateTimeKind.Unspecified), 3, 6, 2, "28" },
                    { 11, "Verde", true, new DateTime(2025, 1, 2, 0, 0, 0, 0, DateTimeKind.Unspecified), 3, 6, 1, "30" },
                    { 12, "Negro", true, new DateTime(2025, 1, 2, 0, 0, 0, 0, DateTimeKind.Unspecified), 3, 5, 1, "30" },
                    { 13, "Azul", true, new DateTime(2025, 1, 2, 0, 0, 0, 0, DateTimeKind.Unspecified), 4, 10, 2, "28" },
                    { 14, "Beige", true, new DateTime(2025, 1, 2, 0, 0, 0, 0, DateTimeKind.Unspecified), 4, 9, 1, "28" },
                    { 15, "Azul", true, new DateTime(2025, 1, 2, 0, 0, 0, 0, DateTimeKind.Unspecified), 4, 9, 1, "30" },
                    { 16, "Beige", true, new DateTime(2025, 1, 2, 0, 0, 0, 0, DateTimeKind.Unspecified), 4, 7, 1, "30" },
                    { 17, "Negro", true, new DateTime(2025, 1, 2, 0, 0, 0, 0, DateTimeKind.Unspecified), 5, 8, 1, "28" },
                    { 18, "Beige", true, new DateTime(2025, 1, 2, 0, 0, 0, 0, DateTimeKind.Unspecified), 5, 8, 1, "28" },
                    { 19, "Negro", true, new DateTime(2025, 1, 2, 0, 0, 0, 0, DateTimeKind.Unspecified), 5, 7, 1, "30" },
                    { 20, "Beige", true, new DateTime(2025, 1, 2, 0, 0, 0, 0, DateTimeKind.Unspecified), 5, 7, 1, "30" },
                    { 21, "Blanco", true, new DateTime(2025, 1, 2, 0, 0, 0, 0, DateTimeKind.Unspecified), 6, 12, 2, "28" },
                    { 22, "Negro", true, new DateTime(2025, 1, 2, 0, 0, 0, 0, DateTimeKind.Unspecified), 6, 11, 2, "28" },
                    { 23, "Blanco", true, new DateTime(2025, 1, 2, 0, 0, 0, 0, DateTimeKind.Unspecified), 6, 12, 1, "30" },
                    { 24, "Negro", true, new DateTime(2025, 1, 2, 0, 0, 0, 0, DateTimeKind.Unspecified), 6, 10, 1, "30" },
                    { 25, "Blanco", true, new DateTime(2025, 1, 2, 0, 0, 0, 0, DateTimeKind.Unspecified), 7, 13, 5, "28" },
                    { 26, "Azul", true, new DateTime(2025, 1, 2, 0, 0, 0, 0, DateTimeKind.Unspecified), 7, 12, 5, "28" },
                    { 27, "Blanco", true, new DateTime(2025, 1, 2, 0, 0, 0, 0, DateTimeKind.Unspecified), 7, 13, 5, "30" },
                    { 28, "Azul", true, new DateTime(2025, 1, 2, 0, 0, 0, 0, DateTimeKind.Unspecified), 7, 12, 5, "30" },
                    { 29, "Blanco", true, new DateTime(2025, 1, 2, 0, 0, 0, 0, DateTimeKind.Unspecified), 8, 8, 3, "28" },
                    { 30, "Marrón", true, new DateTime(2025, 1, 2, 0, 0, 0, 0, DateTimeKind.Unspecified), 8, 7, 3, "28" },
                    { 31, "Blanco", true, new DateTime(2025, 1, 2, 0, 0, 0, 0, DateTimeKind.Unspecified), 8, 8, 2, "30" },
                    { 32, "Marrón", true, new DateTime(2025, 1, 2, 0, 0, 0, 0, DateTimeKind.Unspecified), 8, 7, 2, "30" },
                    { 33, "Beige", true, new DateTime(2025, 1, 2, 0, 0, 0, 0, DateTimeKind.Unspecified), 9, 8, 4, "26" },
                    { 34, "Azul", true, new DateTime(2025, 1, 2, 0, 0, 0, 0, DateTimeKind.Unspecified), 9, 7, 4, "26" },
                    { 35, "Beige", true, new DateTime(2025, 1, 2, 0, 0, 0, 0, DateTimeKind.Unspecified), 9, 8, 4, "28" },
                    { 36, "Azul", true, new DateTime(2025, 1, 2, 0, 0, 0, 0, DateTimeKind.Unspecified), 9, 7, 3, "28" },
                    { 37, "Beige", true, new DateTime(2025, 1, 2, 0, 0, 0, 0, DateTimeKind.Unspecified), 10, 15, 8, "36" },
                    { 38, "Rosado", true, new DateTime(2025, 1, 2, 0, 0, 0, 0, DateTimeKind.Unspecified), 10, 15, 8, "36" },
                    { 39, "Beige", true, new DateTime(2025, 1, 2, 0, 0, 0, 0, DateTimeKind.Unspecified), 10, 15, 7, "38" },
                    { 40, "Rosado", true, new DateTime(2025, 1, 2, 0, 0, 0, 0, DateTimeKind.Unspecified), 10, 15, 7, "38" },
                    { 41, "Azul", true, new DateTime(2025, 1, 2, 0, 0, 0, 0, DateTimeKind.Unspecified), 11, 25, 13, "28" },
                    { 42, "Azul Oscuro", true, new DateTime(2025, 1, 2, 0, 0, 0, 0, DateTimeKind.Unspecified), 11, 25, 13, "28" },
                    { 43, "Azul", true, new DateTime(2025, 1, 2, 0, 0, 0, 0, DateTimeKind.Unspecified), 11, 25, 12, "30" },
                    { 44, "Azul Oscuro", true, new DateTime(2025, 1, 2, 0, 0, 0, 0, DateTimeKind.Unspecified), 11, 25, 12, "30" },
                    { 45, "Azul", true, new DateTime(2025, 1, 2, 0, 0, 0, 0, DateTimeKind.Unspecified), 12, 23, 15, "28" },
                    { 46, "Azul Oscuro", true, new DateTime(2025, 1, 2, 0, 0, 0, 0, DateTimeKind.Unspecified), 12, 22, 15, "28" },
                    { 47, "Azul", true, new DateTime(2025, 1, 2, 0, 0, 0, 0, DateTimeKind.Unspecified), 12, 23, 15, "30" },
                    { 48, "Azul Oscuro", true, new DateTime(2025, 1, 2, 0, 0, 0, 0, DateTimeKind.Unspecified), 12, 22, 15, "30" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Administradores_UsuarioId",
                table: "Administradores",
                column: "UsuarioId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AlertaStock_ProductoVarianteId",
                table: "AlertaStock",
                column: "ProductoVarianteId");

            migrationBuilder.CreateIndex(
                name: "IX_Clientes_UsuarioId",
                table: "Clientes",
                column: "UsuarioId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CuponDescuento_CodigoCupon",
                table: "CuponDescuento",
                column: "CodigoCupon",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DetallesVentas_ProductoId",
                table: "DetallesVentas",
                column: "ProductoId");

            migrationBuilder.CreateIndex(
                name: "IX_DetallesVentas_VentaId",
                table: "DetallesVentas",
                column: "VentaId");

            migrationBuilder.CreateIndex(
                name: "IX_Envios_AdministradorId",
                table: "Envios",
                column: "AdministradorId");

            migrationBuilder.CreateIndex(
                name: "IX_Envios_VentaId",
                table: "Envios",
                column: "VentaId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Productos_CategoriaId",
                table: "Productos",
                column: "CategoriaId");

            migrationBuilder.CreateIndex(
                name: "IX_Productos_ProveedorId",
                table: "Productos",
                column: "ProveedorId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductoVariantes_ProductoId_Talla_Color",
                table: "ProductoVariantes",
                columns: new[] { "ProductoId", "Talla", "Color" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Ventas_ClienteId",
                table: "Ventas",
                column: "ClienteId");

            migrationBuilder.CreateIndex(
                name: "IX_Ventas_CuponDescuentoId",
                table: "Ventas",
                column: "CuponDescuentoId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AlertaStock");

            migrationBuilder.DropTable(
                name: "DetallesVentas");

            migrationBuilder.DropTable(
                name: "Envios");

            migrationBuilder.DropTable(
                name: "ProductoVariantes");

            migrationBuilder.DropTable(
                name: "Administradores");

            migrationBuilder.DropTable(
                name: "Ventas");

            migrationBuilder.DropTable(
                name: "Productos");

            migrationBuilder.DropTable(
                name: "Clientes");

            migrationBuilder.DropTable(
                name: "CuponDescuento");

            migrationBuilder.DropTable(
                name: "Categorias");

            migrationBuilder.DropTable(
                name: "Proveedores");

            migrationBuilder.DropTable(
                name: "Usuarios");
        }
    }
}
