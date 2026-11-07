using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Proyecto_DAS.Models
{
    public class DetalleVenta
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int IdDetalleVenta { get; set; }

        [Required]
        public int Cantidad { get; set; }

        [Required]
        public decimal PrecioUnitario { get; set; }

        [Required]
        public decimal SubTotal { get; set; }

        // Talla y Color
        [StringLength(10)]
        public string? Talla { get; set; }

        [StringLength(50)]
        public string? Color { get; set; }

        // Conexion con Venta
        public int VentaId { get; set; }
        public Venta? Venta { get; set; }

        // Conexion con Producto
        public int ProductoId { get; set; }
        public Producto? Producto { get; set; }
    }
}
