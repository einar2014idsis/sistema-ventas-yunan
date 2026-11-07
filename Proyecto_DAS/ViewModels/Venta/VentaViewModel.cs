namespace Proyecto_DAS.ViewModels.Venta
{
    public class VentaViewModel
    {
        public int IdVenta { get; set; }
        public string ClienteNombre { get; set; }
        public int ClienteId { get; set; }
        public DateTime FechaVenta { get; set; }
        public decimal SubtotalSinDescuento { get; set; }
        public decimal TotalVenta { get; set; }
        public string MetodoPago { get; set; }

        // Cupón de descuento (opcional)
        public string? CodigoCupon { get; set; }
        public decimal PorcentajeDescuento { get; set; }

        // Información de envío
        public string EstadoEnvio { get; set; }
        public string NumeroSeguimiento { get; set; }

        // Total de productos
        public int CantidadProductos { get; set; }
    }
}
