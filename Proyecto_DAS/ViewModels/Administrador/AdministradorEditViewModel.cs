using System.ComponentModel.DataAnnotations;
using Proyecto_DAS.Helpers;

namespace Proyecto_DAS.ViewModels.Administrador
{
    public class AdministradorEditViewModel : IValidatableObject
    {
        public int IdAdministrador { get; set; }
        public int UsuarioId { get; set; }

        // Datos de Usuario
        [Required(ErrorMessage = "Los nombres son obligatorios")]
        [StringLength(100, MinimumLength = 4, ErrorMessage = "Los nombres deben tener entre 4 y 100 caracteres")]
        [Display(Name = "Nombres")]
        public string Nombres { get; set; }

        [Required(ErrorMessage = "Los apellidos son obligatorios")]
        [StringLength(100, MinimumLength = 4, ErrorMessage = "Los apellidos deben tener entre 4 y 100 caracteres")]
        [Display(Name = "Apellidos")]
        public string Apellidos { get; set; }

        [Required(ErrorMessage = "El correo es obligatorio")]
        [EmailAddress(ErrorMessage = "Formato de correo inválido")]
        [StringLength(100, ErrorMessage = "Máximo 100 caracteres")]
        [Display(Name = "Correo Electrónico")]
        public string Email { get; set; }

        [StringLength(100, MinimumLength = 8, ErrorMessage = "La contraseña debe tener mínimo 8 caracteres")]
        [DataType(DataType.Password)]
        [Display(Name = "Nueva Contraseña (dejar vacío para mantener)")]
        public string? NuevaContrasenia { get; set; }

        [Required(ErrorMessage = "El celular es obligatorio")]
        [StringLength(9, MinimumLength = 9, ErrorMessage = "El celular debe tener exactamente 9 dígitos")]
        [Display(Name = "Celular")]
        public string Celular { get; set; }

        [Display(Name = "Estado")]
        public bool Estado { get; set; }

        // Datos de Administrador
        [Required(ErrorMessage = "El cargo es obligatorio")]
        [Display(Name = "Cargo")]
        public string Cargo { get; set; }

        [Required(ErrorMessage = "La fecha de contratación es obligatoria")]
        [DataType(DataType.Date)]
        [Display(Name = "Fecha de Contratación")]
        public DateTime FechaContratacion { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            // Validar Nombres
            if (!string.IsNullOrWhiteSpace(Nombres))
            {
                Nombres = ValidationHelper.LimpiarTexto(Nombres);
                if (!ValidationHelper.EsSoloLetrasYEspacios(Nombres))
                {
                    yield return new ValidationResult(
                        "Los nombres solo pueden contener letras y espacios",
                        new[] { nameof(Nombres) }
                    );
                }
            }

            // Validar Apellidos
            if (!string.IsNullOrWhiteSpace(Apellidos))
            {
                Apellidos = ValidationHelper.LimpiarTexto(Apellidos);
                if (!ValidationHelper.EsSoloLetrasYEspacios(Apellidos))
                {
                    yield return new ValidationResult(
                        "Los apellidos solo pueden contener letras y espacios",
                        new[] { nameof(Apellidos) }
                    );
                }
            }

            // Validar Email
            if (!string.IsNullOrWhiteSpace(Email))
            {
                Email = Email.Trim();
                if (!ValidationHelper.EsEmailValido(Email))
                {
                    yield return new ValidationResult(
                        "El formato del correo electrónico no es válido",
                        new[] { nameof(Email) }
                    );
                }
            }

            // Validar Celular
            if (!string.IsNullOrWhiteSpace(Celular))
            {
                Celular = Celular.Trim();
                if (!ValidationHelper.EsTelefonoValido(Celular))
                {
                    yield return new ValidationResult(
                        "El celular debe tener 9 dígitos y empezar con 9 (ej: 987654321)",
                        new[] { nameof(Celular) }
                    );
                }
            }

            // Validar Cargo
            if (!string.IsNullOrWhiteSpace(Cargo))
            {
                Cargo = ValidationHelper.LimpiarTexto(Cargo);
                if (!ValidationHelper.EsLetrasNumerosYEspacios(Cargo))
                {
                    yield return new ValidationResult(
                        "El cargo solo puede contener letras, números y espacios",
                        new[] { nameof(Cargo) }
                    );
                }
            }

            // Validar Fecha de Contratación
            if (FechaContratacion != default)
            {
                if (!ValidationHelper.EsFechaValidaDesdeCreacionEmpresa(FechaContratacion))
                {
                    yield return new ValidationResult(
                        $"La fecha de contratación debe ser desde {ValidationHelper.ANIO_CREACION_EMPRESA}",
                        new[] { nameof(FechaContratacion) }
                    );
                }

                if (FechaContratacion > DateTime.Now)
                {
                    yield return new ValidationResult(
                        "La fecha de contratación no puede ser futura",
                        new[] { nameof(FechaContratacion) }
                    );
                }
            }

            if (!string.IsNullOrWhiteSpace(NuevaContrasenia))
            {
                if (!ValidationHelper.EsContrasenaSegura(NuevaContrasenia))
                {
                    yield return new ValidationResult(
                        "La contraseña debe tener mínimo 8 caracteres, una mayúscula, una minúscula, un número y un carácter especial (@$!%*?&.-)",
                        new[] { nameof(NuevaContrasenia) }
                    );
                }
            }
        }
    }
}
