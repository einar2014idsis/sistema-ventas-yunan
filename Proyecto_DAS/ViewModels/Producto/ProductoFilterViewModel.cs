using System.ComponentModel.DataAnnotations;

namespace Proyecto_DAS.ViewModels.Producto
{
    public class ProductoFilterViewModel
    {
        [Display(Name = "Categoría")]
        public int? CategoriaId { get; set; }

        [Display(Name = "Estado")]
        public bool? Estado { get; set; }

        [Display(Name = "Stock Bajo")]
        public bool? StockBajo { get; set; }

        [Display(Name = "Nombre Producto")]
        public string? NombreProducto { get; set; }
    }
}
