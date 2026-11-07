using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Proyecto_DAS.Data;
using Proyecto_DAS.Models;
using Proyecto_DAS.ViewModels.Auth;

namespace Proyecto_DAS.Controllers
{
    public class AuthController : Controller
    {
        private readonly AppDbContext _context;

        public AuthController(AppDbContext context)
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
        public IActionResult Login()
        {
            if (TempData["Mensaje"] != null)
            {
                ViewBag.Mensaje = TempData["Mensaje"].ToString();
            }

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            string contraHasheada = HashPassword(model.Contrasenia);

            // Primero buscar usuario solo por email y contraseña
            var usuario = _context.Usuarios
                .FirstOrDefault(u =>
                    u.Email == model.Correo &&
                    u.Contrasenia == contraHasheada);

            if (usuario == null)
            {
                ModelState.AddModelError(string.Empty, "Credenciales incorrectas.");
                return View(model);
            }

            // Verificar si la cuenta está desactivada
            if (!usuario.Estado)
            {
                ModelState.AddModelError(string.Empty, "Tu cuenta ha sido desactivada. Contacta al administrador.");
                return View(model);
            }

            // Guardar datos en sesión
            HttpContext.Session.SetInt32("UsuarioId", usuario.IdUsuario);
            HttpContext.Session.SetString("Rol", usuario.Rol);
            HttpContext.Session.SetString("NombreUsuario", usuario.Nombres);

            if (usuario.Rol == "Cliente")
            {
                var cliente = _context.Clientes.FirstOrDefault(c => c.UsuarioId == usuario.IdUsuario);
                if (cliente != null)
                {
                    HttpContext.Session.SetInt32("ClienteId", cliente.IdCliente);
                }
                return RedirectToAction("Index", "ClientePanel");
            }

            if (usuario.Rol == "Administrador")
            {
                return RedirectToAction("PanelAdministrador", "Home");
            }

            return RedirectToAction("Index", "ClientePanel");
        }


        [HttpGet]
        public IActionResult RegisterCliente()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RegisterCliente(RegisterClienteViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var emailExiste = _context.Usuarios
                .FirstOrDefault(u => u.Email == model.Correo);

            if (emailExiste != null)
            {
                ModelState.AddModelError("Correo", "Ya existe un usuario con ese correo.");
                return View(model);
            }

            // Validar DNI/RUC
            var dniRucExiste = _context.Clientes
                .FirstOrDefault(c => c.DniRuc == model.DniRuc);

            if (dniRucExiste != null)
            {
                ModelState.AddModelError("DniRuc", "Ya existe un cliente con ese DNI/RUC.");
                return View(model);
            }

            // Validar celular
            var celularExiste = _context.Usuarios
                .FirstOrDefault(u => u.Celular == model.Celular);

            if (celularExiste != null)
            {
                ModelState.AddModelError("Celular", "Ya existe un usuario con ese número de celular.");
                return View(model);
            }

            if (!Helpers.ValidationHelper.EsTelefonoValido(model.Celular))
            {
                ModelState.AddModelError("Celular", "El celular debe empezar con 9 y tener exactamente 9 dígitos.");
                return View(model);
            }

            // Validar contraseña segura
            if (!Helpers.ValidationHelper.EsContrasenaSegura(model.Contrasenia))
            {
                ModelState.AddModelError("Contrasenia", "La contraseña debe tener mínimo 8 caracteres, una mayúscula, una minúscula, un número y un carácter especial (@$!%*?&).");
                return View(model);
            }

            var usuario = new Usuario
            {
                Nombres = model.Nombres,
                Apellidos = model.Apellidos,
                Email = model.Correo,
                Contrasenia = HashPassword(model.Contrasenia),
                Celular = model.Celular,
                Rol = "Cliente",
                FechaRegistro = System.DateTime.Now,
                Estado = true
            };

            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync();

            var cliente = new Cliente
            {
                UsuarioId = usuario.IdUsuario,
                Direccion = model.Direccion,
                Ciudad = model.Ciudad,
                CodigoPostal = model.CodigoPostal,
                DniRuc = model.DniRuc,
                FechaNacimiento = model.FechaNacimiento
            };

            _context.Clientes.Add(cliente);
            await _context.SaveChangesAsync();

            TempData["Mensaje"] = "Registro exitoso. Ahora puede iniciar sesión.";
            return RedirectToAction("Login");
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login", "Auth");
        }
    }
}
