using Microsoft.AspNetCore.Mvc.Rendering;

namespace Proyecto_DAS.ViewModels.ClientePanel
{
    public class CatalogoFilterViewModel
    {
        // Filtros seleccionados
        public int? CategoriaId { get; set; }
        public string Talla { get; set; }
        public string Color { get; set; }
        public decimal? PrecioMin { get; set; }
        public decimal? PrecioMax { get; set; }
        public string Busqueda { get; set; }

        // Paginación
        public int PaginaActual { get; set; } = 1;
        public int ProductosPorPagina { get; set; } = 12;
        public int TotalProductos { get; set; }
        public int TotalPaginas => (int)Math.Ceiling((double)TotalProductos / ProductosPorPagina);
        public bool TienePaginaAnterior => PaginaActual > 1;
        public bool TienePaginaSiguiente => PaginaActual < TotalPaginas;

        // Listas para los dropdowns
        public List<SelectListItem> Categorias { get; set; } = new List<SelectListItem>();
        public List<SelectListItem> Tallas { get; set; } = new List<SelectListItem>();
        public List<SelectListItem> Colores { get; set; } = new List<SelectListItem>();

        // Productos filtrados (solo los de la página actual)
        public List<Proyecto_DAS.Models.Producto> Productos { get; set; } = new List<Proyecto_DAS.Models.Producto>();
    }
}
