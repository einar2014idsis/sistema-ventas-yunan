using System.ComponentModel.DataAnnotations;
using Proyecto_DAS.Helpers;

namespace Proyecto_DAS.ViewModels.Auth
{
    public class LoginViewModel : IValidatableObject
    {
        [Required(ErrorMessage = "El correo es obligatorio")]
        [EmailAddress(ErrorMessage = "Formato de correo inválido")]
        [Display(Name = "Correo")]
        public string Correo { get; set; }

        [Required(ErrorMessage = "La contraseña es obligatoria")]
        [Display(Name = "Contraseña")]
        public string Contrasenia { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            // Validar Email
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
        }
    }
}
