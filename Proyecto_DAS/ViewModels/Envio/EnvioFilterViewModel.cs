using System;

namespace Proyecto_DAS.ViewModels.Envio
{
    public class EnvioFilterViewModel
    {
        public string? Estado { get; set; }
        public string? Ciudad { get; set; }
        public string? Empresa { get; set; }
        public string? Tracking { get; set; }
        public int? VentaId { get; set; }

        public DateTime? Desde { get; set; }
        public DateTime? Hasta { get; set; }
    }
}
