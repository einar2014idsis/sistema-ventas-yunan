using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Proyecto_DAS.Models
{
    public class Administrador
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int IdAdministrador { get; set; }

        [Required, StringLength(100)]
        public string Cargo { get; set; }

        [Required]
        public DateTime FechaContratacion { get; set; }


        // Conexion con Usuario (relación 1:1)
        public int UsuarioId { get; set; }
        public Usuario? Usuario { get; set; }

        // Conexion con Envio (administrador actualiza estados)
        public ICollection<Envio>? Envios { get; set; }
    }
}
