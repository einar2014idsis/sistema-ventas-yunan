using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Proyecto_DAS.Data;
using Proyecto_DAS.Filters;
using Proyecto_DAS.ViewModels.Envio;

namespace Proyecto_DAS.Controllers
{
    [AdminOnly]
    public class EnvioController : Controller
    {
        private readonly AppDbContext _context;

        public EnvioController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Index(EnvioFilterViewModel filtros)
        {
            var consulta = _context.Envios
                .Include(e => e.Venta)
                    .ThenInclude(v => v.Cliente)
                        .ThenInclude(c => c.Usuario)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(filtros.Estado))
                consulta = consulta.Where(estado => estado.EstadoEnvio == filtros.Estado);

            if (!string.IsNullOrWhiteSpace(filtros.Ciudad))
                consulta = consulta.Where(estado => estado.Ciudad.Contains(filtros.Ciudad));

            if (!string.IsNullOrWhiteSpace(filtros.Empresa))
                consulta = consulta.Where(estado => estado.EmpresaTransporte.Contains(filtros.Empresa));

            if (!string.IsNullOrWhiteSpace(filtros.Tracking))
                consulta = consulta.Where(estado => estado.NumeroSeguimiento.Contains(filtros.Tracking));

            if (filtros.VentaId.HasValue)
                consulta = consulta.Where(venta => venta.VentaId == filtros.VentaId.Value);

            if (filtros.Desde.HasValue)
                consulta = consulta.Where(venta =>
                    venta.FechaEnvio >= filtros.Desde.Value ||
                    venta.FechaEntregaEstimada >= filtros.Desde.Value ||
                    venta.FechaEntregaReal >= filtros.Desde.Value);

            if (filtros.Hasta.HasValue)
                consulta = consulta.Where(venta =>
                    venta.FechaEnvio <= filtros.Hasta.Value ||
                    venta.FechaEntregaEstimada <= filtros.Hasta.Value ||
                    venta.FechaEntregaReal <= filtros.Hasta.Value);

            var lista = await consulta
                .OrderByDescending(envio => envio.FechaEnvio)
                .Select(envio => new EnvioViewModel
                {
                    IdEnvio = envio.IdEnvio,
                    VentaId = envio.VentaId,
                    ClienteNombre = envio.Venta.Cliente.Usuario.Nombres + " " + envio.Venta.Cliente.Usuario.Apellidos,
                    Ciudad = envio.Ciudad,
                    DireccionEnvio = envio.DireccionEnvio,
                    CodigoPostal = envio.CodigoPostal,
                    FechaEnvio = envio.FechaEnvio,
                    FechaEntregaEstimada = envio.FechaEntregaEstimada,
                    EstadoEnvio = envio.EstadoEnvio,
                    EmpresaTransporte = envio.EmpresaTransporte,
                    NumeroSeguimiento = envio.NumeroSeguimiento
                })
                .ToListAsync();

            return View(lista);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ActualizarEstado(int idEnvio, string nuevoEstado)
        {
            var envio = await _context.Envios
                .Include(e => e.Venta)
                .FirstOrDefaultAsync(e => e.IdEnvio == idEnvio);

            if (envio == null)
            {
                TempData["ErrorMessage"] = "El envío no existe.";
                return RedirectToAction(nameof(Index));
            }

            string estadoAnterior = envio.EstadoEnvio;

            if (estadoAnterior == nuevoEstado)
            {
                TempData["ErrorMessage"] = "El envío ya se encuentra en ese estado.";
                return RedirectToAction(nameof(Index));
            }

            if (!ValidarCambioEstado(estadoAnterior, nuevoEstado))
            {
                TempData["ErrorMessage"] = $"No se puede cambiar de '{estadoAnterior}' a '{nuevoEstado}'. Debe seguir el flujo: pendiente → en_proceso → alistado → enviado → entregado";
                return RedirectToAction(nameof(Index));
            }

            envio.EstadoEnvio = nuevoEstado;

            if (nuevoEstado == "entregado")
            {
                envio.FechaEntregaReal = DateTime.Now;
            }

            _context.Update(envio);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = $"Estado del envío actualizado correctamente.";
            return RedirectToAction(nameof(Index));
        }

        // Validar flujo de estados
        private bool ValidarCambioEstado(string estadoActual, string estadoNuevo)
        {
            if (estadoActual == estadoNuevo)
                return true;

            if (estadoNuevo == "cancelado" && estadoActual != "entregado")
                return true;

            if (estadoActual == "entregado" || estadoActual == "cancelado")
                return false;

            var flujoValido = new Dictionary<string, string[]>
            {
                { "pendiente", new[] { "en_proceso" } },
                { "en_proceso", new[] { "alistado" } },
                { "alistado", new[] { "enviado" } },
                { "enviado", new[] { "entregado" } }
            };

            if (flujoValido.ContainsKey(estadoActual))
            {
                return flujoValido[estadoActual].Contains(estadoNuevo);
            }

            return false;
        }
    }
}
