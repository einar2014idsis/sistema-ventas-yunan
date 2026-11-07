using System.ComponentModel.DataAnnotations;

namespace Proyecto_DAS.ViewModels.Producto
{
    public class ProductoVarianteItemViewModel
    {
        public int? IdProductoVariante { get; set; }

        [Required(ErrorMessage = "La talla es obligatoria")]
        [StringLength(10)]
        public string Talla { get; set; }

        [Required(ErrorMessage = "El color es obligatorio")]
        [StringLength(20, ErrorMessage = "El color no puede tener más de 20 caracteres")]
        public string Color { get; set; }

        [Required(ErrorMessage = "El stock actual es obligatorio")]
        [Range(0, 999999, ErrorMessage = "El stock debe ser un número positivo")]
        [Display(Name = "Stock Actual")]
        public int StockActual { get; set; }

        [Required(ErrorMessage = "El stock mínimo es obligatorio")]
        [Range(0, 999999, ErrorMessage = "El stock mínimo debe ser un número positivo")]
        [Display(Name = "Stock Mínimo")]
        public int StockMinimo { get; set; }

        [Display(Name = "Activo")]
        public bool Estado { get; set; } = true;
    }
}
