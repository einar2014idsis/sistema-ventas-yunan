using System.ComponentModel.DataAnnotations;

namespace Proyecto_DAS.ViewModels.Proveedor
{
    public class ProveedorViewModel
    {
        public int IdProveedor { get; set; }

        [Display(Name = "Nombre de Empresa")]
        public string NombreEmpresa { get; set; }

        [Display(Name = "Contacto")]
        public string ContactoNombre { get; set; }

        [Display(Name = "Celular")]
        public string Celular { get; set; }

        [Display(Name = "Email")]
        public string Email { get; set; }

        [Display(Name = "Dirección")]
        public string Direccion { get; set; }

        [Display(Name = "RUC")]
        public string Ruc { get; set; }

        [Display(Name = "Estado")]
        public bool Estado { get; set; }
    }
}
