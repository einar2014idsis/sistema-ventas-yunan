using System.ComponentModel.DataAnnotations;
using Proyecto_DAS.Helpers;

namespace Proyecto_DAS.ViewModels.Categoria
{
    public class CategoriaEditViewModel : IValidatableObject
    {
        [Required]
        public int IdCategoria { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio")]
        [StringLength(50, MinimumLength = 4, ErrorMessage = "El nombre debe tener entre 4 y 50 caracteres")]
        [Display(Name = "Nombre")]
        public string Nombre { get; set; }

        [Required(ErrorMessage = "La descripción es obligatoria")]
        [StringLength(200, MinimumLength = 5, ErrorMessage = "La descripción debe tener entre 5 y 200 caracteres")]
        [Display(Name = "Descripción")]
        public string Descripcion { get; set; }

        [Display(Name = "Activo")]
        public bool Estado { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            // Validar Nombre
            if (!string.IsNullOrWhiteSpace(Nombre))
            {
                Nombre = ValidationHelper.LimpiarTexto(Nombre);
                if (!ValidationHelper.EsLetrasNumerosYEspacios(Nombre))
                {
                    yield return new ValidationResult(
                        "El nombre solo puede contener letras, números y espacios",
                        new[] { nameof(Nombre) }
                    );
                }
                else if (!ValidationHelper.ContieneAlMenosUnaLetra(Nombre))
                {
                    yield return new ValidationResult(
                        "El nombre debe contener al menos una letra",
                        new[] { nameof(Nombre) }
                    );
                }
            }

            // Validar Descripción
            if (!string.IsNullOrWhiteSpace(Descripcion))
            {
                Descripcion = ValidationHelper.LimpiarTexto(Descripcion);
                if (!ValidationHelper.TieneLongitudMinima(Descripcion, 5))
                {
                    yield return new ValidationResult(
                        "La descripción debe tener al menos 5 caracteres",
                        new[] { nameof(Descripcion) }
                    );
                }
            }
        }
    }
}
