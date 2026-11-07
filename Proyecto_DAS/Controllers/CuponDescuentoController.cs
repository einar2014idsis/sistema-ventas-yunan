using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Proyecto_DAS.Data;
using Proyecto_DAS.Filters;
using Proyecto_DAS.Models;
using Proyecto_DAS.ViewModels.CuponDescuento;

namespace Proyecto_DAS.Controllers
{
    [AdminOnly]
    public class CuponDescuentoController : Controller
    {
        private readonly AppDbContext _context;

        public CuponDescuentoController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var cupones = await _context.CuponDescuento
                .OrderByDescending(cupon => cupon.FechaCreacion)
                .Select(cupon => new CuponDescuentoViewModel
                {
                    IdCuponDescuento = cupon.IdCuponDescuento,
                    CodigoCupon = cupon.CodigoCupon,
                    PorcentajeDescuento = cupon.PorcentajeDescuento,
                    FechaInicio = cupon.FechaInicio,
                    FechaVencimiento = cupon.FechaVencimiento,
                    Estado = cupon.Estado,
                    FechaCreacion = cupon.FechaCreacion
                })
                .ToListAsync();

            return View(cupones);
        }

        [HttpGet]
        public IActionResult Create()
        {
            var viewModel = new CuponDescuentoCreateViewModel
            {
                FechaInicio = DateTime.Now,
                FechaVencimiento = DateTime.Now.AddDays(30),
                Estado = true
            };
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CuponDescuentoCreateViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                // Validar que FechaInicio no sea una fecha pasada
                if (viewModel.FechaInicio.Date < DateTime.Now.Date)
                {
                    ModelState.AddModelError("FechaInicio", "La fecha de inicio no puede ser anterior a hoy");
                    return View(viewModel);
                }

                // Validar que FechaVencimiento sea mayor a FechaInicio
                if (viewModel.FechaVencimiento <= viewModel.FechaInicio)
                {
                    ModelState.AddModelError("FechaVencimiento", "La fecha de vencimiento debe ser posterior a la fecha de inicio");
                    return View(viewModel);
                }

                // Validar que el código del cupón no exista
                var codigoExiste = await _context.CuponDescuento
                    .AnyAsync(cupon => cupon.CodigoCupon.ToUpper() == viewModel.CodigoCupon.ToUpper());

                if (codigoExiste)
                {
                    ModelState.AddModelError("CodigoCupon", "El código de cupón ya existe");
                    return View(viewModel);
                }

                var cupon = new CuponDescuento
                {
                    CodigoCupon = viewModel.CodigoCupon.ToUpper(),
                    PorcentajeDescuento = viewModel.PorcentajeDescuento,
                    FechaInicio = viewModel.FechaInicio,
                    FechaVencimiento = viewModel.FechaVencimiento,
                    Estado = viewModel.Estado,
                    FechaCreacion = DateTime.Now
                };

                _context.CuponDescuento.Add(cupon);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Cupón de descuento creado exitosamente";
                return RedirectToAction(nameof(Index));
            }

            return View(viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var cupon = await _context.CuponDescuento.FindAsync(id);

            if (cupon == null)
            {
                TempData["ErrorMessage"] = "Cupón no encontrado";
                return RedirectToAction(nameof(Index));
            }

            var viewModel = new CuponDescuentoEditViewModel
            {
                IdCuponDescuento = cupon.IdCuponDescuento,
                CodigoCupon = cupon.CodigoCupon,
                PorcentajeDescuento = cupon.PorcentajeDescuento,
                FechaInicio = cupon.FechaInicio,
                FechaVencimiento = cupon.FechaVencimiento,
                Estado = cupon.Estado,
                FechaCreacion = cupon.FechaCreacion
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, CuponDescuentoEditViewModel viewModel)
        {
            if (id != viewModel.IdCuponDescuento)
            {
                TempData["ErrorMessage"] = "Cupón no encontrado";
                return RedirectToAction(nameof(Index));
            }

            if (ModelState.IsValid)
            {
                // Obtener el cupón actual para comparar fechas
                var cupon = await _context.CuponDescuento.FindAsync(id);

                if (cupon == null)
                {
                    TempData["ErrorMessage"] = "Cupón no encontrado";
                    return RedirectToAction(nameof(Index));
                }

                if (viewModel.FechaInicio.Date != cupon.FechaInicio.Date && viewModel.FechaInicio.Date < DateTime.Now.Date)
                {
                    ModelState.AddModelError("FechaInicio", "La nueva fecha de inicio no puede ser anterior a hoy");
                    return View(viewModel);
                }

                if (viewModel.FechaVencimiento <= viewModel.FechaInicio)
                {
                    ModelState.AddModelError("FechaVencimiento", "La fecha de vencimiento debe ser posterior a la fecha de inicio");
                    return View(viewModel);
                }

                var codigoExiste = await _context.CuponDescuento
                    .AnyAsync(cupon => cupon.CodigoCupon.ToUpper() == viewModel.CodigoCupon.ToUpper()
                                && cupon.IdCuponDescuento != viewModel.IdCuponDescuento);

                if (codigoExiste)
                {
                    ModelState.AddModelError("CodigoCupon", "El código de cupón ya existe");
                    return View(viewModel);
                }

                cupon.CodigoCupon = viewModel.CodigoCupon.ToUpper();
                cupon.PorcentajeDescuento = viewModel.PorcentajeDescuento;
                cupon.FechaInicio = viewModel.FechaInicio;
                cupon.FechaVencimiento = viewModel.FechaVencimiento;
                cupon.Estado = viewModel.Estado;

                _context.Update(cupon);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Cupón actualizado exitosamente";
                return RedirectToAction(nameof(Index));
            }

            return View(viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var cupon = await _context.CuponDescuento
                .Include(cupon => cupon.Ventas)
                .FirstOrDefaultAsync(cupon => cupon.IdCuponDescuento == id);

            if (cupon == null)
            {
                TempData["ErrorMessage"] = "Cupón no encontrado";
                return RedirectToAction(nameof(Index));
            }

            var viewModel = new CuponDescuentoViewModel
            {
                IdCuponDescuento = cupon.IdCuponDescuento,
                CodigoCupon = cupon.CodigoCupon,
                PorcentajeDescuento = cupon.PorcentajeDescuento,
                FechaInicio = cupon.FechaInicio,
                FechaVencimiento = cupon.FechaVencimiento,
                Estado = cupon.Estado,
                FechaCreacion = cupon.FechaCreacion
            };

            ViewBag.CantidadVentas = cupon.Ventas?.Count ?? 0;

            return View(viewModel);
        }


        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var cupon = await _context.CuponDescuento
                .Include(cupon => cupon.Ventas)
                .FirstOrDefaultAsync(cupon => cupon.IdCuponDescuento == id);

            if (cupon == null)
            {
                TempData["ErrorMessage"] = "Cupón no encontrado";
                return RedirectToAction(nameof(Index));
            }

            // Verificar si el cupón tiene ventas asociadas
            if (cupon.Ventas != null && cupon.Ventas.Any())
            {
                TempData["ErrorMessage"] = $"No se puede eliminar el cupón porque tiene {cupon.Ventas.Count} venta(s) asociada(s)";
                return RedirectToAction(nameof(Index));
            }

            _context.CuponDescuento.Remove(cupon);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Cupón eliminado exitosamente";
            return RedirectToAction(nameof(Index));
        }
    }
}
