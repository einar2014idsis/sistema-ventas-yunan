using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Proyecto_DAS.Data;
using Proyecto_DAS.Filters;
using Proyecto_DAS.ViewModels.AlertaStock;

namespace Proyecto_DAS.Controllers
{
    [AdminOnly]
    public class AlertaStockController : Controller
    {
        private readonly AppDbContext _context;

        public AlertaStockController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Index(AlertaStockFilterViewModel filtros)
        {
            var consulta = _context.AlertaStock
                .Include(a => a.ProductoVariante)
                    .ThenInclude(v => v.Producto)
                .AsQueryable();

            // Filtro por estado (Pendiente/Resuelta)
            if (!string.IsNullOrEmpty(filtros.Estado))
            {
                consulta = consulta.Where(a => a.Estado == filtros.Estado);
            }

            // Filtro por producto
            if (filtros.ProductoId.HasValue)
            {
                consulta = consulta.Where(a => a.ProductoVariante.ProductoId == filtros.ProductoId.Value);
            }

            // Filtro por fecha desde
            if (filtros.FechaDesde.HasValue)
            {
                consulta = consulta.Where(a => a.FechaAlerta >= filtros.FechaDesde.Value);
            }

            // Filtro por fecha hasta
            if (filtros.FechaHasta.HasValue)
            {
                consulta = consulta.Where(a => a.FechaAlerta <= filtros.FechaHasta.Value);
            }

            var alertas = await consulta
                .OrderByDescending(a => a.FechaAlerta)
                .Select(a => new AlertaStockViewModel
                {
                    IdAlerta = a.IdAlertaStock,
                    ProductoVarianteId = a.ProductoVarianteId,
                    ProductoNombre = a.ProductoVariante.Producto.Nombre,
                    Talla = a.ProductoVariante.Talla,
                    Color = a.ProductoVariante.Color,
                    StockActual = a.ProductoVariante.StockActual,
                    StockMinimo = a.ProductoVariante.StockMinimo,
                    FechaAlerta = a.FechaAlerta,
                    FechaResolucion = a.FechaResolucion,
                    Estado = a.Estado,
                    Descripcion = a.Descripcion
                })
                .ToListAsync();

            // Cargar lista de productos para el filtro
            ViewBag.Productos = await _context.Productos
                .Where(p => p.Estado)
                .OrderBy(p => p.Nombre)
                .Select(p => new { p.IdProducto, p.Nombre })
                .ToListAsync();

            ViewBag.Filtros = filtros;
            return View(alertas);
        }
    }
}
