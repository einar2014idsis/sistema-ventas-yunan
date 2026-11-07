using System.ComponentModel.DataAnnotations;

namespace Proyecto_DAS.ViewModels.AlertaStock
{
    public class AlertaStockFilterViewModel
    {
        [Display(Name = "Estado")]
        public string? Estado { get; set; } // Pendiente, Resuelta o null para todos

        [Display(Name = "Producto")]
        public int? ProductoId { get; set; }

        [Display(Name = "Desde")]
        public DateTime? FechaDesde { get; set; }

        [Display(Name = "Hasta")]
        public DateTime? FechaHasta { get; set; }
    }
}
