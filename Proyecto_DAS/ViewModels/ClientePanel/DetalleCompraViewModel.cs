namespace Proyecto_DAS.ViewModels.ClientePanel
{
    public class DetalleCompraViewModel
    {
        // Datos de la venta
        public int IdVenta { get; set; }
        public DateTime FechaVenta { get; set; }
        public decimal SubtotalSinDescuento { get; set; } 
        public decimal IGV { get; set; }
        public decimal Total { get; set; }
        public string MetodoPago { get; set; }

        // Cupón aplicado
        public string CodigoCupon { get; set; }
        public decimal? PorcentajeDescuento { get; set; }

        // Propiedades calculadas
        public decimal Descuento => PorcentajeDescuento.HasValue ? SubtotalSinDescuento * PorcentajeDescuento.Value / 100 : 0;
        public decimal BaseImponible => Total - IGV;

        // Productos
        public List<ProductoCompraViewModel> Productos { get; set; } = new List<ProductoCompraViewModel>();

        // Datos del envío
        public EnvioDetalleViewModel Envio { get; set; }
    }

    public class ProductoCompraViewModel
    {
        public string NombreProducto { get; set; }
        public string ImagenUrl { get; set; }
        public string Color { get; set; }
        public string Talla { get; set; }
        public int Cantidad { get; set; }
        public decimal PrecioUnitario { get; set; }
        public decimal Subtotal { get; set; }
    }

    public class EnvioDetalleViewModel
    {
        public int IdEnvio { get; set; }
        public string DireccionEnvio { get; set; }
        public string Ciudad { get; set; }
        public string CodigoPostal { get; set; }
        public DateTime FechaEnvio { get; set; }
        public DateTime FechaEntregaEstimada { get; set; }
        public DateTime? FechaEntregaReal { get; set; }
        public string EstadoEnvio { get; set; }
        public string EmpresaTransporte { get; set; }
        public string NumeroSeguimiento { get; set; }
    }
}
