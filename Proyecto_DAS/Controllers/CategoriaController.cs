using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Proyecto_DAS.Data;
using Proyecto_DAS.Filters;
using Proyecto_DAS.Models;
using Proyecto_DAS.ViewModels.Categoria;

namespace Proyecto_DAS.Controllers
{
    [AdminOnly]
    public class CategoriaController : Controller
    {
        private readonly AppDbContext _context;

        public CategoriaController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var categorias = await _context.Categorias
                .Include(c => c.Productos)
                .Select(c => new CategoriaViewModel
                {
                    IdCategoria = c.IdCategoria,
                    Nombre = c.Nombre,
                    Descripcion = c.Descripcion,
                    Estado = c.Estado,
                    CantidadProductos = c.Productos != null ? c.Productos.Count : 0
                })
                .ToListAsync();

            return View(categorias);
        }

        [HttpGet]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var categoria = await _context.Categorias
                .Include(c => c.Productos)
                .FirstOrDefaultAsync(m => m.IdCategoria == id);

            if (categoria == null)
            {
                return NotFound();
            }

            var viewModel = new CategoriaViewModel
            {
                IdCategoria = categoria.IdCategoria,
                Nombre = categoria.Nombre,
                Descripcion = categoria.Descripcion,
                Estado = categoria.Estado,
                CantidadProductos = categoria.Productos != null ? categoria.Productos.Count : 0
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
        public async Task<IActionResult> Create(CategoriaCreateViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                // Validar nombre único
                var nombreNormalizado = viewModel.Nombre.Trim().ToUpper();
                var nombreExiste = await _context.Categorias
                    .AnyAsync(c => c.Nombre.ToUpper() == nombreNormalizado);

                if (nombreExiste)
                {
                    ModelState.AddModelError("Nombre", "Ya existe una categoría con este nombre");
                    return View(viewModel);
                }

                var categoria = new Categoria
                {
                    Nombre = viewModel.Nombre.Trim(),
                    Descripcion = viewModel.Descripcion,
                    Estado = viewModel.Estado
                };

                _context.Add(categoria);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Categoría creada exitosamente.";
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

            var categoria = await _context.Categorias.FindAsync(id);
            if (categoria == null)
            {
                return NotFound();
            }

            var viewModel = new CategoriaEditViewModel
            {
                IdCategoria = categoria.IdCategoria,
                Nombre = categoria.Nombre,
                Descripcion = categoria.Descripcion,
                Estado = categoria.Estado
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, CategoriaEditViewModel viewModel)
        {
            if (id != viewModel.IdCategoria)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var categoria = await _context.Categorias.FindAsync(id);
                if (categoria == null)
                {
                    return NotFound();
                }

                // Validar nombre único
                var nombreNormalizado = viewModel.Nombre.Trim().ToUpper();
                var nombreExiste = await _context.Categorias
                    .AnyAsync(c => c.Nombre.ToUpper() == nombreNormalizado && c.IdCategoria != id);

                if (nombreExiste)
                {
                    ModelState.AddModelError("Nombre", "Ya existe otra categoría con este nombre");
                    return View(viewModel);
                }

                categoria.Nombre = viewModel.Nombre.Trim();
                categoria.Descripcion = viewModel.Descripcion;
                categoria.Estado = viewModel.Estado;

                _context.Update(categoria);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Categoría actualizada exitosamente.";
                return RedirectToAction(nameof(Index));
            }
            return View(viewModel);
        }

        
        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var categoria = await _context.Categorias
                .Include(c => c.Productos)
                .FirstOrDefaultAsync(m => m.IdCategoria == id);

            if (categoria == null)
            {
                return NotFound();
            }

            if (categoria.Productos != null && categoria.Productos.Any())
            {
                TempData["ErrorMessage"] = "No se puede eliminar la categoría porque tiene productos asociados.";
                return RedirectToAction(nameof(Index));
            }

            _context.Categorias.Remove(categoria);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Categoría eliminada exitosamente.";
            return RedirectToAction(nameof(Index));
        }
    }
}
