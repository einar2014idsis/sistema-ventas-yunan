using System.ComponentModel.DataAnnotations;

namespace Proyecto_DAS.ViewModels.Categoria
{
    public class CategoriaViewModel
    {
        public int IdCategoria { get; set; }

        [Display(Name = "Nombre")]
        public string Nombre { get; set; }

        [Display(Name = "Descripción")]
        public string Descripcion { get; set; }

        [Display(Name = "Estado")]
        public bool Estado { get; set; }

        [Display(Name = "Cantidad de Productos")]
        public int CantidadProductos { get; set; }
    }
}
