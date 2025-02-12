using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.Data;
using System.Drawing;

namespace Libraries.Services
{
    public class ExportExcelService : IExportExcelService
    {
        public byte[] Export(DataTable dataTable)
        {
            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Sheet1");

                // Add the header row
                for (int col = 1; col <= dataTable.Columns.Count; col++)
                {
                    worksheet.Cells[1, col].Value = dataTable.Columns[col - 1].ColumnName;
                }

                worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

                // Apply borders to all cells
                var allCells = worksheet.Cells[worksheet.Dimension.Address];
                allCells.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                allCells.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                allCells.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                allCells.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                allCells.Style.Font.Color.SetColor(Color.White);

                // Set column color to blue (example for all columns)
                worksheet.Cells[worksheet.Dimension.Address].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells[worksheet.Dimension.Address].Style.Fill.BackgroundColor.SetColor(Color.Blue);

                // Add the data rows
                for (int row = 0; row < dataTable.Rows.Count; row++)
                {
                    for (int col = 0; col < dataTable.Columns.Count; col++)
                    {
                        worksheet.Cells[row + 2, col + 1].Value = dataTable.Rows[row][col];
                        worksheet.Cells[row + 2, col + 1].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                        worksheet.Cells[row + 2, col + 1].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                        worksheet.Cells[row + 2, col + 1].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                        worksheet.Cells[row + 2, col + 1].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                        worksheet.Cells[row + 2, col + 1].AutoFitColumns();
                    }
                }

                return package.GetAsByteArray();
            }
        }
    }
}
