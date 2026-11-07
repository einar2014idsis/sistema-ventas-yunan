using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Proyecto_DAS.Data;
using Proyecto_DAS.Filters;
using Proyecto_DAS.Models;
using Proyecto_DAS.ViewModels.Cliente;
using System.Security.Cryptography;
using System.Text;

namespace Proyecto_DAS.Controllers
{
    [AdminOnly]
    public class ClienteController : Controller
    {
        private readonly AppDbContext _context;

        public ClienteController(AppDbContext context)
        {
            _context = context;
        }

        // Método para hashear contraseñas con SHA256
        private string HashPassword(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var clientes = await _context.Clientes
                .Include(c => c.Usuario)
                .Select(c => new ClienteViewModel
                {
                    IdCliente = c.IdCliente,
                    Nombres = c.Usuario.Nombres,
                    Apellidos = c.Usuario.Apellidos,
                    Email = c.Usuario.Email,
                    Celular = c.Usuario.Celular,
                    Direccion = c.Direccion,
                    Ciudad = c.Ciudad,
                    CodigoPostal = c.CodigoPostal,
                    DniRuc = c.DniRuc,
                    FechaNacimiento = c.FechaNacimiento,
                    Estado = c.Usuario.Estado
                })
                .OrderBy(c => c.Apellidos)
                .ToListAsync();

            return View(clientes);
        }

        [HttpGet]
        public IActionResult Create()
        {
            var viewModel = new ClienteCreateViewModel
            {
                FechaNacimiento = DateTime.Today.AddYears(-18)
            };
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ClienteCreateViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(viewModel);
            }

            // Verificar email
            if (await _context.Usuarios.AnyAsync(u => u.Email == viewModel.Email))
            {
                ModelState.AddModelError("Email", "Este correo ya está registrado");
                return View(viewModel);
            }

            // Verificar celular
            if (await _context.Usuarios.AnyAsync(u => u.Celular == viewModel.Celular))
            {
                ModelState.AddModelError("Celular", "Este número de celular ya está registrado");
                return View(viewModel);
            }

            // Validar que el celular empiece con 9 y tenga 9 dígitos
            if (!Helpers.ValidationHelper.EsTelefonoValido(viewModel.Celular))
            {
                ModelState.AddModelError("Celular", "El celular debe empezar con 9 y tener exactamente 9 dígitos");
                return View(viewModel);
            }

            // Verificar DNI/RUC
            if (await _context.Clientes.AnyAsync(c => c.DniRuc == viewModel.DniRuc))
            {
                ModelState.AddModelError("DniRuc", "Este DNI/RUC ya está registrado");
                return View(viewModel);
            }

            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                // Crear Usuario
                var usuario = new Usuario
                {
                    Nombres = viewModel.Nombres,
                    Apellidos = viewModel.Apellidos,
                    Email = viewModel.Email,
                    Contrasenia = HashPassword(viewModel.Contrasenia),
                    Celular = viewModel.Celular,
                    Rol = "Cliente",
                    FechaRegistro = DateTime.Now,
                    Estado = true
                };

                _context.Usuarios.Add(usuario);
                await _context.SaveChangesAsync();

                // Crear Cliente
                var cliente = new Cliente
                {
                    UsuarioId = usuario.IdUsuario,
                    Direccion = viewModel.Direccion,
                    Ciudad = viewModel.Ciudad,
                    CodigoPostal = viewModel.CodigoPostal,
                    DniRuc = viewModel.DniRuc,
                    FechaNacimiento = viewModel.FechaNacimiento
                };

                _context.Clientes.Add(cliente);
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();

                TempData["SuccessMessage"] = "Cliente creado exitosamente";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                ModelState.AddModelError("", "Error al crear el cliente");
                return View(viewModel);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var cliente = await _context.Clientes
                .Include(c => c.Usuario)
                .FirstOrDefaultAsync(c => c.IdCliente == id);

            if (cliente == null)
            {
                return NotFound();
            }

            var viewModel = new ClienteEditViewModel
            {
                IdCliente = cliente.IdCliente,
                UsuarioId = cliente.UsuarioId,
                Nombres = cliente.Usuario.Nombres,
                Apellidos = cliente.Usuario.Apellidos,
                Email = cliente.Usuario.Email,
                Celular = cliente.Usuario.Celular,
                Estado = cliente.Usuario.Estado,
                Direccion = cliente.Direccion,
                Ciudad = cliente.Ciudad,
                CodigoPostal = cliente.CodigoPostal,
                DniRuc = cliente.DniRuc,
                FechaNacimiento = cliente.FechaNacimiento
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ClienteEditViewModel viewModel)
        {
            if (id != viewModel.IdCliente)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return View(viewModel);
            }

            var cliente = await _context.Clientes
                .Include(c => c.Usuario)
                .FirstOrDefaultAsync(c => c.IdCliente == id);

            if (cliente == null)
            {
                return NotFound();
            }

            // Verificar email
            if (await _context.Usuarios.AnyAsync(u => u.Email == viewModel.Email && u.IdUsuario != viewModel.UsuarioId))
            {
                ModelState.AddModelError("Email", "Este correo ya está registrado por otro usuario");
                return View(viewModel);
            }

            // Verificar celular
            if (await _context.Usuarios.AnyAsync(u => u.Celular == viewModel.Celular && u.IdUsuario != viewModel.UsuarioId))
            {
                ModelState.AddModelError("Celular", "Este número de celular ya está registrado por otro usuario");
                return View(viewModel);
            }

            // Validar que el celular empiece con 9 y tenga 9 dígitos
            if (!Helpers.ValidationHelper.EsTelefonoValido(viewModel.Celular))
            {
                ModelState.AddModelError("Celular", "El celular debe empezar con 9 y tener exactamente 9 dígitos");
                return View(viewModel);
            }

            // Verificar DNI/RUC
            if (await _context.Clientes.AnyAsync(c => c.DniRuc == viewModel.DniRuc && c.IdCliente != id))
            {
                ModelState.AddModelError("DniRuc", "Este DNI/RUC ya está registrado por otro cliente");
                return View(viewModel);
            }

            try
            {
                // Actualizar Usuario
                cliente.Usuario.Nombres = viewModel.Nombres;
                cliente.Usuario.Apellidos = viewModel.Apellidos;
                cliente.Usuario.Email = viewModel.Email;
                cliente.Usuario.Celular = viewModel.Celular;
                cliente.Usuario.Estado = viewModel.Estado;

                if (!string.IsNullOrEmpty(viewModel.NuevaContrasenia))
                {
                    cliente.Usuario.Contrasenia = HashPassword(viewModel.NuevaContrasenia);
                }

                cliente.Direccion = viewModel.Direccion;
                cliente.Ciudad = viewModel.Ciudad;
                cliente.CodigoPostal = viewModel.CodigoPostal;
                cliente.DniRuc = viewModel.DniRuc;
                cliente.FechaNacimiento = viewModel.FechaNacimiento;

                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Cliente actualizado exitosamente";
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateConcurrencyException)
            {
                ModelState.AddModelError("", "Error al actualizar el cliente");
                return View(viewModel);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var cliente = await _context.Clientes
                .Include(c => c.Usuario)
                .FirstOrDefaultAsync(c => c.IdCliente == id);

            if (cliente == null)
            {
                return NotFound();
            }

            var viewModel = new ClienteViewModel
            {
                IdCliente = cliente.IdCliente,
                Nombres = cliente.Usuario.Nombres,
                Apellidos = cliente.Usuario.Apellidos,
                Email = cliente.Usuario.Email,
                Celular = cliente.Usuario.Celular,
                Direccion = cliente.Direccion,
                Ciudad = cliente.Ciudad,
                CodigoPostal = cliente.CodigoPostal,
                DniRuc = cliente.DniRuc,
                FechaNacimiento = cliente.FechaNacimiento,
                Estado = cliente.Usuario.Estado
            };

            return View(viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var cliente = await _context.Clientes
                .Include(c => c.Usuario)
                .Include(c => c.Ventas)
                .FirstOrDefaultAsync(c => c.IdCliente == id);

            if (cliente == null)
            {
                return NotFound();
            }

            // Verificar si tiene ventas asociadas
            if (cliente.Ventas != null && cliente.Ventas.Any())
            {
                TempData["ErrorMessage"] = "No se puede eliminar el cliente porque tiene compras asociadas";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                _context.Clientes.Remove(cliente);
                _context.Usuarios.Remove(cliente.Usuario);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Cliente eliminado exitosamente";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception)
            {
                TempData["ErrorMessage"] = "Error al eliminar el cliente";
                return RedirectToAction(nameof(Index));
            }
        }
    }
}
