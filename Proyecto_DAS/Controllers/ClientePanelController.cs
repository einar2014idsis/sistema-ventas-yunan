using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Proyecto_DAS.Data;
using Proyecto_DAS.Filters;
using Proyecto_DAS.Services;
using Proyecto_DAS.ViewModels.Carrito;
using Proyecto_DAS.ViewModels.ClientePanel;

namespace Proyecto_DAS.Controllers
{
    public class ClientePanelController : Controller
    {
        private readonly AppDbContext _context;
        private readonly CarritoService _carritoService;

        public ClientePanelController(AppDbContext context, CarritoService carritoService)
        {
            _context = context;
            _carritoService = carritoService;
        }

        [HttpGet]
        public IActionResult Index(int? categoriaId, string talla, string color, decimal? precioMin, decimal? precioMax, string busqueda, int pagina = 1)
        {
            const int productosPorPagina = 12;
            // Verificar que no sea un administrador intentando acceder
            var rol = HttpContext.Session.GetString("Rol");
            if (rol == "Administrador")
            {
                return RedirectToAction("PanelAdministrador", "Home");
            }

            if (precioMin.HasValue && precioMax.HasValue)
            {
                if (precioMin >= precioMax)
                {
                    TempData["ErrorMessage"] = "El precio mínimo debe ser menor que el precio máximo.";
                    return RedirectToAction("Index");
                }
            }

            if (precioMin.HasValue && precioMin < 0)
            {
                TempData["ErrorMessage"] = "El precio mínimo no puede ser negativo.";
                return RedirectToAction("Index");
            }

            if (precioMax.HasValue && precioMax < 0)
            {
                TempData["ErrorMessage"] = "El precio máximo no puede ser negativo.";
                return RedirectToAction("Index");
            }

            var consulta = _context.Productos
                .Include(producto => producto.Categoria)
                .Include(producto => producto.Variantes)
                .Where(producto => producto.Estado == true)
                .AsQueryable();

            // Aplicar filtros
            if (categoriaId.HasValue)
            {
                consulta = consulta.Where(producto => producto.CategoriaId == categoriaId);
            }

            if (!string.IsNullOrEmpty(talla))
            {
                consulta = consulta.Where(producto => producto.Variantes.Any(variante => variante.Talla == talla && variante.Estado && variante.StockActual > 0));
            }

            if (!string.IsNullOrEmpty(color))
            {
                consulta = consulta.Where(producto => producto.Variantes.Any(variante => variante.Color == color && variante.Estado && variante.StockActual > 0));
            }

            if (precioMin.HasValue)
            {
                consulta = consulta.Where(producto => producto.PrecioVenta >= precioMin);
            }

            if (precioMax.HasValue)
            {
                consulta = consulta.Where(producto => producto.PrecioVenta <= precioMax);
            }

            if (!string.IsNullOrEmpty(busqueda))
            {
                consulta = consulta.Where(producto => producto.Nombre.Contains(busqueda));
            }

            // Obtener listas
            var categorias = _context.Categorias
                .Where(cat => cat.Estado)
                .Select(cat => new SelectListItem { Value = cat.IdCategoria.ToString(), Text = cat.Nombre })
                .ToList();

            // Obtener tallas
            var tallas = _context.ProductoVariantes
                .Where(variante => variante.Estado && variante.StockActual > 0 && variante.Producto.Estado)
                .Select(variante => variante.Talla)
                .Distinct()
                .OrderBy(t => t)
                .Select(t => new SelectListItem { Value = t, Text = t })
                .ToList();

            // Obtener colores
            var colores = _context.ProductoVariantes
                .Where(variante => variante.Estado && variante.StockActual > 0 && variante.Producto.Estado)
                .Select(variante => variante.Color)
                .Distinct()
                .OrderBy(c => c)
                .Select(c => new SelectListItem { Value = c, Text = c })
                .ToList();

            // Contar total de productos
            var totalProductos = consulta.Count();

            // Validar página
            if (pagina < 1) pagina = 1;
            var totalPaginas = (int)Math.Ceiling((double)totalProductos / productosPorPagina);
            if (pagina > totalPaginas && totalPaginas > 0) pagina = totalPaginas;

            // Aplicar paginación
            var productosPaginados = consulta
                .OrderBy(producto => producto.Nombre)
                .Skip((pagina - 1) * productosPorPagina)
                .Take(productosPorPagina)
                .ToList();

            var viewModel = new CatalogoFilterViewModel
            {
                CategoriaId = categoriaId,
                Talla = talla,
                Color = color,
                PrecioMin = precioMin,
                PrecioMax = precioMax,
                Busqueda = busqueda,
                Categorias = categorias,
                Tallas = tallas,
                Colores = colores,
                Productos = productosPaginados,
                PaginaActual = pagina,
                ProductosPorPagina = productosPorPagina,
                TotalProductos = totalProductos
            };

            return View(viewModel);
        }

        [HttpPost]
        public IActionResult AgregarAlCarrito(int productoId, int cantidad = 1, string talla = "", string color = "")
        {
            var producto = _context.Productos
                .Include(producto => producto.Categoria)
                .Include(producto => producto.Variantes)
                .FirstOrDefault(producto => producto.IdProducto == productoId);

            if (producto == null)
            {
                return Json(new { success = false, message = "Producto no encontrado." });
            }

            // Validar que se haya seleccionado talla y color
            if (string.IsNullOrWhiteSpace(talla))
            {
                return Json(new { success = false, message = "Debes seleccionar una talla." });
            }

            if (string.IsNullOrWhiteSpace(color))
            {
                return Json(new { success = false, message = "Debes seleccionar un color." });
            }

            // Buscar la variante específica
            var variante = producto.Variantes
                .FirstOrDefault(v => v.Talla == talla && v.Color == color && v.Estado);

            if (variante == null)
            {
                return Json(new { success = false, message = "La combinación de talla y color seleccionada no está disponible." });
            }

            if (cantidad > variante.StockActual)
            {
                return Json(new { success = false, message = $"Stock insuficiente. Solo hay {variante.StockActual} unidades disponibles." });
            }

            var carrito = _carritoService.ObtenerCarrito();

            // Buscar si ya existe el mismo producto con la misma variante
            var itemExistente = carrito.FirstOrDefault(item =>
                item.ProductoVarianteId == variante.IdProductoVariante);

            if (itemExistente != null)
            {
                if (itemExistente.Cantidad + cantidad > variante.StockActual)
                {
                    return Json(new { success = false, message = $"No puedes agregar más. Ya tienes {itemExistente.Cantidad} en el carrito y solo hay {variante.StockActual} disponibles." });
                }
                itemExistente.Cantidad += cantidad;
            }
            else
            {
                carrito.Add(new CarritoItemViewModel
                {
                    ProductoId = producto.IdProducto,
                    ProductoVarianteId = variante.IdProductoVariante,
                    NombreProducto = producto.Nombre,
                    ImagenUrl = producto.ImagenUrl,
                    Categoria = producto.Categoria?.Nombre ?? "Sin categoría",
                    Color = variante.Color,
                    Talla = variante.Talla,
                    PrecioUnitario = producto.PrecioVenta,
                    Cantidad = cantidad,
                    StockDisponible = variante.StockActual
                });
            }

            _carritoService.GuardarCarrito(carrito);

            var totalItems = carrito.Sum(item => item.Cantidad);
            return Json(new { success = true, message = "Producto añadido al carrito.", totalItems });
        }

        [HttpPost]
        public IActionResult EditarItemCarrito(int productoId, int productoVarianteIdAnterior, string tallaNueva, string colorNuevo, int cantidad)
        {
            var carrito = _carritoService.ObtenerCarrito();

            // Buscar el item
            var itemAnterior = carrito.FirstOrDefault(item => item.ProductoVarianteId == productoVarianteIdAnterior);

            if (itemAnterior == null)
            {
                return Json(new { success = false, message = "Item no encontrado en el carrito." });
            }

            // Validar que se hayan seleccionado talla y color nuevos
            if (string.IsNullOrWhiteSpace(tallaNueva))
            {
                return Json(new { success = false, message = "Debes seleccionar una talla." });
            }

            if (string.IsNullOrWhiteSpace(colorNuevo))
            {
                return Json(new { success = false, message = "Debes seleccionar un color." });
            }

            if (cantidad < 1)
            {
                return Json(new { success = false, message = "La cantidad debe ser al menos 1." });
            }

            // Buscar la nueva variante
            var nuevaVariante = _context.ProductoVariantes
                .FirstOrDefault(v => v.ProductoId == itemAnterior.ProductoId &&
                                     v.Talla == tallaNueva &&
                                     v.Color == colorNuevo &&
                                     v.Estado);

            if (nuevaVariante == null)
            {
                return Json(new { success = false, message = "La combinación de talla y color seleccionada no está disponible." });
            }

            if (cantidad > nuevaVariante.StockActual)
            {
                return Json(new { success = false, message = $"Stock insuficiente. Solo hay {nuevaVariante.StockActual} unidades disponibles." });
            }

            // Si cambió la variante
            if (nuevaVariante.IdProductoVariante != productoVarianteIdAnterior)
            {
                var itemExistente = carrito.FirstOrDefault(item =>
                    item.ProductoVarianteId == nuevaVariante.IdProductoVariante);

                if (itemExistente != null)
                {
                    if (itemExistente.Cantidad + cantidad > nuevaVariante.StockActual)
                    {
                        return Json(new { success = false, message = $"No puedes agregar más. Ya tienes {itemExistente.Cantidad} de esa variante." });
                    }

                    itemExistente.Cantidad += cantidad;
                    carrito.Remove(itemAnterior);
                }
                else
                {
                    // Actualizar el item actual con la nueva variante
                    itemAnterior.ProductoVarianteId = nuevaVariante.IdProductoVariante;
                    itemAnterior.Talla = nuevaVariante.Talla;
                    itemAnterior.Color = nuevaVariante.Color;
                    itemAnterior.StockDisponible = nuevaVariante.StockActual;
                    itemAnterior.Cantidad = cantidad;
                }
            }
            else
            {
                // Solo cambió la cantidad
                itemAnterior.Cantidad = cantidad;
            }

            _carritoService.GuardarCarrito(carrito);

            var totalItems = carrito.Sum(item => item.Cantidad);
            return Json(new { success = true, message = "Carrito actualizado correctamente.", totalItems });
        }

        [HttpGet]
        public IActionResult ObtenerOpcionesProducto(int productoId)
        {
            var producto = _context.Productos
                .Include(p => p.Variantes)
                .FirstOrDefault(p => p.IdProducto == productoId);

            if (producto == null)
            {
                return Json(new { success = false, message = "Producto no encontrado." });
            }

            // Obtener variantes activas con stock
            var variantesActivas = producto.Variantes
                .Where(v => v.Estado && v.StockActual > 0)
                .ToList();

            var tallas = variantesActivas
                .Select(v => v.Talla)
                .Distinct()
                .OrderBy(t => t)
                .ToList();

            var colores = variantesActivas
                .Select(v => v.Color)
                .Distinct()
                .OrderBy(c => c)
                .ToList();

            // Devolver también las variantes para validar combinaciones válidas
            var variantes = variantesActivas
                .Select(v => new { v.IdProductoVariante, v.Talla, v.Color, v.StockActual })
                .ToList();

            return Json(new {
                success = true,
                tallas = tallas,
                colores = colores,
                variantes = variantes,
                stockTotal = variantesActivas.Sum(v => v.StockActual)
            });
        }

        [HttpGet]
        [ClienteOnly]
        public async Task<IActionResult> MisCompras()
        {
            var clienteId = HttpContext.Session.GetInt32("ClienteId");
            if (clienteId == null)
            {
                return RedirectToAction("Login", "Auth");
            }

            var compras = await _context.Ventas
                .Include(venta => venta.DetallesVenta)
                .Include(venta => venta.Envio)
                .Where(venta => venta.ClienteId == clienteId)
                .OrderByDescending(venta => venta.FechaVenta)
                .Select(venta => new MisComprasViewModel
                {
                    IdVenta = venta.IdVenta,
                    FechaVenta = venta.FechaVenta,
                    Total = venta.TotalVenta,
                    MetodoPago = venta.MetodoPago,
                    CantidadProductos = venta.DetallesVenta.Sum(detaCompra => detaCompra.Cantidad),
                    IdEnvio = venta.Envio != null ? venta.Envio.IdEnvio : null,
                    EstadoEnvio = venta.Envio != null ? venta.Envio.EstadoEnvio : "Sin envío",
                    NumeroSeguimiento = venta.Envio != null ? venta.Envio.NumeroSeguimiento : null
                })
                .ToListAsync();

            return View(compras);
        }

        [HttpGet]
        [ClienteOnly]
        public async Task<IActionResult> DetalleCompra(int id)
        {
            var clienteId = HttpContext.Session.GetInt32("ClienteId");
            if (clienteId == null)
            {
                return RedirectToAction("Login", "Auth");
            }

            var venta = await _context.Ventas
                .Include(venta => venta.DetallesVenta)
                    .ThenInclude(detaCompra => detaCompra.Producto)
                .Include(venta => venta.Envio)
                .Include(venta => venta.CuponDescuento)
                .FirstOrDefaultAsync(venta => venta.IdVenta == id && venta.ClienteId == clienteId);

            if (venta == null)
            {
                TempData["ErrorMessage"] = "Compra no encontrada";
                return RedirectToAction("MisCompras");
            }

            var viewModel = new DetalleCompraViewModel
            {
                IdVenta = venta.IdVenta,
                FechaVenta = venta.FechaVenta,
                SubtotalSinDescuento = venta.SubtotalSinDescuento,
                IGV = venta.IGV,
                Total = venta.TotalVenta,
                MetodoPago = venta.MetodoPago,
                CodigoCupon = venta.CuponDescuento?.CodigoCupon,
                PorcentajeDescuento = venta.CuponDescuento?.PorcentajeDescuento,
                Productos = venta.DetallesVenta.Select(detaCompra => new ProductoCompraViewModel
                {
                    NombreProducto = detaCompra.Producto?.Nombre ?? "Producto no disponible",
                    ImagenUrl = detaCompra.Producto?.ImagenUrl ?? "/img/default.png",
                    Color = detaCompra.Color ?? "",
                    Talla = detaCompra.Talla ?? "",
                    Cantidad = detaCompra.Cantidad,
                    PrecioUnitario = detaCompra.PrecioUnitario,
                    Subtotal = detaCompra.SubTotal
                }).ToList()
            };

            if (venta.Envio != null)
            {
                viewModel.Envio = new EnvioDetalleViewModel
                {
                    IdEnvio = venta.Envio.IdEnvio,
                    DireccionEnvio = venta.Envio.DireccionEnvio,
                    Ciudad = venta.Envio.Ciudad,
                    CodigoPostal = venta.Envio.CodigoPostal,
                    FechaEnvio = venta.Envio.FechaEnvio,
                    FechaEntregaEstimada = venta.Envio.FechaEntregaEstimada,
                    FechaEntregaReal = venta.Envio.FechaEntregaReal,
                    EstadoEnvio = venta.Envio.EstadoEnvio,
                    EmpresaTransporte = venta.Envio.EmpresaTransporte,
                    NumeroSeguimiento = venta.Envio.NumeroSeguimiento
                };
            }

            return View(viewModel);
        }

        [HttpGet]
        [ClienteOnly]
        public async Task<IActionResult> SeguimientoEnvio(int id)
        {
            var clienteId = HttpContext.Session.GetInt32("ClienteId");
            if (clienteId == null)
            {
                return RedirectToAction("Login", "Auth");
            }

            var envio = await _context.Envios
                .Include(e => e.Venta)
                .FirstOrDefaultAsync(e => e.IdEnvio == id && e.Venta.ClienteId == clienteId);

            if (envio == null)
            {
                TempData["ErrorMessage"] = "Envío no encontrado";
                return RedirectToAction("MisCompras");
            }

            var viewModel = new EnvioDetalleViewModel
            {
                IdEnvio = envio.IdEnvio,
                DireccionEnvio = envio.DireccionEnvio,
                Ciudad = envio.Ciudad,
                CodigoPostal = envio.CodigoPostal,
                FechaEnvio = envio.FechaEnvio,
                FechaEntregaEstimada = envio.FechaEntregaEstimada,
                FechaEntregaReal = envio.FechaEntregaReal,
                EstadoEnvio = envio.EstadoEnvio,
                EmpresaTransporte = envio.EmpresaTransporte,
                NumeroSeguimiento = envio.NumeroSeguimiento
            };

            return View(viewModel);
        }
    }
}
