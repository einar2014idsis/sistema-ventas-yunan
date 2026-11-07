using System.ComponentModel.DataAnnotations;
using Proyecto_DAS.Helpers;

namespace Proyecto_DAS.ViewModels.Proveedor
{
    public class ProveedorCreateViewModel : IValidatableObject
    {
        [Required(ErrorMessage = "El nombre de la empresa es obligatorio")]
        [StringLength(100, MinimumLength = 4, ErrorMessage = "El nombre debe tener entre 4 y 100 caracteres")]
        [Display(Name = "Nombre de Empresa")]
        public string NombreEmpresa { get; set; }

        [Required(ErrorMessage = "El nombre del contacto es obligatorio")]
        [StringLength(100, MinimumLength = 4, ErrorMessage = "El nombre debe tener entre 4 y 100 caracteres")]
        [Display(Name = "Nombre de Contacto")]
        public string ContactoNombre { get; set; }

        [Required(ErrorMessage = "El celular es obligatorio")]
        [StringLength(9, MinimumLength = 9, ErrorMessage = "El celular debe tener exactamente 9 dígitos")]
        [Display(Name = "Celular")]
        public string Celular { get; set; }

        [Required(ErrorMessage = "El email es obligatorio")]
        [StringLength(100, ErrorMessage = "El email no puede exceder 100 caracteres")]
        [EmailAddress(ErrorMessage = "El formato del email no es válido")]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required(ErrorMessage = "La dirección es obligatoria")]
        [StringLength(200, MinimumLength = 15, ErrorMessage = "La dirección debe tener entre 15 y 200 caracteres")]
        [Display(Name = "Dirección")]
        public string Direccion { get; set; }

        [Required(ErrorMessage = "El RUC es obligatorio")]
        [StringLength(11, MinimumLength = 11, ErrorMessage = "El RUC debe tener exactamente 11 dígitos")]
        [RegularExpression(@"^\d{11}$", ErrorMessage = "El RUC debe contener solo números")]
        [Display(Name = "RUC")]
        public string Ruc { get; set; }

        [Display(Name = "Estado Activo")]
        public bool Estado { get; set; } = true;

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            // Nombre de empresa
            if (!string.IsNullOrWhiteSpace(NombreEmpresa))
            {
                NombreEmpresa = ValidationHelper.LimpiarTexto(NombreEmpresa);
                if (!ValidationHelper.EsLetrasNumerosYEspacios(NombreEmpresa))
                {
                    yield return new ValidationResult(
                        "El nombre de la empresa solo puede contener letras, números y espacios (sin caracteres especiales)",
                        new[] { nameof(NombreEmpresa) }
                    );
                }
                else if (!ValidationHelper.ContieneAlMenosUnaLetra(NombreEmpresa))
                {
                    yield return new ValidationResult(
                        "El nombre de la empresa debe contener al menos una letra",
                        new[] { nameof(NombreEmpresa) }
                    );
                }
            }

            // Nombre de contacto
            if (!string.IsNullOrWhiteSpace(ContactoNombre))
            {
                ContactoNombre = ValidationHelper.LimpiarTexto(ContactoNombre);
                if (!ValidationHelper.EsSoloLetrasYEspacios(ContactoNombre))
                {
                    yield return new ValidationResult(
                        "El nombre del contacto solo puede contener letras y espacios (sin números ni caracteres especiales)",
                        new[] { nameof(ContactoNombre) }
                    );
                }
            }

            // Celular
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

            // Email
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

            // Dirección
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

            // RUC 
            if (!string.IsNullOrWhiteSpace(Ruc))
            {
                Ruc = Ruc.Trim();
                if (!ValidationHelper.EsRucValido(Ruc))
                {
                    yield return new ValidationResult(
                        "El RUC debe contener exactamente 11 dígitos (solo números)",
                        new[] { nameof(Ruc) }
                    );
                }
            }
        }
    }
}
