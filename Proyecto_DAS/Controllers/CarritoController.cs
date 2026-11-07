using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Proyecto_DAS.Data;
using Proyecto_DAS.Models;
using Proyecto_DAS.Services;
using Proyecto_DAS.ViewModels.Carrito;

namespace Proyecto_DAS.Controllers
{
    public class CarritoController : Controller
    {
        private readonly AppDbContext _context;
        private readonly BoletaService _boletaService;
        private readonly EmailService _emailService;
        private readonly CarritoService _carritoService;
        private readonly IWebHostEnvironment _environment;

        public CarritoController(AppDbContext context, BoletaService boletaService, EmailService emailService, CarritoService carritoService, IWebHostEnvironment environment)
        {
            _context = context;
            _boletaService = boletaService;
            _emailService = emailService;
            _carritoService = carritoService;
            _environment = environment;
        }

        [HttpGet]
        public IActionResult Index()
        {
            var carrito = _carritoService.ObtenerCarrito();
            return View(carrito);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Agregar(int productoVarianteId, int cantidad = 1)
        {
            var variante = _context.ProductoVariantes.Include(proVr => proVr.Producto).ThenInclude(pro => pro.Categoria).FirstOrDefault(proVr => proVr.IdProductoVariante == productoVarianteId);

            if (variante == null)
            {
                TempData["ErrorMessage"] = "Variante no encontrada";
                return RedirectToAction("Index", "ClientePanel");
            }

            if (cantidad > variante.StockActual)
            {
                TempData["ErrorMessage"] = $"Stock insuficiente. Solo hay {variante.StockActual} unidades de Talla {variante.Talla} - { variante.Color}";
                return RedirectToAction("Index", "ClientePanel");
            }

            var carrito = _carritoService.ObtenerCarrito();
            var itemExistente = carrito.FirstOrDefault(c => c.ProductoVarianteId == productoVarianteId);

            if (itemExistente != null)
            {
                if (itemExistente.Cantidad + cantidad > variante.StockActual)
                {
                    TempData["ErrorMessage"] = $"No puedes agregar más. Ya tienes {itemExistente.Cantidad} en el carrito y solo hay {variante.StockActual} disponibles";
                    return RedirectToAction("Index", "ClientePanel");
                }
                itemExistente.Cantidad += cantidad;
            }
            else
            {
                carrito.Add(new CarritoItemViewModel
                {
                    ProductoId = variante.Producto.IdProducto,
                    ProductoVarianteId = variante.IdProductoVariante,
                    NombreProducto = variante.Producto.Nombre,
                    ImagenUrl = variante.Producto.ImagenUrl,
                    Categoria = variante.Producto.Categoria?.Nombre ?? "Sin categoría",
                    Color = variante.Color,
                    Talla = variante.Talla,
                    PrecioUnitario = variante.Producto.PrecioVenta,
                    Cantidad = cantidad,
                    StockDisponible = variante.StockActual
                });
            }

            _carritoService.GuardarCarrito(carrito);
            TempData["SuccessMessage"] = $"Agregado: {variante.Producto.Nombre} - Talla {variante.Talla} - {variante.Color}";
            return RedirectToAction("Index", "ClientePanel");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ActualizarCantidad(int productoVarianteId, int cantidad)
        {
            if (cantidad < 1)
            {
                return RedirectToAction("Eliminar", new { productoVarianteId });
            }

            var variante = _context.ProductoVariantes
                .Include(v => v.Producto)
                .FirstOrDefault(v => v.IdProductoVariante == productoVarianteId);

            if (variante == null)
            {
                TempData["ErrorMessage"] = "Variante no encontrada";
                return RedirectToAction("Index");
            }

            if (cantidad > variante.StockActual)
            {
                TempData["ErrorMessage"] = $"Stock insuficiente. Solo hay {variante.StockActual} unidades de Talla {variante.Talla} - {variante.Color}";
                return RedirectToAction("Index");
            }

            var carrito = _carritoService.ObtenerCarrito();
            var item = carrito.FirstOrDefault(c => c.ProductoVarianteId == productoVarianteId);

            if (item != null)
            {
                item.Cantidad = cantidad;
                item.StockDisponible = variante.StockActual;
                _carritoService.GuardarCarrito(carrito);
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Eliminar(int productoVarianteId)
        {
            var carrito = _carritoService.ObtenerCarrito();
            var item = carrito.FirstOrDefault(c => c.ProductoVarianteId == productoVarianteId);

            if (item != null)
            {
                carrito.Remove(item);
                _carritoService.GuardarCarrito(carrito);
                TempData["SuccessMessage"] = "Producto eliminado del carrito";
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Vaciar()
        {
            _carritoService.LimpiarCarrito();
            TempData["SuccessMessage"] = "Carrito vaciado";
            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult ObtenerCantidad()
        {
            var carrito = _carritoService.ObtenerCarrito();
            var cantidad = carrito.Sum(c => c.Cantidad);
            return Json(new { cantidad });
        }

        [HttpGet]
        public async Task<IActionResult> Checkout()
        {
            var clienteId = HttpContext.Session.GetInt32("ClienteId");
            if (clienteId == null)
            {
                TempData["ErrorMessage"] = "Debes iniciar sesión para realizar la compra";
                return RedirectToAction("Login", "Auth");
            }

            var carrito = _carritoService.ObtenerCarrito();
            if (!carrito.Any())
            {
                TempData["ErrorMessage"] = "El carrito está vacío";
                return RedirectToAction("Index");
            }

            // Validar stock de cada variante en el carrito
            foreach (var item in carrito)
            {
                var variante = _context.ProductoVariantes
                    .Include(v => v.Producto)
                    .FirstOrDefault(v => v.IdProductoVariante == item.ProductoVarianteId);

                if (variante == null || variante.Producto == null)
                {
                    TempData["ErrorMessage"] = $"El producto '{item.NombreProducto}' (Talla {item.Talla} - {item.Color}) ya no está disponible";
                    return RedirectToAction("Index");
                }

                if (!variante.Estado || !variante.Producto.Estado)
                {
                    TempData["ErrorMessage"] = $"El producto '{item.NombreProducto}' (Talla {item.Talla} - {item.Color}) ya no está disponible";
                    return RedirectToAction("Index");
                }

                if (item.Cantidad > variante.StockActual)
                {
                    TempData["ErrorMessage"] = $"Stock insuficiente para '{item.NombreProducto}' Talla {item.Talla} - {item.Color}. Solo hay {variante.StockActual} disponibles";
                    return RedirectToAction("Index");
                }

                // Actualizar stock disponible en el carrito
                item.StockDisponible = variante.StockActual;
            }
            _carritoService.GuardarCarrito(carrito);

            var cliente = await _context.Clientes
                .Include(c => c.Usuario)
                .FirstOrDefaultAsync(c => c.IdCliente == clienteId);

            if (cliente != null)
            {
                ViewBag.DireccionRegistrada = cliente.Direccion;
                ViewBag.CiudadRegistrada = cliente.Ciudad;
                ViewBag.CodigoPostalRegistrado = cliente.CodigoPostal;
            }

            return View(carrito);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ConfirmarCompra(
            string direccionEnvio,
            string ciudadEnvio,
            string codigoPostalEnvio,
            bool usarDireccionRegistrada = true,
            string metodoPago = "Tarjeta",
            string celularYape = null,
            string codigoOperacionYape = null,
            string numeroTarjeta = null,
            string nombreTarjeta = null,
            string expiracionTarjeta = null,
            string cvvTarjeta = null,
            string codigoCupon = null)
        {
            var clienteId = HttpContext.Session.GetInt32("ClienteId");
            if (clienteId == null)
            {
                TempData["ErrorMessage"] = "Debes iniciar sesión para realizar la compra";
                return RedirectToAction("Login", "Auth");
            }

            var carrito = _carritoService.ObtenerCarrito();
            if (!carrito.Any())
            {
                TempData["ErrorMessage"] = "El carrito está vacío";
                return RedirectToAction("Index");
            }

            // Obtener datos del cliente
            var cliente = await _context.Clientes
                .Include(c => c.Usuario)
                .FirstOrDefaultAsync(c => c.IdCliente == clienteId);

            if (cliente == null)
            {
                TempData["ErrorMessage"] = "Cliente no encontrado";
                return RedirectToAction("Index");
            }

            // Determinar qué dirección usar
            string direccionFinal, ciudadFinal, codigoPostalFinal;

            if (usarDireccionRegistrada)
            {
                direccionFinal = cliente.Direccion;
                ciudadFinal = cliente.Ciudad;
                codigoPostalFinal = cliente.CodigoPostal;
            }
            else
            {
                // Validar dirección temporal completa
                if (string.IsNullOrWhiteSpace(direccionEnvio) ||
                    string.IsNullOrWhiteSpace(ciudadEnvio) ||
                    string.IsNullOrWhiteSpace(codigoPostalEnvio))
                {
                    TempData["ErrorMessage"] = "Por favor completa todos los campos de la dirección de envío";
                    return RedirectToAction("Checkout");
                }

                direccionFinal = direccionEnvio.Trim();
                ciudadFinal = ciudadEnvio.Trim();
                codigoPostalFinal = codigoPostalEnvio.Trim();
            }

            if (metodoPago != "Tarjeta" && metodoPago != "Yape")
            {
                TempData["ErrorMessage"] = "Método de pago no válido";
                return RedirectToAction("Checkout");
            }

            if (metodoPago == "Tarjeta")
            {
                if (string.IsNullOrWhiteSpace(numeroTarjeta) || string.IsNullOrWhiteSpace(nombreTarjeta) ||
                    string.IsNullOrWhiteSpace(expiracionTarjeta) || string.IsNullOrWhiteSpace(cvvTarjeta))
                {
                    TempData["ErrorMessage"] = "Por favor completa todos los datos de la tarjeta";
                    return RedirectToAction("Checkout");
                }

                numeroTarjeta = numeroTarjeta.Replace(" ", "").Trim();
                nombreTarjeta = nombreTarjeta.Trim();
                expiracionTarjeta = expiracionTarjeta.Trim();
                cvvTarjeta = cvvTarjeta.Trim();

                if (!System.Text.RegularExpressions.Regex.IsMatch(numeroTarjeta, @"^[0-9]{16}$"))
                {
                    TempData["ErrorMessage"] = "El número de tarjeta debe tener exactamente 16 dígitos";
                    return RedirectToAction("Checkout");
                }

                if (!System.Text.RegularExpressions.Regex.IsMatch(cvvTarjeta, @"^[0-9]{3}$"))
                {
                    TempData["ErrorMessage"] = "El CVV debe tener exactamente 3 dígitos";
                    return RedirectToAction("Checkout");
                }

                if (nombreTarjeta.Length < 5)
                {
                    TempData["ErrorMessage"] = "El nombre en la tarjeta debe tener al menos 5 caracteres";
                    return RedirectToAction("Checkout");
                }

                if (!System.Text.RegularExpressions.Regex.IsMatch(expiracionTarjeta, @"^(0[1-9]|1[0-2])\/\d{2}$"))
                {
                    TempData["ErrorMessage"] = "La fecha de expiración debe tener el formato MM/AA";
                    return RedirectToAction("Checkout");
                }

                var partes = expiracionTarjeta.Split('/');
                var mesExpiracion = int.Parse(partes[0]);
                var anioExpiracion = 2000 + int.Parse(partes[1]);

                var fechaActual = DateTime.Now;
                var mesActual = fechaActual.Month;
                var anioActual = fechaActual.Year;

                if (anioExpiracion < anioActual || (anioExpiracion == anioActual && mesExpiracion < mesActual))
                {
                    TempData["ErrorMessage"] = $"La tarjeta está vencida. Fecha de expiración: {expiracionTarjeta}";
                    return RedirectToAction("Checkout");
                }
                
                if (anioExpiracion > anioActual + 5 || (anioExpiracion == anioActual + 5 && mesExpiracion > mesActual)) {
                    TempData["ErrorMessage"] = $"La tarjeta no puede tener una vigencia mayor a {mesActual}/{anioActual + 5}";
                    return RedirectToAction("Checkout");
                }

                if (mesExpiracion < 1 || mesExpiracion > 12)
                {
                    TempData["ErrorMessage"] = "El mes de expiración no es válido";
                    return RedirectToAction("Checkout");
                }

            }

            if (metodoPago == "Yape")
            {
                if (string.IsNullOrWhiteSpace(celularYape) || string.IsNullOrWhiteSpace(codigoOperacionYape))
                {
                    TempData["ErrorMessage"] = "Por favor completa todos los datos de Yape";
                    return RedirectToAction("Checkout");
                }

                celularYape = celularYape.Trim();
                codigoOperacionYape = codigoOperacionYape.Trim();

                if (!System.Text.RegularExpressions.Regex.IsMatch(celularYape, @"^9[0-9]{8}$"))
                {
                    TempData["ErrorMessage"] = "El número de celular debe empezar con 9 y tener exactamente 9 dígitos";
                    return RedirectToAction("Checkout");
                }

                if (!System.Text.RegularExpressions.Regex.IsMatch(codigoOperacionYape, @"^[0-9]{6}$"))
                {
                    TempData["ErrorMessage"] = "El código de operación debe tener exactamente 6 dígitos";
                    return RedirectToAction("Checkout");
                }
            }

            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                // Calcular subtotal (los precios ya incluyen IGV)
                decimal subtotalConIGV = carrito.Sum(item => item.PrecioUnitario * item.Cantidad);
                decimal baseImponible = subtotalConIGV / 1.18m; 
                decimal igv = subtotalConIGV - baseImponible;
                decimal totalVenta = subtotalConIGV;
                int? cuponDescuentoId = null;

                // Validar y aplicar cupón si existe
                if (!string.IsNullOrWhiteSpace(codigoCupon))
                {
                    var cupon = await _context.CuponDescuento
                        .FirstOrDefaultAsync(c => c.CodigoCupon == codigoCupon.ToUpper().Trim());

                    if (cupon != null && cupon.Estado &&
                        cupon.FechaInicio <= DateTime.Now &&
                        cupon.FechaVencimiento >= DateTime.Now)
                    {
                        // Aplicar descuento sobre el subtotal con IGV
                        decimal descuento = subtotalConIGV * cupon.PorcentajeDescuento / 100;
                        totalVenta = subtotalConIGV - descuento;
                        // Recalcular IGV sobre el nuevo total
                        baseImponible = totalVenta / 1.18m;
                        igv = totalVenta - baseImponible;
                        cuponDescuentoId = cupon.IdCuponDescuento;
                    }
                }

                // Crear la Venta
                var venta = new Models.Venta
                {
                    FechaVenta = DateTime.Now,
                    SubtotalSinDescuento = subtotalConIGV,
                    IGV = Math.Round(igv, 2),
                    TotalVenta = Math.Round(totalVenta, 2),
                    MetodoPago = metodoPago,
                    EstadoPago = "Completada",
                    ClienteId = clienteId.Value,
                    CuponDescuentoId = cuponDescuentoId
                };

                _context.Ventas.Add(venta);
                await _context.SaveChangesAsync();

                // Crear los DetalleVenta y reducir stock de variantes
                foreach (var item in carrito)
                {
                    var variante = await _context.ProductoVariantes
                        .Include(v => v.Producto)
                        .FirstOrDefaultAsync(v => v.IdProductoVariante == item.ProductoVarianteId);

                    if (variante == null || variante.Producto == null)
                    {
                        throw new Exception($"Producto '{item.NombreProducto}' no encontrado");
                    }

                    if (item.Cantidad > variante.StockActual)
                    {
                        throw new Exception($"Stock insuficiente para '{item.NombreProducto}' Talla {variante.Talla} - {variante.Color}");
                    }

                    // Crear detalle con talla y color de la variante comprada
                    var detalle = new Models.DetalleVenta
                    {
                        VentaId = venta.IdVenta,
                        ProductoId = item.ProductoId,
                        Cantidad = item.Cantidad,
                        PrecioUnitario = item.PrecioUnitario,
                        SubTotal = item.PrecioUnitario * item.Cantidad,
                        Talla = variante.Talla,
                        Color = variante.Color
                    };

                    _context.DetallesVentas.Add(detalle);

                    // Reducir stock de la variante
                    variante.StockActual -= item.Cantidad;

                    // Generar alerta si el stock queda bajo
                    if (variante.StockActual <= variante.StockMinimo)
                    {
                        var alertaExistente = await _context.AlertaStock
                            .FirstOrDefaultAsync(a => a.ProductoVarianteId == variante.IdProductoVariante && a.Estado == "Pendiente");

                        if (alertaExistente == null)
                        {
                            var alerta = new AlertaStock
                            {
                                ProductoVarianteId = variante.IdProductoVariante,
                                FechaAlerta = DateTime.Now,
                                Descripcion = $"Stock bajo para {variante.Producto.Nombre} - Talla: {variante.Talla}, Color: {variante.Color}. Stock actual: {variante.StockActual}, Minimo: {variante.StockMinimo}",
                                Estado = "Pendiente"
                            };
                            _context.AlertaStock.Add(alerta);
                        }
                    }
                }

                await _context.SaveChangesAsync();

                // Envío dirección seleccionada
                var envio = new Models.Envio
                {
                    VentaId = venta.IdVenta,
                    DireccionEnvio = direccionFinal,
                    Ciudad = ciudadFinal,
                    CodigoPostal = codigoPostalFinal,
                    FechaEnvio = DateTime.Now,
                    FechaEntregaEstimada = DateTime.Now.AddDays(5),
                    EstadoEnvio = "pendiente",
                    EmpresaTransporte = "YUNAN Express",
                    NumeroSeguimiento = $"YUN-{DateTime.Now:yyyyMMdd}-{venta.IdVenta:D4}"
                };

                _context.Envios.Add(envio);
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();

                // Generar boleta PDF y enviar por email
                bool emailEnviado = false;
                try
                {
                    var ventaCompleta = await _context.Ventas
                        .Include(v => v.DetallesVenta)
                            .ThenInclude(d => d.Producto)
                        .Include(v => v.Cliente)
                            .ThenInclude(c => c.Usuario)
                        .Include(v => v.Envio)
                        .Include(v => v.CuponDescuento)
                        .FirstOrDefaultAsync(v => v.IdVenta == venta.IdVenta);

                    if (ventaCompleta != null)
                    {
                        var pdfBytes = _boletaService.GenerarBoletaPDF(ventaCompleta, ventaCompleta.Cliente, ventaCompleta.Cliente.Usuario);

                        var boletasPath = Path.Combine(_environment.WebRootPath, "Boletas");
                        var fileName = $"Boleta_{venta.IdVenta}.pdf";
                        var filePath = Path.Combine(boletasPath, fileName);

                        await System.IO.File.WriteAllBytesAsync(filePath, pdfBytes);

                        HttpContext.Session.SetInt32("UltimaBoletaId", venta.IdVenta);

                        var emailCliente = ventaCompleta.Cliente.Usuario.Email;
                        var nombreCliente = $"{ventaCompleta.Cliente.Usuario.Nombres} {ventaCompleta.Cliente.Usuario.Apellidos}";

                        emailEnviado = await _emailService.EnviarBoletaPorEmailAsync(
                            emailCliente,
                            nombreCliente,
                            pdfBytes,
                            venta.IdVenta
                        );
                    }
                }
                catch (Exception pdfEx)
                {
                    TempData["ErrorMessage"] = $"Error generando PDF: {pdfEx.Message}";
                }

                _carritoService.LimpiarCarrito();

                if (emailEnviado)
                {
                    TempData["EmailEnviado"] = true;
                }
                else
                {
                    TempData["EmailNoEnviado"] = true;
                }

                return RedirectToAction("MisCompras", "ClientePanel");
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                TempData["ErrorMessage"] = $"Error al procesar la compra: {ex.Message}";
                return RedirectToAction("Index");
            }
        }

        // Validar cupón de descuento
        [HttpPost]
        public async Task<IActionResult> ValidarCupon(string codigoCupon)
        {
            if (string.IsNullOrWhiteSpace(codigoCupon))
            {
                return Json(new { valido = false, mensaje = "Debes ingresar un código de cupón" });
            }

            var cupon = await _context.CuponDescuento
                .FirstOrDefaultAsync(c => c.CodigoCupon == codigoCupon.ToUpper().Trim());

            if (cupon == null)
            {
                return Json(new { valido = false, mensaje = "El cupón no existe" });
            }

            if (!cupon.Estado)
            {
                return Json(new { valido = false, mensaje = "El cupón está inactivo" });
            }

            if (cupon.FechaInicio > DateTime.Now)
            {
                return Json(new { valido = false, mensaje = $"El cupón estará disponible desde {cupon.FechaInicio:dd/MM/yyyy}" });
            }

            if (cupon.FechaVencimiento < DateTime.Now)
            {
                return Json(new { valido = false, mensaje = $"El cupón venció el {cupon.FechaVencimiento:dd/MM/yyyy}" });
            }

            return Json(new
            {
                valido = true,
                cupon = new
                {
                    codigoCupon = cupon.CodigoCupon,
                    porcentajeDescuento = cupon.PorcentajeDescuento
                }
            });
        }

        // Descargar boleta
        [HttpGet]
        public async Task<IActionResult> DescargarBoleta(int id)
        {
            try
            {
                var venta = await _context.Ventas
                    .Include(v => v.DetallesVenta)
                        .ThenInclude(d => d.Producto)
                    .Include(v => v.Cliente)
                        .ThenInclude(c => c.Usuario)
                    .Include(v => v.Envio)
                    .Include(v => v.CuponDescuento)
                    .FirstOrDefaultAsync(v => v.IdVenta == id);

                if (venta == null)
                {
                    TempData["ErrorMessage"] = "Venta no encontrada";
                    return RedirectToAction("MisCompras", "ClientePanel");
                }

                var clienteId = HttpContext.Session.GetInt32("ClienteId");
                var rol = HttpContext.Session.GetString("Rol");

                if (rol != "Administrador" && venta.ClienteId != clienteId)
                {
                    TempData["ErrorMessage"] = "No tienes permiso para descargar esta boleta";
                    return RedirectToAction("MisCompras", "ClientePanel");
                }

                var boletasPath = Path.Combine(_environment.WebRootPath, "Boletas");
                var fileName = $"Boleta_{id}.pdf";
                var filePath = Path.Combine(boletasPath, fileName);

                byte[] pdfBytes;

                if (System.IO.File.Exists(filePath))
                {
                    pdfBytes = await System.IO.File.ReadAllBytesAsync(filePath);
                }
                else
                {
                    pdfBytes = _boletaService.GenerarBoletaPDF(venta, venta.Cliente, venta.Cliente.Usuario);

                    await System.IO.File.WriteAllBytesAsync(filePath, pdfBytes);
                }

                return File(pdfBytes, "application/pdf", $"Boleta_YUNAN_{id}_{DateTime.Now:yyyyMMdd}.pdf");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error al descargar la boleta: {ex.Message}";
                return RedirectToAction("MisCompras", "ClientePanel");
            }
        }
    }
}
