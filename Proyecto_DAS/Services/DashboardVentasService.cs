using Microsoft.EntityFrameworkCore;
using Proyecto_DAS.Data;

namespace Proyecto_DAS.Services;

// Los importes de los detalles se prorratean según el total final de cada venta.
// Así, descuentos y redondeos no multiplican el ingreso cuando hay varios productos.
public class DashboardVentasService(AppDbContext db)
{
    public async Task<object> ObtenerAsync(int? clienteId, string? metodoPago)
    {
        var consulta = db.Ventas.AsNoTracking().AsQueryable();
        if (clienteId is > 0) consulta = consulta.Where(v => v.ClienteId == clienteId);
        if (!string.IsNullOrWhiteSpace(metodoPago)) consulta = consulta.Where(v => v.MetodoPago == metodoPago);
        var ventas = await consulta.Select(v => new {
            id = v.IdVenta, fecha = v.FechaVenta, clienteId = v.ClienteId,
            metodo = v.MetodoPago, estado = v.EstadoPago, total = v.TotalVenta,
            detalles = v.DetallesVenta.Select(d => new {
                productoId = d.ProductoId, nombre = d.Producto!.Nombre,
                categoriaId = d.Producto.CategoriaId, categoria = d.Producto.Categoria!.Nombre,
                unidades = d.Cantidad, baseImporte = d.PrecioUnitario * d.Cantidad
            }).ToList()
        }).ToListAsync();
        var productos = await db.Productos.AsNoTracking().Where(p => p.Estado).Select(p => new {
            id = p.IdProducto, nombre = p.Nombre, categoriaId = p.CategoriaId,
            categoria = p.Categoria!.Nombre,
            stock = p.Variantes!.Where(v => v.Estado).Sum(v => (int?)v.StockActual) ?? 0,
            critico = p.Variantes!.Any(v => v.Estado && v.StockActual <= v.StockMinimo)
        }).ToListAsync();
        TimeZoneInfo zona;
        try { zona = TimeZoneInfo.FindSystemTimeZoneById("America/Lima"); }
        catch (TimeZoneNotFoundException) { zona = TimeZoneInfo.FindSystemTimeZoneById("SA Pacific Standard Time"); }
        return new { ventas, productos, hoy = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, zona).ToString("yyyy-MM-dd"), zona = "America/Lima" };
    }
}
