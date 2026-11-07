using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Proyecto_DAS.Data;
using Proyecto_DAS.Filters;
using Proyecto_DAS.Models;
using Proyecto_DAS.ViewModels.Producto;

namespace Proyecto_DAS.Controllers
{
    [AdminOnly]
    public class ProductoController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _hostEnvironment;

        public ProductoController(AppDbContext context, IWebHostEnvironment hostEnvironment)
        {
            _context = context;
            _hostEnvironment = hostEnvironment;
        }

        [HttpGet]
        public async Task<IActionResult> Index(ProductoFilterViewModel filtros)
        {
            var consulta = _context.Productos
                .Include(pro => pro.Categoria)
                .Include(pro => pro.Proveedor)
                .Include(pro => pro.Variantes)
                .AsQueryable();

            // Filtros
            if (filtros.CategoriaId.HasValue)
            {
                consulta = consulta.Where(pro => pro.CategoriaId == filtros.CategoriaId.Value);
            }

            if (filtros.Estado.HasValue)
            {
                consulta = consulta.Where(pro => pro.Estado == filtros.Estado.Value);
            }

            if (filtros.StockBajo.HasValue)
            {
                if (filtros.StockBajo.Value)
                {
                    consulta = consulta.Where(pro => pro.Variantes.Any(v => v.StockActual <= v.StockMinimo && v.Estado));
                }
                else
                {
                    consulta = consulta.Where(pro => !pro.Variantes.Any(v => v.StockActual <= v.StockMinimo && v.Estado));
                }
            }

            if (!string.IsNullOrWhiteSpace(filtros.NombreProducto))
            {
                consulta = consulta.Where(pro => pro.Nombre.Contains(filtros.NombreProducto));
            }

            var productos = await consulta
                .Select(pro => new ProductoViewModel
                {
                    IdProducto = pro.IdProducto,
                    Nombre = pro.Nombre,
                    Descripcion = pro.Descripcion,
                    PrecioCompra = pro.PrecioCompra,
                    PrecioVenta = pro.PrecioVenta,
                    StockActual = pro.Variantes.Where(v => v.Estado).Sum(v => v.StockActual),
                    ImagenUrl = pro.ImagenUrl,
                    Talla = string.Join(", ", pro.Variantes.Where(v => v.Estado).Select(v => v.Talla).Distinct()),
                    Color = string.Join(", ", pro.Variantes.Where(v => v.Estado).Select(v => v.Color).Distinct()),
                    Estado = pro.Estado,
                    FechaRegistro = pro.FechaRegistro,
                    CategoriaNombre = pro.Categoria != null ? pro.Categoria.Nombre : "Sin categoria",
                    CategoriaId = pro.CategoriaId,
                    ProveedorNombre = pro.Proveedor != null ? pro.Proveedor.NombreEmpresa : "Sin proveedor",
                    ProveedorId = pro.ProveedorId,
                    TieneStockBajo = pro.Variantes.Any(v => v.Estado && v.StockActual <= v.StockMinimo),
                    VariantesConStockBajo = pro.Variantes.Count(v => v.Estado && v.StockActual <= v.StockMinimo)
                })
                .ToListAsync();

            ViewBag.Filtros = filtros;
            ViewBag.Categorias = await _context.Categorias
                .Where(catego => catego.Estado == true)
                .Select(catego => new SelectListItem { Value = catego.IdCategoria.ToString(), Text = catego.Nombre })
                .ToListAsync();

            return View(productos);
        }

        // Ver detalles por id
        [HttpGet]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var producto = await _context.Productos
                .Include(pro => pro.Categoria)
                .Include(pro => pro.Proveedor)
                .Include(pro => pro.Variantes)
                .FirstOrDefaultAsync(m => m.IdProducto == id);

            if (producto == null)
            {
                return NotFound();
            }

            var variantesActivas = producto.Variantes?.Where(v => v.Estado).ToList() ?? new List<ProductoVariante>();

            var viewModel = new ProductoViewModel
            {
                IdProducto = producto.IdProducto,
                Nombre = producto.Nombre,
                Descripcion = producto.Descripcion,
                PrecioCompra = producto.PrecioCompra,
                PrecioVenta = producto.PrecioVenta,
                StockActual = variantesActivas.Sum(v => v.StockActual),
                ImagenUrl = producto.ImagenUrl,
                Talla = string.Join(", ", variantesActivas.Select(v => v.Talla).Distinct()),
                Color = string.Join(", ", variantesActivas.Select(v => v.Color).Distinct()),
                Estado = producto.Estado,
                FechaRegistro = producto.FechaRegistro,
                CategoriaNombre = producto.Categoria?.Nombre ?? "Sin categoria",
                CategoriaId = producto.CategoriaId,
                ProveedorNombre = producto.Proveedor?.NombreEmpresa ?? "Sin proveedor",
                ProveedorId = producto.ProveedorId,
                TieneStockBajo = variantesActivas.Any(v => v.StockActual <= v.StockMinimo),
                VariantesConStockBajo = variantesActivas.Count(v => v.StockActual <= v.StockMinimo)
            };

            ViewBag.Variantes = producto.Variantes?.ToList() ?? new List<ProductoVariante>();

            return View(viewModel);
        }

        // Mostrar formulario de creación
        [HttpGet]
        public IActionResult Create()
        {
            var viewModel = new ProductoCreateViewModel
            {
                Categorias = _context.Categorias
                    .Where(catego => catego.Estado == true)
                    .Select(catego => new SelectListItem
                    {
                        Value = catego.IdCategoria.ToString(),
                        Text = catego.Nombre
                    })
                    .ToList(),
                Proveedores = _context.Proveedores
                    .Where(pro => pro.Estado == true)
                    .Select(pro => new SelectListItem
                    {
                        Value = pro.IdProveedor.ToString(),
                        Text = pro.NombreEmpresa
                    })
                    .ToList(),
                Variantes = new List<ProductoVarianteItemViewModel>()
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductoCreateViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                CargarListasSelectViewModel(viewModel);
                return View(viewModel);
            }

            var nombreNormalizado = viewModel.Nombre.Trim().ToUpper();
            var nombreExiste = await _context.Productos
                .AnyAsync(p => p.Nombre.ToUpper() == nombreNormalizado);

            if (nombreExiste)
            {
                ModelState.AddModelError("Nombre", "Ya existe un producto con este nombre");
                CargarListasSelectViewModel(viewModel);
                return View(viewModel);
            }

            string imagenUrl = "/img/Img_Productos/default.jpg";

            if (viewModel.ImagenArchivo != null && viewModel.ImagenArchivo.Length > 0)
            {
                var extension = Path.GetExtension(viewModel.ImagenArchivo.FileName).ToLowerInvariant();
                if (extension != ".jpg" && extension != ".jpeg")
                {
                    ModelState.AddModelError("ImagenArchivo", "Solo se permiten archivos .jpg o .jpeg");
                    CargarListasSelectViewModel(viewModel);
                    return View(viewModel);
                }

                try
                {
                    imagenUrl = await GuardarImagen(viewModel.ImagenArchivo);
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("ImagenArchivo", "Error al subir la imagen: " + ex.Message);
                    CargarListasSelectViewModel(viewModel);
                    return View(viewModel);
                }
            }

            // Crear producto
            var producto = new Producto
            {
                Nombre = viewModel.Nombre,
                Descripcion = viewModel.Descripcion,
                PrecioCompra = viewModel.PrecioCompra,
                PrecioVenta = viewModel.PrecioVenta,
                ImagenUrl = imagenUrl,
                Estado = viewModel.Estado,
                CategoriaId = viewModel.CategoriaId,
                ProveedorId = viewModel.ProveedorId,
                FechaRegistro = DateTime.Now
            };

            _context.Add(producto);
            await _context.SaveChangesAsync();

            // Crear variantes
            if (viewModel.Variantes != null && viewModel.Variantes.Any())
            {
                foreach (var varianteVM in viewModel.Variantes)
                {
                    var variante = new ProductoVariante
                    {
                        ProductoId = producto.IdProducto,
                        Talla = varianteVM.Talla,
                        Color = varianteVM.Color,
                        StockActual = varianteVM.StockActual,
                        StockMinimo = varianteVM.StockMinimo,
                        Estado = varianteVM.Estado,
                        FechaRegistro = DateTime.Now
                    };

                    _context.ProductoVariantes.Add(variante);
                }

                await _context.SaveChangesAsync();

                await GenerarAlertasStockVariantes(producto.IdProducto);
            }

            TempData["SuccessMessage"] = "Producto creado exitosamente.";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var producto = await _context.Productos
                .Include(pro => pro.Variantes)
                .FirstOrDefaultAsync(pro => pro.IdProducto == id);

            if (producto == null)
            {
                return NotFound();
            }

            var viewModel = new ProductoEditViewModel
            {
                IdProducto = producto.IdProducto,
                Nombre = producto.Nombre,
                Descripcion = producto.Descripcion,
                PrecioCompra = producto.PrecioCompra,
                PrecioVenta = producto.PrecioVenta,
                ImagenUrl = producto.ImagenUrl,
                Estado = producto.Estado,
                CategoriaId = producto.CategoriaId,
                ProveedorId = producto.ProveedorId,
                FechaRegistro = producto.FechaRegistro,
                Categorias = _context.Categorias
                    .Where(catego => catego.Estado == true)
                    .Select(catego => new SelectListItem
                    {
                        Value = catego.IdCategoria.ToString(),
                        Text = catego.Nombre
                    })
                    .ToList(),
                Proveedores = _context.Proveedores
                    .Where(pro => pro.Estado == true)
                    .Select(pro => new SelectListItem
                    {
                        Value = pro.IdProveedor.ToString(),
                        Text = pro.NombreEmpresa
                    })
                    .ToList(),
                
                Variantes = producto.Variantes?.Select(v => new ProductoVarianteItemViewModel
                {
                    IdProductoVariante = v.IdProductoVariante,
                    Talla = v.Talla,
                    Color = v.Color,
                    StockActual = v.StockActual,
                    StockMinimo = v.StockMinimo,
                    Estado = v.Estado
                }).ToList() ?? new List<ProductoVarianteItemViewModel>(),

                TallasSeleccionadas = producto.Variantes?.Select(v => v.Talla).Distinct().ToList(),
                ColoresSeleccionados = producto.Variantes?.Select(v => v.Color).Distinct().ToList()
            };

            return View(viewModel);
        }

        // Actualizar producto
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ProductoEditViewModel viewModel)
        {
            if (id != viewModel.IdProducto)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                CargarListasSelectViewModel(viewModel);
                return View(viewModel);
            }

            var producto = await _context.Productos
                .Include(pro => pro.Variantes)
                .FirstOrDefaultAsync(pro => pro.IdProducto == id);

            if (producto == null)
            {
                return NotFound();
            }

            var nombreNormalizado = viewModel.Nombre.Trim().ToUpper();
            var nombreExiste = await _context.Productos
                .AnyAsync(p => p.Nombre.ToUpper() == nombreNormalizado && p.IdProducto != id);

            if (nombreExiste)
            {
                ModelState.AddModelError("Nombre", "Ya existe otro producto con este nombre");
                CargarListasSelectViewModel(viewModel);
                return View(viewModel);
            }

            if (viewModel.ImagenArchivo != null && viewModel.ImagenArchivo.Length > 0)
            {
                var extension = Path.GetExtension(viewModel.ImagenArchivo.FileName).ToLowerInvariant();
                if (extension != ".jpg" && extension != ".jpeg")
                {
                    ModelState.AddModelError("ImagenArchivo", "Solo se permiten archivos .jpg o .jpeg");
                    CargarListasSelectViewModel(viewModel);
                    return View(viewModel);
                }

                try
                {
                    if (!string.IsNullOrEmpty(producto.ImagenUrl) && !producto.ImagenUrl.Contains("default.jpg"))
                    {
                        EliminarImagen(producto.ImagenUrl);
                    }

                    producto.ImagenUrl = await GuardarImagen(viewModel.ImagenArchivo);
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("ImagenArchivo", "Error al subir la imagen: " + ex.Message);
                    CargarListasSelectViewModel(viewModel);
                    return View(viewModel);
                }
            }

            producto.Nombre = viewModel.Nombre;
            producto.Descripcion = viewModel.Descripcion;
            producto.PrecioCompra = viewModel.PrecioCompra;
            producto.PrecioVenta = viewModel.PrecioVenta;
            producto.Estado = viewModel.Estado;
            producto.CategoriaId = viewModel.CategoriaId;
            producto.ProveedorId = viewModel.ProveedorId;

            if (viewModel.Variantes != null)
            {
                var variantesIdsFormulario = viewModel.Variantes
                    .Where(v => v.IdProductoVariante.HasValue)
                    .Select(v => v.IdProductoVariante.Value)
                    .ToList();

                var variantesAEliminar = producto.Variantes
                    .Where(v => !variantesIdsFormulario.Contains(v.IdProductoVariante))
                    .ToList();

                foreach (var variante in variantesAEliminar)
                {
                    _context.ProductoVariantes.Remove(variante);
                }

                foreach (var varianteVM in viewModel.Variantes)
                {
                    if (varianteVM.IdProductoVariante.HasValue)
                    {
                        var varianteExistente = producto.Variantes
                            .FirstOrDefault(v => v.IdProductoVariante == varianteVM.IdProductoVariante);

                        if (varianteExistente != null)
                        {
                            varianteExistente.Talla = varianteVM.Talla;
                            varianteExistente.Color = varianteVM.Color;
                            varianteExistente.StockActual = varianteVM.StockActual;
                            varianteExistente.StockMinimo = varianteVM.StockMinimo;
                            varianteExistente.Estado = varianteVM.Estado;
                        }
                    }
                    else
                    {
                        var nuevaVariante = new ProductoVariante
                        {
                            ProductoId = producto.IdProducto,
                            Talla = varianteVM.Talla,
                            Color = varianteVM.Color,
                            StockActual = varianteVM.StockActual,
                            StockMinimo = varianteVM.StockMinimo,
                            Estado = varianteVM.Estado,
                            FechaRegistro = DateTime.Now
                        };

                        _context.ProductoVariantes.Add(nuevaVariante);
                    }
                }
            }

            await _context.SaveChangesAsync();

            await GenerarAlertasStockVariantes(producto.IdProducto);

            TempData["SuccessMessage"] = "Producto actualizado exitosamente.";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var producto = await _context.Productos
                .FirstOrDefaultAsync(pro => pro.IdProducto == id);

            if (producto == null)
            {
                return NotFound();
            }

            // Eliminar imagen
            if (!string.IsNullOrEmpty(producto.ImagenUrl) && !producto.ImagenUrl.Contains("default.jpg"))
            {
                EliminarImagen(producto.ImagenUrl);
            }

            _context.Productos.Remove(producto);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Producto eliminado exitosamente.";
            return RedirectToAction(nameof(Index));
        }

        // Métodos privados auxiliares
        private void CargarListasSelectViewModel(ProductoCreateViewModel viewModel)
        {
            viewModel.Categorias = _context.Categorias
                .Where(catego => catego.Estado == true)
                .Select(catego => new SelectListItem { Value = catego.IdCategoria.ToString(), Text = catego.Nombre })
                .ToList();
            viewModel.Proveedores = _context.Proveedores
                .Where(pro => pro.Estado == true)
                .Select(pro => new SelectListItem { Value = pro.IdProveedor.ToString(), Text = pro.NombreEmpresa })
                .ToList();
        }

        private void CargarListasSelectViewModel(ProductoEditViewModel viewModel)
        {
            viewModel.Categorias = _context.Categorias
                .Where(catego => catego.Estado == true)
                .Select(catego => new SelectListItem { Value = catego.IdCategoria.ToString(), Text = catego.Nombre })
                .ToList();
            viewModel.Proveedores = _context.Proveedores
                .Where(pro => pro.Estado == true)
                .Select(pro => new SelectListItem { Value = pro.IdProveedor.ToString(), Text = pro.NombreEmpresa })
                .ToList();
        }

        private async Task<string> GuardarImagen(IFormFile archivo)
        {
            var uploadPath = Path.Combine(_hostEnvironment.WebRootPath, "img", "Img_Productos");
            if (!Directory.Exists(uploadPath))
            {
                Directory.CreateDirectory(uploadPath);
            }

            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(archivo.FileName);
            var filePath = Path.Combine(uploadPath, fileName);

            using (var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                await archivo.CopyToAsync(fileStream);
                await fileStream.FlushAsync();
            }

            return "/img/Img_Productos/" + fileName;
        }

        private void EliminarImagen(string imagenUrl)
        {
            var imagePath = Path.Combine(_hostEnvironment.WebRootPath, imagenUrl.TrimStart('/').Replace('/', Path.DirectorySeparatorChar));
            if (System.IO.File.Exists(imagePath))
            {
                System.IO.File.Delete(imagePath);
            }
        }

        private async Task GenerarAlertasStockVariantes(int productoId)
        {
            var variantes = await _context.ProductoVariantes
                .Include(v => v.Producto)
                .Where(v => v.ProductoId == productoId && v.Estado)
                .ToListAsync();

            foreach (var variante in variantes)
            {
                // Verificar si existe una alerta pendiente para esta variante
                var alertaExistente = await _context.AlertaStock
                    .FirstOrDefaultAsync(a => a.ProductoVarianteId == variante.IdProductoVariante && a.Estado == "Pendiente");

                if (variante.StockActual <= variante.StockMinimo)
                {
                    if (alertaExistente == null)
                    {
                        var alerta = new AlertaStock
                        {
                            ProductoVarianteId = variante.IdProductoVariante,
                            FechaAlerta = DateTime.Now,
                            Descripcion = $"Stock bajo para {variante.Producto?.Nombre} - Talla: {variante.Talla}, Color: {variante.Color}. Stock actual: {variante.StockActual}, Mínimo: {variante.StockMinimo}",
                            Estado = "Pendiente"
                        };

                        _context.AlertaStock.Add(alerta);
                    }
                }
                else
                {
                    if (alertaExistente != null)
                    {
                        alertaExistente.Estado = "Resuelta";
                        alertaExistente.FechaResolucion = DateTime.Now;
                    }
                }
            }

            await _context.SaveChangesAsync();
        }

    }
}
