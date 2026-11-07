using System.Text.Json;
using Proyecto_DAS.ViewModels.Carrito;

namespace Proyecto_DAS.Services
{
    public class CarritoService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private const string CarritoSessionKey = "Carrito";

        public CarritoService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public List<CarritoItemViewModel> ObtenerCarrito()
        {
            var session = _httpContextAccessor.HttpContext?.Session;
            if (session == null)
            {
                return new List<CarritoItemViewModel>();
            }

            var carritoJson = session.GetString(CarritoSessionKey);
            if (string.IsNullOrEmpty(carritoJson))
            {
                return new List<CarritoItemViewModel>();
            }
            return JsonSerializer.Deserialize<List<CarritoItemViewModel>>(carritoJson) ?? new List<CarritoItemViewModel>();
        }

        public void GuardarCarrito(List<CarritoItemViewModel> carrito)
        {
            var session = _httpContextAccessor.HttpContext?.Session;
            if (session == null)
            {
                return;
            }

            var carritoJson = JsonSerializer.Serialize(carrito);
            session.SetString(CarritoSessionKey, carritoJson);
        }

        public void LimpiarCarrito()
        {
            var session = _httpContextAccessor.HttpContext?.Session;
            session?.Remove(CarritoSessionKey);
        }

        public int ObtenerCantidadItems()
        {
            return ObtenerCarrito().Sum(c => c.Cantidad);
        }
    }
}
