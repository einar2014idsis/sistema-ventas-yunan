using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Proyecto_DAS.Models
{
    public class Categoria
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int IdCategoria { get; set; }

        [Required, StringLength(50)]
        public string Nombre { get; set; }

        [Required, StringLength(500)]
        public string Descripcion { get; set; }

        [Required]
        public bool Estado { get; set; } = true;


        // Conexion con Producto
        public ICollection<Producto>? Productos { get; set; } 
    }
}
