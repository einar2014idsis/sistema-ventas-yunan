using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using Proyecto_DAS.Helpers;

namespace Proyecto_DAS.ViewModels.Producto
{
    public class ProductoEditViewModel : IValidatableObject
    {
        [Required]
        public int IdProducto { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio")]
        [StringLength(100, MinimumLength = 4, ErrorMessage = "El nombre debe tener entre 4 y 100 caracteres")]
        [Display(Name = "Nombre del Producto")]
        public string Nombre { get; set; }

        [Required(ErrorMessage = "La descripción es obligatoria")]
        [StringLength(255, MinimumLength = 10, ErrorMessage = "La descripción debe tener entre 10 y 255 caracteres")]
        [Display(Name = "Descripción")]
        public string Descripcion { get; set; }

        [Required(ErrorMessage = "El precio de compra es obligatorio")]
        [Range(1.01, 999999.99, ErrorMessage = "El precio de compra debe ser mayor a 1")]
        [Display(Name = "Precio de Compra")]
        public decimal PrecioCompra { get; set; }

        [Required(ErrorMessage = "El precio de venta es obligatorio")]
        [Range(1.01, 999999.99, ErrorMessage = "El precio de venta debe ser mayor a 1")]
        [Display(Name = "Precio de Venta")]
        public decimal PrecioVenta { get; set; }

        [Display(Name = "Imagen del Producto")]
        public IFormFile? ImagenArchivo { get; set; }

        public string? ImagenUrl { get; set; }

        [Display(Name = "Activo")]
        public bool Estado { get; set; }

        [Required(ErrorMessage = "Debe seleccionar una categoría")]
        [Display(Name = "Categoría")]
        public int CategoriaId { get; set; }

        [Required(ErrorMessage = "Debe seleccionar un proveedor")]
        [Display(Name = "Proveedor")]
        public int ProveedorId { get; set; }

        [Display(Name = "Fecha de Registro")]
        public DateTime FechaRegistro { get; set; }

        public IEnumerable<SelectListItem>? Categorias { get; set; }

        public IEnumerable<SelectListItem>? Proveedores { get; set; }

        [Display(Name = "Tallas")]
        public List<string>? TallasSeleccionadas { get; set; }

        [Display(Name = "Colores")]
        public List<string>? ColoresSeleccionados { get; set; }

        public List<ProductoVarianteItemViewModel>? Variantes { get; set; }

        public static List<string> TallasDisponibles => new List<string>
        {
            "26", "28", "30", "32", "34", "36", "38", "40"
        };

        public int StockTotal => Variantes?.Where(v => v.Estado).Sum(v => v.StockActual) ?? 0;

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (!string.IsNullOrWhiteSpace(Nombre))
            {
                Nombre = ValidationHelper.LimpiarTexto(Nombre);
                if (!ValidationHelper.EsLetrasNumerosYEspacios(Nombre))
                {
                    yield return new ValidationResult(
                        "El nombre del producto solo puede contener letras, números y espacios",
                        new[] { nameof(Nombre) }
                    );
                }
                else if (!ValidationHelper.ContieneAlMenosUnaLetra(Nombre))
                {
                    yield return new ValidationResult(
                        "El nombre debe contener al menos una letra",
                        new[] { nameof(Nombre) }
                    );
                }
            }

            if (!string.IsNullOrWhiteSpace(Descripcion))
            {
                Descripcion = ValidationHelper.LimpiarTexto(Descripcion);
            }

            if (!ValidationHelper.PrecioVentaMayorQueCompra(PrecioCompra, PrecioVenta))
            {
                yield return new ValidationResult(
                    "El precio de venta debe ser mayor al precio de compra para obtener ganancia",
                    new[] { nameof(PrecioVenta) }
                );
            }

            if (Variantes == null || !Variantes.Any())
            {
                yield return new ValidationResult(
                    "Debe tener al menos una variante (combinación de talla y color)",
                    new[] { nameof(Variantes) }
                );
            }
            else
            {
                if (Variantes.Count > 5)
                {
                    yield return new ValidationResult(
                        "No se pueden agregar más de 5 variantes por producto",
                        new[] { nameof(Variantes) }
                    );
                }

                for (int i = 0; i < Variantes.Count; i++)
                {
                    var variante = Variantes[i];

                    if (string.IsNullOrWhiteSpace(variante.Talla))
                    {
                        yield return new ValidationResult(
                            $"La variante {i + 1} debe tener una talla",
                            new[] { $"Variantes[{i}].Talla" }
                        );
                    }

                    if (string.IsNullOrWhiteSpace(variante.Color))
                    {
                        yield return new ValidationResult(
                            $"La variante {i + 1} debe tener un color",
                            new[] { $"Variantes[{i}].Color" }
                        );
                    }
                    else if (variante.Color.Length > 20)
                    {
                        yield return new ValidationResult(
                            $"El color '{variante.Color}' excede los 20 caracteres permitidos",
                            new[] { $"Variantes[{i}].Color" }
                        );
                    }

                    if (variante.StockActual < 0)
                    {
                        yield return new ValidationResult(
                            $"El stock actual de la variante {variante.Talla}-{variante.Color} no puede ser negativo",
                            new[] { $"Variantes[{i}].StockActual" }
                        );
                    }

                    if (variante.StockMinimo < 0)
                    {
                        yield return new ValidationResult(
                            $"El stock mínimo de la variante {variante.Talla}-{variante.Color} no puede ser negativo",
                            new[] { $"Variantes[{i}].StockMinimo" }
                        );
                    }

                    if (variante.StockMinimo >= variante.StockActual && variante.StockActual > 0)
                    {
                        yield return new ValidationResult(
                            $"El stock mínimo de {variante.Talla}-{variante.Color} debe ser menor al stock actual",
                            new[] { $"Variantes[{i}].StockMinimo" }
                        );
                    }
                }

                var duplicados = Variantes
                    .GroupBy(v => new { v.Talla, v.Color })
                    .Where(g => g.Count() > 1)
                    .Select(g => $"{g.Key.Talla}-{g.Key.Color}")
                    .ToList();

                if (duplicados.Any())
                {
                    yield return new ValidationResult(
                        $"Hay variantes duplicadas: {string.Join(", ", duplicados)}",
                        new[] { nameof(Variantes) }
                    );
                }
            }
        }
    }
}
