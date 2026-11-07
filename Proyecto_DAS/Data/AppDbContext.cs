using Microsoft.EntityFrameworkCore;
using Proyecto_DAS.Models;
using System.Diagnostics.Metrics;

namespace Proyecto_DAS.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }
        public DbSet<Administrador> Administradores { get; set; }
        public DbSet<AlertaStock> AlertaStock { get; set; }
        public DbSet<Categoria> Categorias { get; set; }
        public DbSet<Cliente> Clientes { get; set; }
        public DbSet<DetalleVenta> DetallesVentas { get; set; }
        public DbSet<Envio> Envios { get; set; }
        public DbSet<Producto> Productos { get; set; }
        public DbSet<ProductoVariante> ProductoVariantes { get; set; }
        public DbSet<Proveedor> Proveedores { get; set; }
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Venta> Ventas { get; set; }
        public DbSet<CuponDescuento> CuponDescuento { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Usuario 1:0..1 Cliente
            modelBuilder.Entity<Cliente>()
                .HasOne(c => c.Usuario)
                .WithOne(u => u.Cliente)
                .HasForeignKey<Cliente>(c => c.UsuarioId);

            // Usuario 1:0..1 Administrador
            modelBuilder.Entity<Administrador>()
                .HasOne(a => a.Usuario)
                .WithOne(u => u.Administrador)
                .HasForeignKey<Administrador>(a => a.UsuarioId);

            // Categoria 1:N Producto
            modelBuilder.Entity<Producto>()
                .HasOne(p => p.Categoria)
                .WithMany(c => c.Productos)
                .HasForeignKey(p => p.CategoriaId);

            // Proveedor 1:N Producto
            modelBuilder.Entity<Producto>()
                .HasOne(p => p.Proveedor)
                .WithMany(pr => pr.Productos)
                .HasForeignKey(p => p.ProveedorId);

            // Cliente 1:N Venta
            modelBuilder.Entity<Venta>()
                .HasOne(v => v.Cliente)
                .WithMany(c => c.Ventas)
                .HasForeignKey(v => v.ClienteId);

            // Venta 1:N DetalleVenta
            modelBuilder.Entity<DetalleVenta>()
                .HasOne(dv => dv.Venta)
                .WithMany(v => v.DetallesVenta)
                .HasForeignKey(dv => dv.VentaId);

            // Producto 1:N DetalleVenta
            modelBuilder.Entity<DetalleVenta>()
                .HasOne(dv => dv.Producto)
                .WithMany(p => p.DetallesVenta)
                .HasForeignKey(dv => dv.ProductoId)
                .OnDelete(DeleteBehavior.NoAction);

            // Venta 1:0..1 Envio
            modelBuilder.Entity<Envio>()
                .HasOne(e => e.Venta)
                .WithOne(v => v.Envio)
                .HasForeignKey<Envio>(e => e.VentaId);

            // Administrador 1:N Envio
            modelBuilder.Entity<Envio>()
                .HasOne(e => e.Administrador)
                .WithMany(a => a.Envios)
                .HasForeignKey(e => e.AdministradorId)
                .OnDelete(DeleteBehavior.NoAction);

            // ProductoVariante 1:N AlertaStock
            modelBuilder.Entity<AlertaStock>()
                .HasOne(a => a.ProductoVariante)
                .WithMany()
                .HasForeignKey(a => a.ProductoVarianteId);

            // Producto 1:N ProductoVariante
            modelBuilder.Entity<ProductoVariante>()
                .HasOne(pv => pv.Producto)
                .WithMany(p => p.Variantes)
                .HasForeignKey(pv => pv.ProductoId)
                .OnDelete(DeleteBehavior.Cascade);

            // Índice único para evitar duplicados de Talla-Color por Producto
            modelBuilder.Entity<ProductoVariante>()
                .HasIndex(pv => new { pv.ProductoId, pv.Talla, pv.Color })
                .IsUnique();

            // CuponDescuento 1:N Venta (un cupon puede usarse en multiples ventas)
            modelBuilder.Entity<Venta>()
                .HasOne(v => v.CuponDescuento)
                .WithMany(c => c.Ventas)
                .HasForeignKey(v => v.CuponDescuentoId)
                .OnDelete(DeleteBehavior.SetNull);

            // Indice unico para CodigoCupon
            modelBuilder.Entity<CuponDescuento>()
                .HasIndex(c => c.CodigoCupon)
                .IsUnique();


            // Contraseñas hasheadas con SHA256:
        

            var fechaRegistroUsuarios = new DateTime(2025, 1, 3);

            // 1 Administrador
            modelBuilder.Entity<Usuario>().HasData(
                new Usuario
                {
                    IdUsuario = 1,
                    Nombres = "Admin",
                    Apellidos = "Principal",
                    Email = "admin@yunan.com",
                    Contrasenia = "931351bc4cc867cfbace04e0131451d0e2231049a2ba9bd4a2b0b398d73e6fe6",
                    Rol = "Administrador",
                    Celular = "999888777",
                    FechaRegistro = fechaRegistroUsuarios,
                    Estado = true
                },
                // 3 Clientes
                new Usuario
                {
                    IdUsuario = 2,
                    Nombres = "María",
                    Apellidos = "García López",
                    Email = "sentopasword@gmail.com",
                    Contrasenia = "54803ff75c237cd234118278e12a76979002cb298e18a088ef9cfbc63fdb3a3c",
                    Rol = "Cliente",
                    Celular = "987654321",
                    FechaRegistro = fechaRegistroUsuarios,
                    Estado = true
                },
                new Usuario
                {
                    IdUsuario = 3,
                    Nombres = "Carlos",
                    Apellidos = "Rodríguez Pérez",
                    Email = "einar2014idsis@gmail.com",
                    Contrasenia = "54803ff75c237cd234118278e12a76979002cb298e18a088ef9cfbc63fdb3a3c",
                    Rol = "Cliente",
                    Celular = "976543210",
                    FechaRegistro = fechaRegistroUsuarios,
                    Estado = true
                }
            );

            // Carga de Administrador
            modelBuilder.Entity<Administrador>().HasData(
                new Administrador
                {
                    IdAdministrador = 1,
                    UsuarioId = 1,
                    Cargo = "Gerente General",
                    FechaContratacion = new DateTime(2025, 1, 1)
                }
            );

            // Carga de Clientes
            modelBuilder.Entity<Cliente>().HasData(
                new Cliente
                {
                    IdCliente = 1,
                    UsuarioId = 2,
                    Direccion = "Av. Larco 123",
                    Ciudad = "Trujillo",
                    CodigoPostal = "13001",
                    DniRuc = "12345678",
                    FechaNacimiento = new DateTime(1990, 5, 15)
                },
                new Cliente
                {
                    IdCliente = 2,
                    UsuarioId = 3,
                    Direccion = "Jr. Pizarro 456",
                    Ciudad = "Trujillo",
                    CodigoPostal = "13002",
                    DniRuc = "23456789",
                    FechaNacimiento = new DateTime(1985, 8, 20)
                }
            );

            // Carga de Categorías
            modelBuilder.Entity<Categoria>().HasData(
                new Categoria { IdCategoria = 1, Nombre = "Short Falda", Descripcion = "Combina short y falda, estilo casual", Estado = true },
                new Categoria { IdCategoria = 2, Nombre = "Faldas", Descripcion = "Mini faldas clásicas y modernas, variadas", Estado = true },
                new Categoria { IdCategoria = 3, Nombre = "Casacas", Descripcion = "Chaquetas ideales para un look", Estado = true },
                new Categoria { IdCategoria = 4, Nombre = "Pantalones", Descripcion = "Pitillo, Baggy o Cargo, para todo estilo", Estado = true },
                new Categoria { IdCategoria = 5, Nombre = "Short", Descripcion = "Shorts que resaltan tu belleza", Estado = true }
            );

            // Carga de Proveedores
            modelBuilder.Entity<Proveedor>().HasData(
                new Proveedor { 
                    IdProveedor = 1, 
                    NombreEmpresa = "Textiles del Norte SAC", 
                    ContactoNombre = "Roberto Sánchez", 
                    Celular = "963256369", 
                    Email = "ventas@textilesnorte.com", 
                    Direccion = "Av. Industrial 456, Trujillo", 
                    Ruc = "20123456789", 
                    Estado = true 
                },
                new Proveedor { 
                    IdProveedor = 2, 
                    NombreEmpresa = "Confecciones Lima EIRL", 
                    ContactoNombre = "Patricia Mendoza", 
                    Celular = "985632159", 
                    Email = "contacto@confeccioneslima.pe", 
                    Direccion = "Jr. Gamarra 1234, La Victoria", 
                    Ruc = "20234567890", 
                    Estado = true 
                },
                new Proveedor { 
                    IdProveedor = 3, 
                    NombreEmpresa = "Denim Perú SA", 
                    ContactoNombre = "Carlos Vega", 
                    Celular = "915935746", 
                    Email = "pedidos@denimperu.com", 
                    Direccion = "Av. Argentina 567, Callao", 
                    Ruc = "20345678901", 
                    Estado = true }
            );

            // Carga de Productos (stock se maneja en ProductoVariante)
            var fechaIni = new DateTime(2025, 1, 2);

            modelBuilder.Entity<Producto>().HasData(
                new Producto
                {
                    IdProducto = 1,
                    Nombre = "Pantalón Cargo",
                    Descripcion = "Wide Leg-Palazo Cargo.",
                    PrecioVenta = 44.00m,
                    PrecioCompra = 20.0m,
                    ImagenUrl = "~/img/Img_Productos/pa_car.jpg",
                    CategoriaId = 4,
                    ProveedorId = 1,
                    FechaRegistro = fechaIni,
                    Estado = true
                },
                new Producto {
                    IdProducto = 2,
                    Nombre = "Pantalón Baggy",
                    Descripcion = "Baggy con elastico y rayas blancas.",
                    PrecioVenta = 38.00m,
                    PrecioCompra = 20.15m,
                    ImagenUrl = "~/img/Img_Productos/pa_ba.jpg",
                    CategoriaId = 4,
                    ProveedorId = 1,
                    FechaRegistro = fechaIni,
                    Estado = true
                },
                new Producto {
                    IdProducto = 3,
                    Nombre = "Short Falda Casual",
                    Descripcion = "Short falda que te acompaña a cualquier lado.",
                    PrecioVenta = 35.00m,
                    PrecioCompra = 24.50m,
                    ImagenUrl = "~/img/Img_Productos/sh_fa_casu.jpg",
                    CategoriaId = 1,
                    ProveedorId = 2,
                    FechaRegistro = fechaIni,
                    Estado = true
                },
                new Producto {
                    IdProducto = 4,
                    Nombre = "Short Falda Urbana",
                    Descripcion = "Short falda para tus salidas casuales.",
                    PrecioVenta = 29.00m,
                    PrecioCompra = 15.80m,
                    ImagenUrl = "~/img/Img_Productos/sh_fa_urb.jpg",
                    CategoriaId = 1,
                    ProveedorId = 2,
                    FechaRegistro = fechaIni,
                    Estado = true
                },
                new Producto {
                    IdProducto = 5,
                    Nombre = "Short Falda 4 Botones",
                    Descripcion = "Short falda para tus fiestas imperdibles.",
                    PrecioVenta = 38.00m,
                    PrecioCompra = 26.60m,
                    ImagenUrl = "~/img/Img_Productos/sh_fa_fies.jpg",
                    CategoriaId = 1,
                    ProveedorId = 2,
                    FechaRegistro = fechaIni,
                    Estado = true
                },
                new Producto {
                    IdProducto = 6,
                    Nombre = "Casaca Diseñador",
                    Descripcion = "Casaca fresh terry.",
                    PrecioVenta = 35.00m,
                    PrecioCompra = 20.60m,
                    ImagenUrl = "~/img/Img_Productos/ca_nor.jpg",
                    CategoriaId = 3,
                    ProveedorId = 3,
                    FechaRegistro = fechaIni,
                    Estado = true
                },
                new Producto {
                    IdProducto = 7,
                    Nombre = "Casaca Mini Rasgada",
                    Descripcion = "Casaca jeans strech.",
                    PrecioVenta = 28.00m,
                    PrecioCompra = 15.80m,
                    ImagenUrl = "~/img/Img_Productos/ca_ras.jpg",
                    CategoriaId = 3,
                    ProveedorId = 3,
                    FechaRegistro = fechaIni,
                    Estado = true
                },
                new Producto {
                    IdProducto = 8,
                    Nombre = "Short Cargo",
                    Descripcion = "Short estilo cargo único.",
                    PrecioVenta = 32.00m,
                    PrecioCompra = 15.60m,
                    ImagenUrl = "~/img/Img_Productos/sh_estre.jpg",
                    CategoriaId = 5,
                    ProveedorId = 1,
                    FechaRegistro = fechaIni,
                    Estado = true
                },
                new Producto {
                    IdProducto = 9,
                    Nombre = "Short 1 Botón",
                    Descripcion = "Short jean strech.",
                    PrecioVenta = 23.00m,
                    PrecioCompra = 10.00m,
                    ImagenUrl = "~/img/Img_Productos/sh_1_bo.jpg",
                    CategoriaId = 5,
                    ProveedorId = 1,
                    FechaRegistro = fechaIni,
                    Estado = true
                },
                new Producto {
                    IdProducto = 10,
                    Nombre = "Short 3 Botones",
                    Descripcion = "Short de jeans drill strech.",
                    PrecioVenta = 24.00m,
                    PrecioCompra = 14.30m,
                    ImagenUrl = "~/img/Img_Productos/sh_3_bo.jpg",
                    CategoriaId = 5,
                    ProveedorId = 1,
                    FechaRegistro = fechaIni,
                    Estado = true
                },
                new Producto {
                    IdProducto = 11,
                    Nombre = "Falda Cargo",
                    Descripcion = "Falda con estilo cargo y 9 botones.",
                    PrecioVenta = 22.00m,
                    PrecioCompra = 10.00m,
                    ImagenUrl = "~/img/Img_Productos/fa_9_bo.jpg",
                    CategoriaId = 2,
                    ProveedorId = 2,
                    FechaRegistro = fechaIni,
                    Estado = true
                },
                new Producto {
                    IdProducto = 12,
                    Nombre = "Falda 3 Botones",
                    Descripcion = "Falda con 3 botones para el día a día.",
                    PrecioVenta = 22.00m,
                    PrecioCompra = 10.00m,
                    ImagenUrl = "~/img/Img_Productos/fa_3_bo.jpg",
                    CategoriaId = 2,
                    ProveedorId = 2,
                    FechaRegistro = fechaIni,
                    Estado = true
                }
            );

            int varId = 1;

            modelBuilder.Entity<ProductoVariante>().HasData(
                // Producto 1: Pantalón Cargo (Tallas: 28,30 | Colores: Marrón,Beige)
                new ProductoVariante { 
                    IdProductoVariante = varId++, 
                    ProductoId = 1, 
                    Talla = "28", 
                    Color = "Marrón", 
                    StockActual = 15, 
                    StockMinimo = 5, 
                    Estado = true, 
                    FechaRegistro = fechaIni
                },
                new ProductoVariante { 
                    IdProductoVariante = varId++, 
                    ProductoId = 1, 
                    Talla = "28", 
                    Color = "Beige", 
                    StockActual = 12, 
                    StockMinimo = 5, 
                    Estado = true, 
                    FechaRegistro = fechaIni
                },
                new ProductoVariante { 
                    IdProductoVariante = varId++, 
                    ProductoId = 1, 
                    Talla = "30", 
                    Color = "Marrón", 
                    StockActual = 18, 
                    StockMinimo = 5, 
                    Estado = true, 
                    FechaRegistro = fechaIni
                },
                new ProductoVariante { 
                    IdProductoVariante = varId++, 
                    ProductoId = 1, 
                    Talla = "30", 
                    Color = "Beige", 
                    StockActual = 15, 
                    StockMinimo = 5, 
                    Estado = true, 
                    FechaRegistro = fechaIni
                },

                // Producto 2: Pantalón Baggy (Tallas: 28,30 | Colores: Celeste,Verde)
                new ProductoVariante { 
                    IdProductoVariante = varId++, 
                    ProductoId = 2, 
                    Talla = "28", 
                    Color = "Celeste", 
                    StockActual = 6, 
                    StockMinimo = 2, 
                    Estado = true, 
                    FechaRegistro = fechaIni
                },
                new ProductoVariante { 
                    IdProductoVariante = varId++, 
                    ProductoId = 2, 
                    Talla = "28", 
                    Color = "Verde", 
                    StockActual = 5, 
                    StockMinimo = 1, 
                    Estado = true, 
                    FechaRegistro = fechaIni
                },
                new ProductoVariante { 
                    IdProductoVariante = varId++, 
                    ProductoId = 2, 
                    Talla = "30", 
                    Color = "Celeste", 
                    StockActual = 5, 
                    StockMinimo = 1, 
                    Estado = true, 
                    FechaRegistro = fechaIni
                },
                new ProductoVariante { 
                    IdProductoVariante = varId++, 
                    ProductoId = 2, 
                    Talla = "30", 
                    Color = "Verde", 
                    StockActual = 4, 
                    StockMinimo = 1, 
                    Estado = true, 
                    FechaRegistro = fechaIni
                },

                new ProductoVariante { 
                    IdProductoVariante = varId++, 
                    ProductoId = 3, 
                    Talla = "28", 
                    Color = "Verde", 
                    StockActual = 8, 
                    StockMinimo = 2, 
                    Estado = true, 
                    FechaRegistro = fechaIni
                },
                new ProductoVariante { 
                    IdProductoVariante = varId++, 
                    ProductoId = 3, 
                    Talla = "28", 
                    Color = "Negro", 
                    StockActual = 6, 
                    StockMinimo = 2, 
                    Estado = true, 
                    FechaRegistro = fechaIni
                },
                new ProductoVariante { 
                    IdProductoVariante = varId++, 
                    ProductoId = 3, 
                    Talla = "30", 
                    Color = "Verde", 
                    StockActual = 6, 
                    StockMinimo = 1, 
                    Estado = true, 
                    FechaRegistro = fechaIni
                },
                new ProductoVariante { 
                    IdProductoVariante = varId++, 
                    ProductoId = 3, 
                    Talla = "30", 
                    Color = "Negro", 
                    StockActual = 5, 
                    StockMinimo = 1, 
                    Estado = true, 
                    FechaRegistro = fechaIni
                },

                // Producto 4: Short Falda Urbana (Tallas: 28,30 | Colores: Azul,Beige)
                new ProductoVariante { 
                    IdProductoVariante = varId++, 
                    ProductoId = 4, 
                    Talla = "28", 
                    Color = "Azul", 
                    StockActual = 10, 
                    StockMinimo = 2, 
                    Estado = true, 
                    FechaRegistro = fechaIni
                },
                new ProductoVariante { 
                    IdProductoVariante = varId++, 
                    ProductoId = 4, 
                    Talla = "28", 
                    Color = "Beige", 
                    StockActual = 9, 
                    StockMinimo = 1, 
                    Estado = true, 
                    FechaRegistro = fechaIni 
                },
                new ProductoVariante { 
                    IdProductoVariante = varId++, 
                    ProductoId = 4, 
                    Talla = "30", 
                    Color = "Azul", 
                    StockActual = 9, 
                    StockMinimo = 1, 
                    Estado = true, 
                    FechaRegistro = fechaIni 
                },
                new ProductoVariante { 
                    IdProductoVariante = varId++, 
                    ProductoId = 4, 
                    Talla = "30", 
                    Color = "Beige", 
                    StockActual = 7, 
                    StockMinimo = 1, 
                    Estado = true, 
                    FechaRegistro = fechaIni 
                },

                // Producto 5: Short Falda 4 Botones (Tallas: 28,30 | Colores: Negro,Beige)
                new ProductoVariante { 
                    IdProductoVariante = varId++, 
                    ProductoId = 5, 
                    Talla = "28", 
                    Color = "Negro", 
                    StockActual = 8, 
                    StockMinimo = 1, 
                    Estado = true, 
                    FechaRegistro = fechaIni 
                },
                new ProductoVariante { 
                    IdProductoVariante = varId++, 
                    ProductoId = 5, 
                    Talla = "28", 
                    Color = "Beige", 
                    StockActual = 8, 
                    StockMinimo = 1, 
                    Estado = true, 
                    FechaRegistro = fechaIni 
                },
                new ProductoVariante { 
                    IdProductoVariante = varId++, 
                    ProductoId = 5, 
                    Talla = "30", 
                    Color = "Negro", 
                    StockActual = 7, 
                    StockMinimo = 1, 
                    Estado = true, 
                    FechaRegistro = fechaIni 
                },
                new ProductoVariante { 
                    IdProductoVariante = varId++, 
                    ProductoId = 5, 
                    Talla = "30", 
                    Color = "Beige", 
                    StockActual = 7,
                    StockMinimo = 1,
                    Estado = true,
                    FechaRegistro = fechaIni
                },

                // Producto 6: Casaca Diseñador (Tallas: 28,30 | Colores: Blanco,Negro)
                new ProductoVariante { 
                    IdProductoVariante = varId++,
                    ProductoId = 6, 
                    Talla = "28", 
                    Color = "Blanco", 
                    StockActual = 12, 
                    StockMinimo = 2, 
                    Estado = true, 
                    FechaRegistro = fechaIni 
                },
                new ProductoVariante { 
                    IdProductoVariante = varId++, 
                    ProductoId = 6, 
                    Talla = "28", 
                    Color = "Negro", 
                    StockActual = 11, 
                    StockMinimo = 2, 
                    Estado = true, 
                    FechaRegistro = fechaIni 
                },
                new ProductoVariante {
                    IdProductoVariante = varId++, 
                    ProductoId = 6, 
                    Talla = "30", 
                    Color = "Blanco", 
                    StockActual = 12, 
                    StockMinimo = 1, 
                    Estado = true, 
                    FechaRegistro = fechaIni 
                },
                new ProductoVariante { 
                    IdProductoVariante = varId++, 
                    ProductoId = 6, 
                    Talla = "30", 
                    Color = "Negro", 
                    StockActual = 10, 
                    StockMinimo = 1, 
                    Estado = true, 
                    FechaRegistro = fechaIni 
                },

                // Producto 7: Casaca Mini Rasgada (Tallas: 28,30 | Colores: Blanco,Azul)
                new ProductoVariante { 
                    IdProductoVariante = 
                    varId++, ProductoId = 7, 
                    Talla = "28", 
                    Color = "Blanco", 
                    StockActual = 13, 
                    StockMinimo = 5, 
                    Estado = true, 
                    FechaRegistro = fechaIni 
                },
                new ProductoVariante { 
                    IdProductoVariante = varId++, 
                    ProductoId = 7, 
                    Talla = "28", 
                    Color = "Azul", 
                    StockActual = 12, 
                    StockMinimo = 5, 
                    Estado = true, 
                    FechaRegistro = fechaIni 
                },
                new ProductoVariante { 
                    IdProductoVariante = varId++, 
                    ProductoId = 7, 
                    Talla = "30", 
                    Color = "Blanco", 
                    StockActual = 13, 
                    StockMinimo = 5, 
                    Estado = true, 
                    FechaRegistro = fechaIni 
                },
                new ProductoVariante { 
                    IdProductoVariante = varId++, 
                    ProductoId = 7, 
                    Talla = "30", 
                    Color = "Azul", 
                    StockActual = 12, 
                    StockMinimo = 5, 
                    Estado = true, 
                    FechaRegistro = fechaIni 
                },

                // Producto 8: Short Cargo (Tallas: 28,30 | Colores: Blanco,Marrón)
                new ProductoVariante { 
                    IdProductoVariante = varId++, 
                    ProductoId = 8, 
                    Talla = "28", 
                    Color = "Blanco", 
                    StockActual = 8, 
                    StockMinimo = 3, 
                    Estado = true, 
                    FechaRegistro = fechaIni 
                },
                new ProductoVariante { 
                    IdProductoVariante = varId++, 
                    ProductoId = 8, 
                    Talla = "28", 
                    Color = "Marrón", 
                    StockActual = 7, 
                    StockMinimo = 3, 
                    Estado = true, 
                    FechaRegistro = fechaIni
                },
                new ProductoVariante { 
                    IdProductoVariante = varId++, 
                    ProductoId = 8, 
                    Talla = "30", 
                    Color = "Blanco", 
                    StockActual = 8, 
                    StockMinimo = 2, 
                    Estado = true, 
                    FechaRegistro = fechaIni 
                },
                new ProductoVariante { 
                    IdProductoVariante = varId++, 
                    ProductoId = 8, 
                    Talla = "30", 
                    Color = "Marrón", 
                    StockActual = 7, 
                    StockMinimo = 2,
                    Estado = true, 
                    FechaRegistro = fechaIni
                },

                // Producto 9: Short 1 Botón (Tallas: 26,28 | Colores: Beige,Azul)
                new ProductoVariante {
                    IdProductoVariante = varId++,
                    ProductoId = 9,
                    Talla = "26", 
                    Color = "Beige", 
                    StockActual = 8,
                    StockMinimo = 4,
                    Estado = true, 
                    FechaRegistro = fechaIni
                },
                new ProductoVariante { 
                    IdProductoVariante = varId++, 
                    ProductoId = 9,
                    Talla = "26", 
                    Color = "Azul", 
                    StockActual = 7, 
                    StockMinimo = 4, 
                    Estado = true, 
                    FechaRegistro = fechaIni 
                },
                new ProductoVariante { 
                    IdProductoVariante = varId++, 
                    ProductoId = 9, 
                    Talla = "28", 
                    Color = "Beige", 
                    StockActual = 8, 
                    StockMinimo = 4, 
                    Estado = true, 
                    FechaRegistro = fechaIni
                },
                new ProductoVariante { 
                    IdProductoVariante = varId++, 
                    ProductoId = 9, Talla = "28", 
                    Color = "Azul", StockActual = 7, 
                    StockMinimo = 3, 
                    Estado = true, 
                    FechaRegistro = fechaIni },

                // Producto 10: Short 3 Botones (Tallas: 36,38 | Colores: Beige,Rosado)
                new ProductoVariante { 
                    IdProductoVariante = varId++, 
                    ProductoId = 10, 
                    Talla = "36", 
                    Color = "Beige", 
                    StockActual = 15, 
                    StockMinimo = 8, 
                    Estado = true, 
                    FechaRegistro = fechaIni
                },
                new ProductoVariante { 
                    IdProductoVariante = varId++, 
                    ProductoId = 10, 
                    Talla = "36", 
                    Color = "Rosado", 
                    StockActual = 15, 
                    StockMinimo = 8, 
                    Estado = true, 
                    FechaRegistro = fechaIni 
                },
                new ProductoVariante { 
                    IdProductoVariante = varId++,
                    ProductoId = 10, 
                    Talla = "38", 
                    Color = "Beige", 
                    StockActual = 15, 
                    StockMinimo = 7, 
                    Estado = true, 
                    FechaRegistro = fechaIni
                },
                new ProductoVariante { 
                    IdProductoVariante = varId++, 
                    ProductoId = 10, 
                    Talla = "38", 
                    Color = "Rosado", 
                    StockActual = 15, 
                    StockMinimo = 7, 
                    Estado = true, 
                    FechaRegistro = fechaIni 
                },

                // Producto 11: Falda Cargo (Tallas: 28,30 | Colores: Azul,Azul Oscuro)
                new ProductoVariante { 
                    IdProductoVariante = varId++, 
                    ProductoId = 11, 
                    Talla = "28", 
                    Color = "Azul", 
                    StockActual = 25, 
                    StockMinimo = 13, 
                    Estado = true, 
                    FechaRegistro = fechaIni 
                },
                new ProductoVariante { 
                    IdProductoVariante = varId++, 
                    ProductoId = 11, 
                    Talla = "28", 
                    Color = "Azul Oscuro", 
                    StockActual = 25, 
                    StockMinimo = 13, 
                    Estado = true, 
                    FechaRegistro = fechaIni 
                },
                new ProductoVariante { 
                    IdProductoVariante = varId++, 
                    ProductoId = 11, 
                    Talla = "30",
                    Color = "Azul",
                    StockActual = 25, 
                    StockMinimo = 12, 
                    Estado = true,
                    FechaRegistro = fechaIni 
                },
                new ProductoVariante { 
                    IdProductoVariante = varId++, 
                    ProductoId = 11, 
                    Talla = "30", 
                    Color = "Azul Oscuro", 
                    StockActual = 25, 
                    StockMinimo = 12, 
                    Estado = true, 
                    FechaRegistro = fechaIni
                },

                // Producto 12: Falda 3 Botones (Tallas: 28,30 | Colores: Azul,Azul Oscuro)
                new ProductoVariante { 
                    IdProductoVariante = varId++, 
                    ProductoId = 12, 
                    Talla = "28", 
                    Color = "Azul", 
                    StockActual = 23,
                    StockMinimo = 15, 
                    Estado = true, 
                    FechaRegistro = fechaIni
                },
                new ProductoVariante { 
                    IdProductoVariante = varId++,
                    ProductoId = 12,
                    Talla = "28", 
                    Color = "Azul Oscuro", 
                    StockActual = 22, 
                    StockMinimo = 15,
                    Estado = true,
                    FechaRegistro = fechaIni 
                },
                new ProductoVariante { 
                    IdProductoVariante = varId++,
                    ProductoId = 12, 
                    Talla = "30",
                    Color = "Azul", 
                    StockActual = 23, 
                    StockMinimo = 15, 
                    Estado = true, 
                    FechaRegistro = fechaIni 
                },
                new ProductoVariante { 
                    IdProductoVariante = varId++, 
                    ProductoId = 12,
                    Talla = "30", 
                    Color = "Azul Oscuro", 
                    StockActual = 22, 
                    StockMinimo = 15, 
                    Estado = true, 
                    FechaRegistro = fechaIni 
                }
            );
        }
    }
}
