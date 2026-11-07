namespace Proyecto_DAS.ViewModels.Administrador
{
    public class AdministradorViewModel
    {
        public int IdAdministrador { get; set; }
        public string Nombres { get; set; }
        public string Apellidos { get; set; }
        public string Email { get; set; }
        public string Celular { get; set; }
        public string Cargo { get; set; }
        public DateTime FechaContratacion { get; set; }
        public bool Estado { get; set; }
    }
}
