using System;

namespace Proyecto_DAS.ViewModels.Envio
{
    public class EnvioViewModel
    {
        public int IdEnvio { get; set; }
        public int VentaId { get; set; }
        public string ClienteNombre { get; set; }
        public string DireccionEnvio { get; set; }
        public string Ciudad { get; set; }
        public string CodigoPostal { get; set; }
        public DateTime FechaEnvio { get; set; }
        public DateTime FechaEntregaEstimada { get; set; }
        public DateTime? FechaEntregaReal { get; set; }
        public string EstadoEnvio { get; set; }
        public string EmpresaTransporte { get; set; }
        public string NumeroSeguimiento { get; set; }
        public int? AdministradorId { get; set; }
    }
}
