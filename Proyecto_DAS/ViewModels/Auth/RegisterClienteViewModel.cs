using System.ComponentModel.DataAnnotations;
using Proyecto_DAS.Helpers;

namespace Proyecto_DAS.ViewModels.Auth
{
    public class RegisterClienteViewModel : IValidatableObject
    {
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
        [Display(Name = "Correo")]
        public string Correo { get; set; }

        [Required(ErrorMessage = "La contraseña es obligatoria")]
        [StringLength(100, MinimumLength = 8, ErrorMessage = "La contraseña debe tener mínimo 8 caracteres")]
        [DataType(DataType.Password)]
        [Display(Name = "Contraseña")]
        public string Contrasenia { get; set; }

        [Required(ErrorMessage = "Debe confirmar la contraseña")]
        [DataType(DataType.Password)]
        [Display(Name = "Confirmar Contraseña")]
        [Compare("Contrasenia", ErrorMessage = "Las contraseñas no coinciden")]
        public string ConfirmarContrasenia { get; set; }

        [Required(ErrorMessage = "El celular es obligatorio")]
        [StringLength(9, MinimumLength = 9, ErrorMessage = "El celular debe tener exactamente 9 dígitos")]
        [Display(Name = "Celular")]
        public string Celular { get; set; }

        // Campos adicionales para Cliente
        [Required(ErrorMessage = "La dirección es obligatoria")]
        [StringLength(200, MinimumLength = 5, ErrorMessage = "La dirección debe tener entre 5 y 200 caracteres")]
        [Display(Name = "Dirección")]
        public string Direccion { get; set; }

        [Required(ErrorMessage = "La ciudad es obligatoria")]
        [StringLength(50, MinimumLength = 4, ErrorMessage = "La ciudad debe tener entre 4 y 50 caracteres")]
        [Display(Name = "Ciudad")]
        public string Ciudad { get; set; }

        [Required(ErrorMessage = "El código postal es obligatorio")]
        [StringLength(6, MinimumLength = 4, ErrorMessage = "El código postal debe tener entre 4 y 6 dígitos")]
        [Display(Name = "Código Postal")]
        public string CodigoPostal { get; set; }

        [Required(ErrorMessage = "El DNI/RUC es obligatorio")]
        [StringLength(11, MinimumLength = 8, ErrorMessage = "El DNI debe tener 8 dígitos o el RUC 11 dígitos")]
        [Display(Name = "DNI/RUC")]
        public string DniRuc { get; set; }

        [Required(ErrorMessage = "La fecha de nacimiento es obligatoria")]
        [DataType(DataType.Date)]
        [Display(Name = "Fecha de Nacimiento")]
        public DateTime FechaNacimiento { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            // Validar

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

            if (!string.IsNullOrWhiteSpace(Correo))
            {
                Correo = Correo.Trim();
                if (!ValidationHelper.EsEmailValido(Correo))
                {
                    yield return new ValidationResult(
                        "El formato del correo electrónico no es válido",
                        new[] { nameof(Correo) }
                    );
                }
            }

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

            if (!string.IsNullOrWhiteSpace(Direccion))
            {
                Direccion = ValidationHelper.LimpiarTexto(Direccion);
                if (!ValidationHelper.TieneLongitudMinima(Direccion, 5))
                {
                    yield return new ValidationResult(
                        "La dirección debe tener al menos 5 caracteres",
                        new[] { nameof(Direccion) }
                    );
                }
            }

            if (!string.IsNullOrWhiteSpace(Ciudad))
            {
                Ciudad = ValidationHelper.LimpiarTexto(Ciudad);
                if (!ValidationHelper.EsSoloLetrasYEspacios(Ciudad))
                {
                    yield return new ValidationResult(
                        "La ciudad solo puede contener letras y espacios",
                        new[] { nameof(Ciudad) }
                    );
                }
            }

            if (!string.IsNullOrWhiteSpace(CodigoPostal))
            {
                CodigoPostal = CodigoPostal.Trim();
                if (!ValidationHelper.EsCodigoPostalValido(CodigoPostal))
                {
                    yield return new ValidationResult(
                        "El código postal debe tener entre 4 y 6 dígitos, solo números y no puede ser todo ceros",
                        new[] { nameof(CodigoPostal) }
                    );
                }
            }

            if (!string.IsNullOrWhiteSpace(DniRuc))
            {
                DniRuc = DniRuc.Trim();
                bool esDniValido = ValidationHelper.EsDniValido(DniRuc);
                bool esRucValido = ValidationHelper.EsRucValido(DniRuc);

                if (!esDniValido && !esRucValido)
                {
                    yield return new ValidationResult(
                        "El DNI debe tener 8 dígitos o el RUC 11 dígitos (solo números)",
                        new[] { nameof(DniRuc) }
                    );
                }
            }

            if (FechaNacimiento != default)
            {
                var hoy = DateTime.Now;
                var edad = hoy.Year - FechaNacimiento.Year;
                if (FechaNacimiento > hoy.AddYears(-edad)) edad--;

                if (FechaNacimiento >= hoy)
                {
                    yield return new ValidationResult(
                        "La fecha de nacimiento no puede ser hoy ni una fecha futura",
                        new[] { nameof(FechaNacimiento) }
                    );
                }
                else if (edad < 18)
                {
                    yield return new ValidationResult(
                        "Debes ser mayor de 18 años para registrarte",
                        new[] { nameof(FechaNacimiento) }
                    );
                }
                else if (edad > 120)
                {
                    yield return new ValidationResult(
                        "La fecha de nacimiento no es válida",
                        new[] { nameof(FechaNacimiento) }
                    );
                }
            }

            if (!string.IsNullOrWhiteSpace(Contrasenia))
            {
                if (!ValidationHelper.EsContrasenaSegura(Contrasenia))
                {
                    yield return new ValidationResult(
                        "La contraseña debe tener mínimo 8 caracteres, una mayúscula, una minúscula, un número y un carácter especial (@$!%*?&.-)",
                        new[] { nameof(Contrasenia) }
                    );
                }
            }
        }
    }
}
