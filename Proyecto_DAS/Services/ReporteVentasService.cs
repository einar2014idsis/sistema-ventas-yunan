using OfficeOpenXml;
using OfficeOpenXml.Style;
using Proyecto_DAS.Models;
using System.Drawing;

namespace Proyecto_DAS.Services
{
    public class ReporteVentasService
    {

        public byte[] GenerarReporteExcel(List<Venta> ventas, DateTime? fechaInicio, DateTime? fechaFin, string? metodoPago, string? clienteNombre)
        {
            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Reporte de Ventas");

                worksheet.Cells["A1:J1"].Merge = true;
                worksheet.Cells["A1"].Value = "YUNAN - REPORTE DE VENTAS";
                worksheet.Cells["A1"].Style.Font.Size = 16;
                worksheet.Cells["A1"].Style.Font.Bold = true;
                worksheet.Cells["A1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells["A1"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells["A1"].Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml("#f1149b"));
                worksheet.Cells["A1"].Style.Font.Color.SetColor(Color.White);
                worksheet.Row(1).Height = 30;

                int currentRow = 3;

                worksheet.Cells[$"A{currentRow}"].Value = "Fecha de generación:";
                worksheet.Cells[$"A{currentRow}"].Style.Font.Bold = true;
                worksheet.Cells[$"B{currentRow}"].Value = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
                currentRow++;

                if (fechaInicio.HasValue || fechaFin.HasValue)
                {
                    worksheet.Cells[$"A{currentRow}"].Value = "Período:";
                    worksheet.Cells[$"A{currentRow}"].Style.Font.Bold = true;
                    string periodo = "";
                    if (fechaInicio.HasValue)
                        periodo += $"Desde {fechaInicio.Value:dd/MM/yyyy} ";
                    if (fechaFin.HasValue)
                        periodo += $"Hasta {fechaFin.Value:dd/MM/yyyy}";
                    worksheet.Cells[$"B{currentRow}"].Value = periodo.Trim();
                    currentRow++;
                }

                if (!string.IsNullOrWhiteSpace(metodoPago))
                {
                    worksheet.Cells[$"A{currentRow}"].Value = "Método de pago:";
                    worksheet.Cells[$"A{currentRow}"].Style.Font.Bold = true;
                    worksheet.Cells[$"B{currentRow}"].Value = metodoPago;
                    currentRow++;
                }

                if (!string.IsNullOrWhiteSpace(clienteNombre))
                {
                    worksheet.Cells[$"A{currentRow}"].Value = "Cliente:";
                    worksheet.Cells[$"A{currentRow}"].Style.Font.Bold = true;
                    worksheet.Cells[$"B{currentRow}"].Value = clienteNombre;
                    currentRow++;
                }

                worksheet.Cells[$"A{currentRow}"].Value = "Total de ventas:";
                worksheet.Cells[$"A{currentRow}"].Style.Font.Bold = true;
                worksheet.Cells[$"B{currentRow}"].Value = ventas.Count;
                worksheet.Cells[$"B{currentRow}"].Style.Font.Bold = true;
                worksheet.Cells[$"B{currentRow}"].Style.Font.Color.SetColor(ColorTranslator.FromHtml("#f1149b"));
                currentRow += 2;

                int headerRow = currentRow;
                worksheet.Cells[$"A{headerRow}"].Value = "ID Venta";
                worksheet.Cells[$"B{headerRow}"].Value = "Fecha";
                worksheet.Cells[$"C{headerRow}"].Value = "Cliente";
                worksheet.Cells[$"D{headerRow}"].Value = "Método Pago";
                worksheet.Cells[$"E{headerRow}"].Value = "Subtotal";
                worksheet.Cells[$"F{headerRow}"].Value = "IGV (18%)";
                worksheet.Cells[$"G{headerRow}"].Value = "Cupón";
                worksheet.Cells[$"H{headerRow}"].Value = "% Desc.";
                worksheet.Cells[$"I{headerRow}"].Value = "Descuento";
                worksheet.Cells[$"J{headerRow}"].Value = "Total";

                // Estilo de encabezados
                using (var range = worksheet.Cells[$"A{headerRow}:J{headerRow}"])
                {
                    range.Style.Font.Bold = true;
                    range.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    range.Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml("#f8cde7"));
                    range.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    range.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    range.Style.Border.BorderAround(ExcelBorderStyle.Medium);
                }

                currentRow = headerRow + 1;
                decimal totalGeneral = 0;

                foreach (var venta in ventas)
                {
                    worksheet.Cells[$"A{currentRow}"].Value = venta.IdVenta;
                    worksheet.Cells[$"B{currentRow}"].Value = venta.FechaVenta.ToString("dd/MM/yyyy HH:mm");
                    worksheet.Cells[$"C{currentRow}"].Value = venta.Cliente?.Usuario?.Nombres + " " + venta.Cliente?.Usuario?.Apellidos;
                    worksheet.Cells[$"D{currentRow}"].Value = venta.MetodoPago;
                    worksheet.Cells[$"E{currentRow}"].Value = venta.SubtotalSinDescuento;
                    worksheet.Cells[$"E{currentRow}"].Style.Numberformat.Format = "S/ #,##0.00";
                    worksheet.Cells[$"F{currentRow}"].Value = venta.IGV;
                    worksheet.Cells[$"F{currentRow}"].Style.Numberformat.Format = "S/ #,##0.00";
                    worksheet.Cells[$"G{currentRow}"].Value = venta.CuponDescuento?.CodigoCupon ?? "-";
                    var porcentajeDescuento = venta.CuponDescuento?.PorcentajeDescuento ?? 0;
                    worksheet.Cells[$"H{currentRow}"].Value = porcentajeDescuento;
                    worksheet.Cells[$"H{currentRow}"].Style.Numberformat.Format = "0\"%\"";
                    // Calcular monto del descuento: Subtotal × (Porcentaje / 100)
                    var montoDescuento = venta.SubtotalSinDescuento * porcentajeDescuento / 100;
                    worksheet.Cells[$"I{currentRow}"].Value = montoDescuento;
                    worksheet.Cells[$"I{currentRow}"].Style.Numberformat.Format = "S/ #,##0.00";
                    worksheet.Cells[$"J{currentRow}"].Value = venta.TotalVenta;
                    worksheet.Cells[$"J{currentRow}"].Style.Numberformat.Format = "S/ #,##0.00";

                    if (currentRow % 2 == 0)
                    {
                        worksheet.Cells[$"A{currentRow}:J{currentRow}"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        worksheet.Cells[$"A{currentRow}:J{currentRow}"].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(245, 245, 245));
                    }

                    totalGeneral += venta.TotalVenta;
                    currentRow++;
                }

                currentRow++;
                worksheet.Cells[$"A{currentRow}:I{currentRow}"].Merge = true;
                worksheet.Cells[$"A{currentRow}"].Value = "TOTAL GENERAL:";
                worksheet.Cells[$"A{currentRow}"].Style.Font.Bold = true;
                worksheet.Cells[$"A{currentRow}"].Style.Font.Size = 12;
                worksheet.Cells[$"A{currentRow}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                worksheet.Cells[$"A{currentRow}"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells[$"A{currentRow}"].Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml("#f8cde7"));

                worksheet.Cells[$"J{currentRow}"].Value = totalGeneral;
                worksheet.Cells[$"J{currentRow}"].Style.Numberformat.Format = "S/ #,##0.00";
                worksheet.Cells[$"J{currentRow}"].Style.Font.Bold = true;
                worksheet.Cells[$"J{currentRow}"].Style.Font.Size = 12;
                worksheet.Cells[$"J{currentRow}"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells[$"J{currentRow}"].Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml("#f1149b"));
                worksheet.Cells[$"J{currentRow}"].Style.Font.Color.SetColor(Color.White);

                using (var range = worksheet.Cells[$"A{headerRow}:J{currentRow}"])
                {
                    range.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    range.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    range.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    range.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                }

                worksheet.Column(1).Width = 12;  // ID
                worksheet.Column(2).Width = 18;  // Fecha
                worksheet.Column(3).Width = 25;  // Cliente
                worksheet.Column(4).Width = 15;  // Método Pago
                worksheet.Column(5).Width = 15;  // Subtotal
                worksheet.Column(6).Width = 14;  // IGV
                worksheet.Column(7).Width = 15;  // Cupón
                worksheet.Column(8).Width = 10;  // % Descuento
                worksheet.Column(9).Width = 14;  // Descuento (monto)
                worksheet.Column(10).Width = 15; // Total

                currentRow += 2;
                worksheet.Cells[$"A{currentRow}:J{currentRow}"].Merge = true;
                worksheet.Cells[$"A{currentRow}"].Value = "Generado por Sistema YUNAN - \"Brilla Bonito\"";
                worksheet.Cells[$"A{currentRow}"].Style.Font.Italic = true;
                worksheet.Cells[$"A{currentRow}"].Style.Font.Size = 9;
                worksheet.Cells[$"A{currentRow}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells[$"A{currentRow}"].Style.Font.Color.SetColor(Color.Gray);

                return package.GetAsByteArray();
            }
        }
    }
}
