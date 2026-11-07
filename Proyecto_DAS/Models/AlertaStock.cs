using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Proyecto_DAS.Models
{
    public class AlertaStock
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int IdAlertaStock { get; set; }

        [Required]
        public DateTime FechaAlerta { get; set; }

        [Required, StringLength(20)]
        public string Estado { get; set; } = "Pendiente"; // Pendiente, Resuelta

        [Required, StringLength(255)]
        public string Descripcion { get; set; }

        public DateTime? FechaResolucion { get; set; }

        // Conexion con ProductoVariante (en lugar de Producto)
        [Required]
        public int ProductoVarianteId { get; set; }
        public ProductoVariante? ProductoVariante { get; set; }
    }
}
