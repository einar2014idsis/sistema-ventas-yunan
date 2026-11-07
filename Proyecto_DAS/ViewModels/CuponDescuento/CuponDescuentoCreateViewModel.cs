using System.ComponentModel.DataAnnotations;
using Proyecto_DAS.Helpers;

namespace Proyecto_DAS.ViewModels.CuponDescuento
{
    public class CuponDescuentoCreateViewModel : IValidatableObject
    {
        [Required(ErrorMessage = "El código del cupón es obligatorio")]
        [StringLength(12, MinimumLength = 6, ErrorMessage = "El código debe tener entre 6 y 12 caracteres")]
        [Display(Name = "Código Cupón")]
        public string CodigoCupon { get; set; }

        [Required(ErrorMessage = "El porcentaje de descuento es obligatorio")]
        [Range(1, 95, ErrorMessage = "El porcentaje debe estar entre 1% y 95% (máximo 95%)")]
        [Display(Name = "Porcentaje de Descuento (%)")]
        public decimal PorcentajeDescuento { get; set; }

        [Required(ErrorMessage = "La fecha de inicio es obligatoria")]
        [Display(Name = "Fecha Inicio")]
        [DataType(DataType.Date)]
        public DateTime FechaInicio { get; set; }

        [Required(ErrorMessage = "La fecha de vencimiento es obligatoria")]
        [Display(Name = "Fecha Vencimiento")]
        [DataType(DataType.Date)]
        public DateTime FechaVencimiento { get; set; }

        [Display(Name = "Estado")]
        public bool Estado { get; set; } = true;

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (!string.IsNullOrWhiteSpace(CodigoCupon))
            {
                CodigoCupon = CodigoCupon.Trim().ToUpper(); // Convertir a mayúsculas automáticamente
                if (!ValidationHelper.EsCodigoCuponValido(CodigoCupon))
                {
                    yield return new ValidationResult(
                        "El código del cupón solo puede contener letras mayúsculas y números (6-12 caracteres, sin espacios)",
                        new[] { nameof(CodigoCupon) }
                    );
                }
                else if (!ValidationHelper.ContieneAlMenosUnaLetra(CodigoCupon))
                {
                    yield return new ValidationResult(
                        "El código del cupón debe contener al menos una letra",
                        new[] { nameof(CodigoCupon) }
                    );
                }
            }

            if (!ValidationHelper.EsPorcentajeDescuentoValido(PorcentajeDescuento))
            {
                yield return new ValidationResult(
                    "El porcentaje de descuento debe estar entre 1% y 95% (máximo 95%, no se permite 100%)",
                    new[] { nameof(PorcentajeDescuento) }
                );
            }

            if (!ValidationHelper.EsFechaValidaDesdeCreacionEmpresa(FechaInicio))
            {
                yield return new ValidationResult(
                    $"La fecha de inicio no puede ser anterior al año {ValidationHelper.ANIO_CREACION_EMPRESA} (año de creación de la empresa)",
                    new[] { nameof(FechaInicio) }
                );
            }

            if (FechaVencimiento <= FechaInicio)
            {
                yield return new ValidationResult(
                    "La fecha de vencimiento debe ser posterior a la fecha de inicio",
                    new[] { nameof(FechaVencimiento) }
                );
            }

            if (!ValidationHelper.EsFechaVencimientoCuponValida(FechaVencimiento))
            {
                var fechaMaxima = DateTime.Now.AddMonths(ValidationHelper.MESES_MAXIMOS_CUPON);
                yield return new ValidationResult(
                    $"La fecha de vencimiento no puede ser mayor a {ValidationHelper.MESES_MAXIMOS_CUPON} meses desde hoy (máximo: {fechaMaxima:dd/MM/yyyy}). No se pueden crear cupones con tanta anticipación.",
                    new[] { nameof(FechaVencimiento) }
                );
            }

            var unMesAtras = DateTime.Now.AddMonths(-1);
            if (FechaInicio < unMesAtras)
            {
                yield return new ValidationResult(
                    $"La fecha de inicio no puede ser anterior a {unMesAtras:dd/MM/yyyy} (máximo 1 mes atrás)",
                    new[] { nameof(FechaInicio) }
                );
            }
        }
    }
}
