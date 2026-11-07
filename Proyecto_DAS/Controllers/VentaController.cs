using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Proyecto_DAS.Data;
using Proyecto_DAS.Filters;
using Proyecto_DAS.Models;
using Proyecto_DAS.ViewModels.Venta;
using Proyecto_DAS.Services;

namespace Proyecto_DAS.Controllers
{
    [AdminOnly]
    public class VentaController : Controller
    {
        private readonly AppDbContext appDbContext;
        private readonly IWebHostEnvironment _hostEnvironment;
        private readonly ReporteVentasService _reporteService;
        private readonly BoletaService _boletaService;
        private readonly DashboardVentasService _dashboardService;

        public VentaController(AppDbContext appDbContext, IWebHostEnvironment hostEnvironment, ReporteVentasService reporteService, BoletaService boletaService, DashboardVentasService dashboardService)
        {
            this.appDbContext = appDbContext;
            _hostEnvironment = hostEnvironment;
            _reporteService = reporteService;
            _boletaService = boletaService;
            _dashboardService = dashboardService;
        }

        [HttpGet]
        public async Task<IActionResult> DatosDashboard(int? clienteId, string? metodoPago)
        {
            return Json(await _dashboardService.ObtenerAsync(clienteId, metodoPago));
        }

        [HttpGet]
        public async Task<IActionResult> Index(DateTime? fechaInicio, DateTime? fechaFin, int? clienteId, string metodoPago)
        {
            var consulta = appDbContext.Ventas
                .Include(v => v.Cliente)
                    .ThenInclude(c => c.Usuario)
                .Include(v => v.CuponDescuento)
                .Include(v => v.DetallesVenta)
                    .ThenInclude(d => d.Producto)
                        .ThenInclude(p => p.Categoria)
                .Include(v => v.Envio)
                .AsQueryable();

            // Aplicar filtros
            if (fechaInicio.HasValue)
            {
                consulta = consulta.Where(v => v.FechaVenta >= fechaInicio.Value);
            }

            if (fechaFin.HasValue)
            {
                var fechaFinConHora = fechaFin.Value.Date.AddDays(1);
                consulta = consulta.Where(v => v.FechaVenta < fechaFinConHora);
            }

            if (clienteId.HasValue && clienteId.Value > 0)
            {
                consulta = consulta.Where(v => v.ClienteId == clienteId.Value);
            }

            if (!string.IsNullOrWhiteSpace(metodoPago))
            {
                consulta = consulta.Where(v => v.MetodoPago == metodoPago);
            }

            List<Venta> ventasList = await consulta
                .OrderByDescending(v => v.FechaVenta)
                .ToListAsync();

            var ventasViewModel = ventasList.Select(v => new VentaViewModel
            {
                IdVenta = v.IdVenta,
                ClienteNombre = $"{v.Cliente.Usuario.Nombres} {v.Cliente.Usuario.Apellidos}",
                ClienteId = v.ClienteId,
                FechaVenta = v.FechaVenta,
                SubtotalSinDescuento = v.SubtotalSinDescuento,
                TotalVenta = v.TotalVenta,
                MetodoPago = v.MetodoPago,
                CodigoCupon = v.CuponDescuento?.CodigoCupon,
                PorcentajeDescuento = v.CuponDescuento?.PorcentajeDescuento ?? 0,
                EstadoEnvio = v.Envio?.EstadoEnvio ?? "Sin envío",
                NumeroSeguimiento = v.Envio?.NumeroSeguimiento ?? "-",
                CantidadProductos = v.DetallesVenta?.Sum(d => d.Cantidad) ?? 0
            }).ToList();

            ViewBag.Clientes = await appDbContext.Clientes
                .Include(c => c.Usuario)
                .Where(c => c.Usuario.Estado == true)
                .OrderBy(c => c.Usuario.Nombres)
                .ToListAsync();

            ViewBag.FechaInicio = fechaInicio?.ToString("yyyy-MM-dd");
            ViewBag.FechaFin = fechaFin?.ToString("yyyy-MM-dd");
            ViewBag.ClienteId = clienteId;
            ViewBag.MetodoPago = metodoPago;

            ViewBag.VentasCompletas = ventasList;

            return View(ventasViewModel);
        }

        [HttpGet]
        public async Task<IActionResult> DescargarBoleta(int id)
        {
            var venta = await appDbContext.Ventas
                .Include(v => v.Cliente)
                    .ThenInclude(c => c.Usuario)
                .Include(v => v.DetallesVenta)
                    .ThenInclude(d => d.Producto)
                .Include(v => v.CuponDescuento)
                .Include(v => v.Envio)
                .FirstOrDefaultAsync(v => v.IdVenta == id);

            if (venta == null)
            {
                TempData["ErrorMessage"] = "Venta no encontrada";
                return RedirectToAction("Index");
            }

            try
            {
                byte[] pdfBytes = _boletaService.GenerarBoletaPDF(venta, venta.Cliente, venta.Cliente.Usuario);

                string fileName = $"Boleta_Venta_{venta.IdVenta}_{DateTime.Now:yyyyMMdd}.pdf";
                return File(pdfBytes, "application/pdf", fileName);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error al generar la boleta: {ex.Message}";
                return RedirectToAction("Index");
            }
        }

        [HttpGet]
        public async Task<IActionResult> GenerarReporteExcel(DateTime? fechaInicio, DateTime? fechaFin, int? clienteId, string metodoPago)
        {
            try
            {
                var consulta = appDbContext.Ventas
                    .Include(v => v.Cliente)
                        .ThenInclude(c => c.Usuario)
                    .Include(v => v.CuponDescuento)
                    .Include(v => v.DetallesVenta)
                        .ThenInclude(d => d.Producto)
                    .Include(v => v.Envio)
                    .AsQueryable();

                if (fechaInicio.HasValue)
                {
                    consulta = consulta.Where(v => v.FechaVenta >= fechaInicio.Value);
                }

                if (fechaFin.HasValue)
                {
                    var fechaFinConHora = fechaFin.Value.Date.AddDays(1);
                    consulta = consulta.Where(v => v.FechaVenta < fechaFinConHora);
                }

                if (clienteId.HasValue && clienteId.Value > 0)
                {
                    consulta = consulta.Where(v => v.ClienteId == clienteId.Value);
                }

                if (!string.IsNullOrWhiteSpace(metodoPago))
                {
                    consulta = consulta.Where(v => v.MetodoPago == metodoPago);
                }

                List<Venta> ventasList = await consulta
                    .OrderByDescending(v => v.FechaVenta)
                    .ToListAsync();

                // Validar que haya ventas
                if (!ventasList.Any())
                {
                    TempData["ErrorMessage"] = "No hay ventas para generar el reporte con los filtros seleccionados.";
                    return RedirectToAction("Index", new { fechaInicio, fechaFin, clienteId, metodoPago });
                }

                // Obtener nombre del cliente si está filtrado
                string? clienteNombre = null;
                if (clienteId.HasValue && clienteId.Value > 0)
                {
                    var cliente = await appDbContext.Clientes
                        .Include(c => c.Usuario)
                        .FirstOrDefaultAsync(c => c.IdCliente == clienteId.Value);
                    if (cliente != null)
                    {
                        clienteNombre = $"{cliente.Usuario.Nombres} {cliente.Usuario.Apellidos}";
                    }
                }

                byte[] excelBytes = _reporteService.GenerarReporteExcel(ventasList, fechaInicio, fechaFin, metodoPago, clienteNombre);

                string fileName = $"Reporte_Ventas_YUNAN_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";

                return File(excelBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error al generar el reporte: {ex.Message}";
                return RedirectToAction("Index");
            }
        }

    }
}
