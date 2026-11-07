using System.ComponentModel.DataAnnotations;

namespace Proyecto_DAS.ViewModels.CuponDescuento
{
    public class CuponDescuentoViewModel
    {
        public int IdCuponDescuento { get; set; }

        [Display(Name = "Código Cupón")]
        public string CodigoCupon { get; set; }

        [Display(Name = "Porcentaje de Descuento (%)")]
        public decimal PorcentajeDescuento { get; set; }

        [Display(Name = "Fecha Inicio")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime FechaInicio { get; set; }

        [Display(Name = "Fecha Vencimiento")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime FechaVencimiento { get; set; }

        [Display(Name = "Estado")]
        public bool Estado { get; set; }

        [Display(Name = "Fecha Creación")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy HH:mm}", ApplyFormatInEditMode = true)]
        public DateTime FechaCreacion { get; set; }

        [Display(Name = "Estado Actual")]
        public string EstadoTexto
        {
            get
            {
                if (!Estado) return "Inactivo";
                if (DateTime.Now < FechaInicio) return "Programado";
                if (DateTime.Now > FechaVencimiento) return "Vencido";
                return "Activo";
            }
        }

        [Display(Name = "Días Restantes")]
        public int DiasRestantes
        {
            get
            {
                if (DateTime.Now > FechaVencimiento) return 0;
                return (FechaVencimiento - DateTime.Now).Days;
            }
        }
    }
}
