namespace Proyecto_DAS.ViewModels.Carrito
{
    public class CarritoItemViewModel
    {
        public int ProductoId { get; set; }
        public int ProductoVarianteId { get; set; } 
        public string NombreProducto { get; set; }
        public string ImagenUrl { get; set; }
        public string Categoria { get; set; }
        public string Color { get; set; }
        public string Talla { get; set; }
        public decimal PrecioUnitario { get; set; }
        public int Cantidad { get; set; }
        public int StockDisponible { get; set; }
        public decimal Subtotal => PrecioUnitario * Cantidad;
    }
}
