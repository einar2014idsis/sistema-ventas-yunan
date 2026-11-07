using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Proyecto_DAS.Data;
using Proyecto_DAS.Filters;
using Proyecto_DAS.Models;
using Proyecto_DAS.ViewModels.Administrador;
using System.Security.Cryptography;
using System.Text;

namespace Proyecto_DAS.Controllers
{
    [AdminOnly]
    public class AdministradorController : Controller
    {
        private readonly AppDbContext _context;

        public AdministradorController(AppDbContext context)
        {
            _context = context;
        }

        // Hasheo de contraseñas con SHA256
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
            var administradores = await _context.Administradores
                .Include(a => a.Usuario)
                .Select(a => new AdministradorViewModel
                {
                    IdAdministrador = a.IdAdministrador,
                    Nombres = a.Usuario.Nombres,
                    Apellidos = a.Usuario.Apellidos,
                    Email = a.Usuario.Email,
                    Celular = a.Usuario.Celular,
                    Cargo = a.Cargo,
                    FechaContratacion = a.FechaContratacion,
                    Estado = a.Usuario.Estado
                })
                .OrderBy(a => a.Apellidos)
                .ToListAsync();

            return View(administradores);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var administrador = await _context.Administradores
                .Include(a => a.Usuario)
                .FirstOrDefaultAsync(a => a.IdAdministrador == id);

            if (administrador == null)
            {
                return NotFound();
            }

            var viewModel = new AdministradorEditViewModel
            {
                IdAdministrador = administrador.IdAdministrador,
                UsuarioId = administrador.UsuarioId,
                Nombres = administrador.Usuario.Nombres,
                Apellidos = administrador.Usuario.Apellidos,
                Email = administrador.Usuario.Email,
                Celular = administrador.Usuario.Celular,
                Estado = administrador.Usuario.Estado,
                Cargo = administrador.Cargo,
                FechaContratacion = administrador.FechaContratacion
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, AdministradorEditViewModel viewModel)
        {
            if (id != viewModel.IdAdministrador)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return View(viewModel);
            }

            var administrador = await _context.Administradores
                .Include(a => a.Usuario)
                .FirstOrDefaultAsync(a => a.IdAdministrador == id);

            if (administrador == null)
            {
                return NotFound();
            }

            if (await _context.Usuarios.AnyAsync(u => u.Email == viewModel.Email && u.IdUsuario != viewModel.UsuarioId))
            {
                ModelState.AddModelError("Email", "Este correo ya está registrado por otro usuario");
                return View(viewModel);
            }

            try
            {
                administrador.Usuario.Nombres = viewModel.Nombres;
                administrador.Usuario.Apellidos = viewModel.Apellidos;
                administrador.Usuario.Email = viewModel.Email;
                administrador.Usuario.Celular = viewModel.Celular;
                administrador.Usuario.Estado = viewModel.Estado;

                if (!string.IsNullOrEmpty(viewModel.NuevaContrasenia))
                {
                    administrador.Usuario.Contrasenia = HashPassword(viewModel.NuevaContrasenia);
                }

                administrador.Cargo = viewModel.Cargo;
                administrador.FechaContratacion = viewModel.FechaContratacion;

                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Administrador actualizado exitosamente";
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateConcurrencyException)
            {
                ModelState.AddModelError("", "Error al actualizar el administrador");
                return View(viewModel);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var administrador = await _context.Administradores
                .Include(a => a.Usuario)
                .FirstOrDefaultAsync(a => a.IdAdministrador == id);

            if (administrador == null)
            {
                return NotFound();
            }

            var viewModel = new AdministradorViewModel
            {
                IdAdministrador = administrador.IdAdministrador,
                Nombres = administrador.Usuario.Nombres,
                Apellidos = administrador.Usuario.Apellidos,
                Email = administrador.Usuario.Email,
                Celular = administrador.Usuario.Celular,
                Cargo = administrador.Cargo,
                FechaContratacion = administrador.FechaContratacion,
                Estado = administrador.Usuario.Estado
            };

            return View(viewModel);
        }
    }
}
