using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Proyecto_DAS.Data;
using Proyecto_DAS.Filters;
using Proyecto_DAS.Models;
using Proyecto_DAS.ViewModels.Proveedor;

namespace Proyecto_DAS.Controllers
{
    [AdminOnly]
    public class ProveedorController : Controller
    {
        private readonly AppDbContext _context;

        public ProveedorController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var proveedores = await _context.Proveedores
                .Select(p => new ProveedorViewModel
                {
                    IdProveedor = p.IdProveedor,
                    NombreEmpresa = p.NombreEmpresa,
                    ContactoNombre = p.ContactoNombre,
                    Celular = p.Celular,
                    Email = p.Email,
                    Direccion = p.Direccion,
                    Ruc = p.Ruc,
                    Estado = p.Estado
                })
                .ToListAsync();

            return View(proveedores);
        }

        [HttpGet]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var proveedor = await _context.Proveedores
                .FirstOrDefaultAsync(m => m.IdProveedor == id);

            if (proveedor == null)
            {
                return NotFound();
            }

            var viewModel = new ProveedorViewModel
            {
                IdProveedor = proveedor.IdProveedor,
                NombreEmpresa = proveedor.NombreEmpresa,
                ContactoNombre = proveedor.ContactoNombre,
                Celular = proveedor.Celular,
                Email = proveedor.Email,
                Direccion = proveedor.Direccion,
                Ruc = proveedor.Ruc,
                Estado = proveedor.Estado
            };

            return View(viewModel);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProveedorCreateViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                // Validar nombre de empresa único (ignorando mayúsculas/minúsculas)
                var nombreNormalizado = viewModel.NombreEmpresa.Trim().ToUpper();
                var nombreExiste = await _context.Proveedores
                    .AnyAsync(p => p.NombreEmpresa.ToUpper() == nombreNormalizado);

                if (nombreExiste)
                {
                    ModelState.AddModelError("NombreEmpresa", "Ya existe un proveedor con este nombre");
                    return View(viewModel);
                }

                var rucExiste = await _context.Proveedores
                    .AnyAsync(p => p.Ruc == viewModel.Ruc);

                if (rucExiste)
                {
                    ModelState.AddModelError("Ruc", "Ya existe un proveedor con este RUC");
                    return View(viewModel);
                }

                var emailExiste = await _context.Proveedores
                    .AnyAsync(p => p.Email == viewModel.Email);

                if (emailExiste)
                {
                    ModelState.AddModelError("Email", "Ya existe un proveedor con este email");
                    return View(viewModel);
                }

                var celularExiste = await _context.Proveedores
                    .AnyAsync(p => p.Celular == viewModel.Celular);

                if (celularExiste)
                {
                    ModelState.AddModelError("Celular", "Ya existe un proveedor con este número de celular");
                    return View(viewModel);
                }

                if (!Helpers.ValidationHelper.EsTelefonoValido(viewModel.Celular))
                {
                    ModelState.AddModelError("Celular", "El celular debe empezar con 9 y tener exactamente 9 dígitos");
                    return View(viewModel);
                }

                var proveedor = new Proveedor
                {
                    NombreEmpresa = viewModel.NombreEmpresa,
                    ContactoNombre = viewModel.ContactoNombre,
                    Celular = viewModel.Celular,
                    Email = viewModel.Email,
                    Direccion = viewModel.Direccion,
                    Ruc = viewModel.Ruc,
                    Estado = viewModel.Estado
                };

                _context.Add(proveedor);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Proveedor creado exitosamente.";
                return RedirectToAction(nameof(Index));
            }

            return View(viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var proveedor = await _context.Proveedores.FindAsync(id);
            if (proveedor == null)
            {
                return NotFound();
            }

            var viewModel = new ProveedorEditViewModel
            {
                IdProveedor = proveedor.IdProveedor,
                NombreEmpresa = proveedor.NombreEmpresa,
                ContactoNombre = proveedor.ContactoNombre,
                Celular = proveedor.Celular,
                Email = proveedor.Email,
                Direccion = proveedor.Direccion,
                Ruc = proveedor.Ruc,
                Estado = proveedor.Estado
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ProveedorEditViewModel viewModel)
        {
            if (id != viewModel.IdProveedor)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var proveedor = await _context.Proveedores.FindAsync(id);
                if (proveedor == null)
                {
                    return NotFound();
                }

                // Validar nombre de empresa único excluyendo el proveedor actual
                var nombreNormalizado = viewModel.NombreEmpresa.Trim().ToUpper();
                var nombreExiste = await _context.Proveedores
                    .AnyAsync(p => p.NombreEmpresa.ToUpper() == nombreNormalizado && p.IdProveedor != id);

                if (nombreExiste)
                {
                    ModelState.AddModelError("NombreEmpresa", "Ya existe otro proveedor con este nombre");
                    return View(viewModel);
                }

                var rucExiste = await _context.Proveedores
                    .AnyAsync(p => p.Ruc == viewModel.Ruc && p.IdProveedor != id);

                if (rucExiste)
                {
                    ModelState.AddModelError("Ruc", "Ya existe otro proveedor con este RUC");
                    return View(viewModel);
                }

                var emailExiste = await _context.Proveedores
                    .AnyAsync(p => p.Email == viewModel.Email && p.IdProveedor != id);

                if (emailExiste)
                {
                    ModelState.AddModelError("Email", "Ya existe otro proveedor con este email");
                    return View(viewModel);
                }

                var celularExiste = await _context.Proveedores
                    .AnyAsync(p => p.Celular == viewModel.Celular && p.IdProveedor != id);

                if (celularExiste)
                {
                    ModelState.AddModelError("Celular", "Ya existe otro proveedor con este número de celular");
                    return View(viewModel);
                }

                if (!Helpers.ValidationHelper.EsTelefonoValido(viewModel.Celular))
                {
                    ModelState.AddModelError("Celular", "El celular debe empezar con 9 y tener exactamente 9 dígitos");
                    return View(viewModel);
                }

                proveedor.NombreEmpresa = viewModel.NombreEmpresa;
                proveedor.ContactoNombre = viewModel.ContactoNombre;
                proveedor.Celular = viewModel.Celular;
                proveedor.Email = viewModel.Email;
                proveedor.Direccion = viewModel.Direccion;
                proveedor.Ruc = viewModel.Ruc;
                proveedor.Estado = viewModel.Estado;

                _context.Update(proveedor);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Proveedor actualizado exitosamente.";
                return RedirectToAction(nameof(Index));
            }
            return View(viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var proveedor = await _context.Proveedores
                .Include(p => p.Productos)
                .FirstOrDefaultAsync(m => m.IdProveedor == id);

            if (proveedor == null)
            {
                return NotFound();
            }

            if (proveedor.Productos != null && proveedor.Productos.Any())
            {
                TempData["ErrorMessage"] = "No se puede eliminar el proveedor porque tiene productos asociados.";
                return RedirectToAction(nameof(Index));
            }

            _context.Proveedores.Remove(proveedor);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Proveedor eliminado exitosamente.";
            return RedirectToAction(nameof(Index));
        }
    }
}
