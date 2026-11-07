using System.ComponentModel.DataAnnotations;

namespace Proyecto_DAS.ViewModels.AlertaStock
{
    public class AlertaStockViewModel
    {
        public int IdAlerta { get; set; }

        public int ProductoVarianteId { get; set; }

        [Display(Name = "Producto")]
        public string ProductoNombre { get; set; }

        [Display(Name = "Talla")]
        public string Talla { get; set; }

        [Display(Name = "Color")]
        public string Color { get; set; }

        [Display(Name = "Stock Actual")]
        public int StockActual { get; set; }

        [Display(Name = "Stock Mínimo")]
        public int StockMinimo { get; set; }

        [Display(Name = "Fecha de Alerta")]
        public DateTime FechaAlerta { get; set; }

        [Display(Name = "Fecha de Resolución")]
        public DateTime? FechaResolucion { get; set; }

        [Display(Name = "Estado")]
        public string Estado { get; set; } // Pendiente, Resuelta

        [Display(Name = "Descripción")]
        public string Descripcion { get; set; }
    }
}
