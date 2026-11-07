using System.ComponentModel.DataAnnotations;

namespace Proyecto_DAS.ViewModels.Producto
{
    public class ProductoViewModel
    {
        public int IdProducto { get; set; }

        [Display(Name = "Nombre")]
        public string Nombre { get; set; }

        [Display(Name = "Descripcion")]
        public string Descripcion { get; set; }

        [Display(Name = "Precio Compra")]
        [DisplayFormat(DataFormatString = "S/ {0:N2}", ApplyFormatInEditMode = false)]
        public decimal PrecioCompra { get; set; }

        [Display(Name = "Precio Venta")]
        [DisplayFormat(DataFormatString = "S/ {0:N2}", ApplyFormatInEditMode = false)]
        public decimal PrecioVenta { get; set; }

        [Display(Name = "Stock Total")]
        public int StockActual { get; set; }

        [Display(Name = "Imagen")]
        public string ImagenUrl { get; set; }

        [Display(Name = "Tallas")]
        public string Talla { get; set; }

        [Display(Name = "Colores")]
        public string Color { get; set; }

        [Display(Name = "Estado")]
        public bool Estado { get; set; }

        [Display(Name = "Fecha Registro")]
        public DateTime FechaRegistro { get; set; }

        [Display(Name = "Categoria")]
        public string CategoriaNombre { get; set; }

        public int CategoriaId { get; set; }

        [Display(Name = "Proveedor")]
        public string ProveedorNombre { get; set; }

        public int ProveedorId { get; set; }

        // Indica si alguna variante tiene stock bajo
        public bool TieneStockBajo { get; set; }

        // Cantidad de variantes con stock bajo
        public int VariantesConStockBajo { get; set; }

        // Propiedades calculadas simples
        public decimal Margen => PrecioVenta - PrecioCompra;
    }
}
