using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using Proyecto_DAS.Models;

namespace Proyecto_DAS.Services
{
    public class BoletaService
    {
        // Configurar licencia de QuestPDF
        static BoletaService()
        {
            QuestPDF.Settings.License = LicenseType.Community;
        }

        public byte[] GenerarBoletaPDF(Venta venta, Cliente cliente, Usuario usuario)
        {
            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(40);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(10).FontFamily("Arial"));

                    page.Header().Element(content => ComposeHeader(content, venta));
                    page.Content().Element(content => ComposeContent(content, venta, cliente, usuario));
                    page.Footer().Element(ComposeFooter);
                });
            });

            return document.GeneratePdf();
        }

        void ComposeHeader(IContainer container, Venta venta)
        {
            container.Column(column =>
            {
                column.Item().PaddingBottom(10).Row(row =>
                {
                    row.RelativeItem().Column(col =>
                    {
                        col.Item().Text("🏪 YUNAN").FontSize(24).Bold().FontColor(Colors.Pink.Darken2);
                        col.Item().Text("Tienda de Ropa").FontSize(12).FontColor(Colors.Grey.Darken1);
                        col.Item().Text("RUC: 20123456789").FontSize(9).FontColor(Colors.Grey.Darken1);
                        col.Item().Text("Av. Larco 123, Cajamarca - Perú").FontSize(9).FontColor(Colors.Grey.Darken1);
                        col.Item().Text("Telf: 914 412 984").FontSize(9).FontColor(Colors.Grey.Darken1);
                    });

                    row.ConstantItem(120).AlignRight().Column(col =>
                    {
                        col.Item().Border(1).BorderColor(Colors.Pink.Darken2).Padding(5).Column(innerCol =>
                        {
                            innerCol.Item().Text("BOLETA DE VENTA").FontSize(11).Bold().FontColor(Colors.Pink.Darken2);
                            innerCol.Item().PaddingTop(3).Text($"N° BOL-2025-{venta.IdVenta:D4}").FontSize(9).Bold();
                        });
                    });
                });

                column.Item().PaddingTop(5).LineHorizontal(1).LineColor(Colors.Grey.Lighten1);
            });
        }

        void ComposeContent(IContainer container, Venta venta, Cliente cliente, Usuario usuario)
        {
            container.PaddingVertical(10).Column(column =>
            {
                // Información de la boleta
                column.Item().Row(row =>
                {
                    row.RelativeItem().Text($"Fecha: {venta.FechaVenta:dd/MM/yyyy}").FontSize(9);
                    row.RelativeItem().AlignRight().Text($"Hora: {venta.FechaVenta:HH:mm}").FontSize(9);
                });

                column.Item().PaddingTop(10).LineHorizontal(1).LineColor(Colors.Grey.Lighten2);

                // Datos del cliente
                column.Item().PaddingTop(10).Column(clienteCol =>
                {
                    clienteCol.Item().Text("DATOS DEL CLIENTE").FontSize(11).Bold().FontColor(Colors.Pink.Darken2);
                    clienteCol.Item().PaddingTop(5).Text($"Nombre: {usuario.Nombres} {usuario.Apellidos}").FontSize(9);
                    clienteCol.Item().Text($"DNI/RUC: {cliente.DniRuc}").FontSize(9);
                    clienteCol.Item().Text($"Email: {usuario.Email}").FontSize(9);
                    clienteCol.Item().Text($"Celular: {usuario.Celular}").FontSize(9);
                });

                column.Item().PaddingTop(10).LineHorizontal(1).LineColor(Colors.Grey.Lighten2);

                // Detalle de productos
                column.Item().PaddingTop(10).Column(detalleCol =>
                {
                    detalleCol.Item().Text("DETALLE DE COMPRA").FontSize(11).Bold().FontColor(Colors.Pink.Darken2);

                    detalleCol.Item().PaddingTop(5).Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.RelativeColumn(3); // Producto
                            columns.ConstantColumn(50); // Cantidad
                            columns.ConstantColumn(70); // Precio
                            columns.ConstantColumn(70); // Subtotal
                        });

                        // Encabezado
                        table.Header(header =>
                        {
                            header.Cell().Background(Colors.Pink.Lighten3).Padding(5).Text("Producto").FontSize(9).Bold();
                            header.Cell().Background(Colors.Pink.Lighten3).Padding(5).AlignCenter().Text("Cant.").FontSize(9).Bold();
                            header.Cell().Background(Colors.Pink.Lighten3).Padding(5).AlignRight().Text("Precio").FontSize(9).Bold();
                            header.Cell().Background(Colors.Pink.Lighten3).Padding(5).AlignRight().Text("Subtotal").FontSize(9).Bold();
                        });

                        // Productos
                        foreach (var detalle in venta.DetallesVenta)
                        {
                            table.Cell().BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten2).Padding(5).Text(detalle.Producto?.Nombre ?? "Producto").FontSize(9);
                            table.Cell().BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten2).Padding(5).AlignCenter().Text(detalle.Cantidad.ToString()).FontSize(9);
                            table.Cell().BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten2).Padding(5).AlignRight().Text($"S/ {detalle.PrecioUnitario:F2}").FontSize(9);
                            table.Cell().BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten2).Padding(5).AlignRight().Text($"S/ {detalle.SubTotal:F2}").FontSize(9);
                        }
                    });
                });

                // Totales
                column.Item().PaddingTop(15).AlignRight().Column(totalCol =>
                {
                    // Base imponible (total sin IGV)
                    var baseImponible = venta.TotalVenta - venta.IGV;

                    if (venta.CuponDescuento != null)
                    {

                        // Subtotal original (con IGV incluido)
                        totalCol.Item().Row(row =>
                        {
                            row.ConstantItem(140).Text("SUBTOTAL:").FontSize(9).Bold();
                            row.ConstantItem(80).AlignRight().Text($"S/ {venta.SubtotalSinDescuento:F2}").FontSize(9);
                        });

                        // Descuento aplicado
                        var descuentoMonto = venta.SubtotalSinDescuento * venta.CuponDescuento.PorcentajeDescuento / 100;
                        totalCol.Item().Row(row =>
                        {
                            row.ConstantItem(140).Text($"DESCUENTO ({venta.CuponDescuento.PorcentajeDescuento}%):").FontSize(9).FontColor(Colors.Green.Darken2);
                            row.ConstantItem(80).AlignRight().Text($"- S/ {descuentoMonto:F2}").FontSize(9).FontColor(Colors.Green.Darken2);
                        });

                        // Base imponible (después de descuento, sin IGV)
                        totalCol.Item().Row(row =>
                        {
                            row.ConstantItem(140).Text("BASE IMPONIBLE:").FontSize(9).Bold();
                            row.ConstantItem(80).AlignRight().Text($"S/ {baseImponible:F2}").FontSize(9);
                        });
                    }
                    else
                    {
                        totalCol.Item().Row(row =>
                        {
                            row.ConstantItem(140).Text("BASE IMPONIBLE:").FontSize(9).Bold();
                            row.ConstantItem(80).AlignRight().Text($"S/ {baseImponible:F2}").FontSize(9);
                        });
                    }

                    // IGV (18%)
                    totalCol.Item().Row(row =>
                    {
                        row.ConstantItem(140).Text("IGV (18%):").FontSize(9).Bold();
                        row.ConstantItem(80).AlignRight().Text($"S/ {venta.IGV:F2}").FontSize(9);
                    });

                    // Total
                    totalCol.Item().PaddingTop(5).Row(row =>
                    {
                        row.ConstantItem(140).Text("TOTAL:").FontSize(12).Bold().FontColor(Colors.Pink.Darken2);
                        row.ConstantItem(80).AlignRight().Text($"S/ {venta.TotalVenta:F2}").FontSize(12).Bold().FontColor(Colors.Pink.Darken2);
                    });
                });

                column.Item().PaddingTop(10).LineHorizontal(1).LineColor(Colors.Grey.Lighten2);

                // Método de pago
                column.Item().PaddingTop(10).Row(row =>
                {
                    row.RelativeItem().Text($"MÉTODO DE PAGO: {venta.MetodoPago}").FontSize(9).Bold();
                });

                // Información de envío
                if (venta.Envio != null)
                {
                    column.Item().PaddingTop(10).LineHorizontal(1).LineColor(Colors.Grey.Lighten2);

                    column.Item().PaddingTop(10).Column(envioCol =>
                    {
                        envioCol.Item().Text("INFORMACIÓN DE ENVÍO").FontSize(11).Bold().FontColor(Colors.Pink.Darken2);
                        envioCol.Item().PaddingTop(5).Text($"Dirección: {venta.Envio.DireccionEnvio}").FontSize(9);
                        envioCol.Item().Text($"Ciudad: {venta.Envio.Ciudad}").FontSize(9);
                        envioCol.Item().Text($"Código Postal: {venta.Envio.CodigoPostal}").FontSize(9);
                        envioCol.Item().Text($"N° Seguimiento: {venta.Envio.NumeroSeguimiento}").FontSize(9).Bold();
                        envioCol.Item().Text($"Empresa: {venta.Envio.EmpresaTransporte}").FontSize(9);
                    });
                }
            });
        }

        void ComposeFooter(IContainer container)
        {
            container.AlignCenter().Column(column =>
            {
                column.Item().LineHorizontal(1).LineColor(Colors.Grey.Lighten2);
                column.Item().PaddingTop(10).Text("IMPORTANTE: NO SE ACEPTAN DEVOLUCIONES").FontSize(9).Bold().FontColor(Colors.Red.Darken1);
                column.Item().PaddingTop(5).Text("¡Gracias por tu compra!").FontSize(10).Bold().FontColor(Colors.Pink.Darken2);
                column.Item().Text("Síguenos en nuestras redes sociales").FontSize(8).FontColor(Colors.Grey.Darken1);
                column.Item().Text("www.yunan.com").FontSize(8).FontColor(Colors.Grey.Darken1);
            });
        }
    }
}
