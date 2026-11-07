namespace Proyecto_DAS.ViewModels.ClientePanel
{
    public class MisComprasViewModel
    {
        public int IdVenta { get; set; }
        public DateTime FechaVenta { get; set; }
        public decimal Total { get; set; }
        public string MetodoPago { get; set; }
        public int CantidadProductos { get; set; }

        // Datos del envío
        public int? IdEnvio { get; set; }
        public string EstadoEnvio { get; set; }
        public string NumeroSeguimiento { get; set; }
    }
}
