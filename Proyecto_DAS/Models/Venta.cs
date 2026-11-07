using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Proyecto_DAS.Models
{
    public class Venta
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int IdVenta { get; set; }

        [Required]
        public DateTime FechaVenta { get; set; }

        [Required]
        public decimal SubtotalSinDescuento { get; set; }

        // IGV 18% calculado sobre el subtotal (después de descuento si aplica)
        [Required]
        public decimal IGV { get; set; }

        [Required]
        public decimal TotalVenta { get; set; }

        //(Efectivo, Tarjeta, Transferencia)
        [Required, StringLength(20)]
        public string MetodoPago { get; set; }

        //(Pendiente, Completada, Cancelada) 
        [Required, StringLength(20)]
        public string EstadoPago { get; set; }


        // Conexion con Cliente
        public int ClienteId { get; set; }
        public Cliente? Cliente { get; set; }

        // Conexion con CuponDescuento (opcional)
        public int? CuponDescuentoId { get; set; }
        public CuponDescuento? CuponDescuento { get; set; }

        // Conexion con DetalleVenta
        public ICollection<DetalleVenta> DetallesVenta { get; set; } = new List<DetalleVenta>();

        // Conexion con Envio
        public Envio? Envio { get; set; }
    }
}
