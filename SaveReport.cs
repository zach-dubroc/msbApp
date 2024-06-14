
using iText.Kernel.Colors;
using iText.Kernel.Font;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
using Microsoft.Win32;
using System.IO;
using System.Windows.Controls;
using static MSBeverageRecordApp.Reports;
using Paragraph = iText.Layout.Element.Paragraph;
using Table = iText.Layout.Element.Table;


namespace MSBeverageRecordApp {
    internal class SaveReport {
        public void ExportToPdf(RootObject obj, DataGrid dataGrid, string title, string filter) {
            string filePath = GetSaveFilePath("PDF Files (*.pdf)|*.pdf|All files (*.*)|*.*");
            if (string.IsNullOrEmpty(filePath)) return;

            using (var pdfWriter = new PdfWriter(filePath))
            using (var pdfDocument = new PdfDocument(pdfWriter))
            using (var document = new Document(pdfDocument)) {


                PdfFont regularFont = PdfFontFactory.CreateFont();
                PdfFont boldFont = PdfFontFactory.CreateFont();

                Paragraph titleParagraph = new Paragraph(title)
                    .SetTextAlignment(TextAlignment.CENTER)
                    .SetFont(boldFont)
                    .SetFontSize(20)
                    .SetFontColor(DeviceGray.BLACK);
                document.Add(titleParagraph);

                var table = new Table(dataGrid.Columns.Count);

                foreach (var column in dataGrid.Columns) {
                    Cell headerCell = new Cell().Add(new Paragraph((column as DataGridColumn).Header.ToString())
                        .SetFont(boldFont)
                        .SetFontSize(12))
                        .SetBackgroundColor(new DeviceRgb(192, 192, 192))
                        .SetTextAlignment(TextAlignment.CENTER)
                        .SetBorder(iText.Layout.Borders.Border.NO_BORDER);

                    headerCell.SetBorder(new iText.Layout.Borders.SolidBorder(new DeviceRgb(0, 0, 0), 1f));

                    table.AddHeaderCell(headerCell);

                }

                AddDataRows(obj, table, regularFont, filter);

                decimal totalCost = CalculateTotalCost(obj, filter);

                AddTotalCostRow(table, regularFont, totalCost);

                document.Add(table);
            }
        }

        public void ExportToCsv(RootObject obj, DataGrid dataGrid, string title, string filter) {
            string filePath = GetSaveFilePath("CSV Files (*.csv)|*.csv|All files (*.*)|*.*");

            if (string.IsNullOrEmpty(filePath)) return;

            using (StreamWriter sw = new StreamWriter(filePath)) {
                sw.WriteLine(title);

                foreach (var column in dataGrid.Columns) {
                    sw.Write((column as DataGridColumn).Header.ToString() + ",");
                }
                sw.WriteLine();

                WriteDataRows(obj, sw, filter);

                decimal totalCost = CalculateTotalCost(obj, filter);
                WriteTotalCostRow(sw, obj, filter);


            }
        }

        private string GetSaveFilePath(string filter) {
            string filePath = "";
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = filter;
            if (saveFileDialog.ShowDialog() == true) {
                filePath = saveFileDialog.FileName;
            }
            return filePath;
        }

        private void AddDataRows(RootObject obj, Table table, PdfFont font, string title) {


            switch (title) {
                case "allData":
                    foreach (var item in obj.Items) {
                        if (item.sub_location == null) {
                            item.sub_location = "";
                        }
                        if (item.is_deleted == 0) {

                            table.AddCell(CreateCell(item.record_id.ToString(), font));
                            table.AddCell(CreateCell(item.categoryName.ToString(), font));
                            table.AddCell(CreateCell(item.companyName.ToString(), font));
                            table.AddCell(CreateCell(item.model.ToString(), font));
                            table.AddCell(CreateCell(item.serial.ToString(), font));
                            table.AddCell(CreateCell(item.purchase_date.ToString(), font));
                            table.AddCell(CreateCell(item.cost.ToString("C"), font));
                            table.AddCell(CreateCell(item.locationName.ToString(), font));
                            table.AddCell(CreateCell(item.sub_location.ToString(), font));
                        }
                    }
                    break;
                case "category":
                    foreach (var item in obj.Items) {
                        if (item.sub_location == null) {
                            item.sub_location = "";
                        }
                        if (item.is_deleted == 0) {
                            table.AddCell(CreateCell(item.categoryName.ToString(), font));
                            table.AddCell(CreateCell(item.companyName.ToString(), font));
                            table.AddCell(CreateCell(item.model.ToString(), font));
                            table.AddCell(CreateCell(item.serial.ToString(), font));
                            table.AddCell(CreateCell(item.locationName.ToString(), font));
                            table.AddCell(CreateCell(item.sub_location.ToString(), font));
                        }
                    }
                    break;
                case "manufacturer":
                    foreach (var item in obj.Items) {
                        if (item.sub_location == null) {
                            item.sub_location = "";
                        }
                        if (item.is_deleted == 0) {
                            table.AddCell(CreateCell(item.companyName.ToString(), font));
                            table.AddCell(CreateCell(item.model.ToString(), font));
                            table.AddCell(CreateCell(item.serial.ToString(), font));
                            table.AddCell(CreateCell(item.locationName.ToString(), font));
                            table.AddCell(CreateCell(item.sub_location.ToString(), font));
                        }
                    }
                    break;
                case "location":
                    foreach (var item in obj.Items) {
                        if (item.sub_location == null) {
                            item.sub_location = "";
                        }
                        if (item.is_deleted == 0) {
                            table.AddCell(CreateCell(item.locationName.ToString(), font));
                            table.AddCell(CreateCell(item.sub_location.ToString(), font));
                            table.AddCell(CreateCell(item.companyName.ToString(), font));
                            table.AddCell(CreateCell(item.model.ToString(), font));
                            table.AddCell(CreateCell(item.serial.ToString(), font));
                        }

                    }
                    break;
                case "totalValue":
                    foreach (var item in obj.Items) {
                        if (item.sub_location == null) {
                            item.sub_location = "";
                        }
                        if (item.is_deleted == 0) {
                            table.AddCell(CreateCell(item.categoryName.ToString(), font));
                            table.AddCell(CreateCell(item.cost.ToString("C"), font));
                        }

                    }
                    break;

            }




        }



        private Cell CreateCell(string content, PdfFont font) {
            return new Cell().Add(new Paragraph(content)
                .SetFont(font)
                .SetFontSize(10))
                .SetTextAlignment(TextAlignment.CENTER)
                .SetBorder(new iText.Layout.Borders.SolidBorder(new DeviceRgb(0, 0, 0), 1f));
        }

        private void WriteDataRows(RootObject obj, StreamWriter sw, string filter) {


            switch (filter) {
                case "allData":
                    foreach (var item in obj.Items) {
                        if (item.sub_location == null) {
                            item.sub_location = "";
                        }

                        if (item.is_deleted == 0) {
                            sw.Write(GetStringValue(item.record_id) + ",");
                            sw.Write(GetStringValue(item.categoryName) + ",");
                            sw.Write(GetStringValue(item.companyName) + ",");
                            sw.Write(GetStringValue(item.model) + ",");
                            sw.Write(GetStringValue(item.serial) + ",");
                            sw.Write(GetStringValue(item.purchase_date.ToString("d") + ","));
                            sw.Write(GetStringValue(item.cost.ToString("C") + ","));
                            sw.Write(GetStringValue(item.locationName) + ",");
                            sw.WriteLine(GetStringValue(item.sub_location));
                        }
                    }
                    break;
                case "category":
                    foreach (var item in obj.Items) {
                        if (item.is_deleted == 0) {
                            if (item.sub_location == null) {
                                item.sub_location = "";
                            }
                            sw.Write(GetStringValue(item.categoryName) + ",");
                            sw.Write(GetStringValue(item.companyName) + ",");
                            sw.Write(GetStringValue(item.model) + ",");
                            sw.Write(GetStringValue(item.serial) + ",");
                            sw.Write(GetStringValue(item.locationName) + ",");
                            sw.WriteLine(GetStringValue(item.sub_location));
                        }
                    }
                    break;
                case "manufacturer":
                    foreach (var item in obj.Items) {
                        if (item.is_deleted == 0) {
                            if (item.sub_location == null) {
                                item.sub_location = "";
                            }
                            sw.Write(GetStringValue(item.companyName) + ",");
                            sw.Write(GetStringValue(item.model) + ",");
                            sw.Write(GetStringValue(item.serial) + ",");
                            sw.Write(GetStringValue(item.locationName) + ",");
                            sw.WriteLine(GetStringValue(item.sub_location));
                        }
                    }
                    break;
                case "location":
                    foreach (var item in obj.Items) {
                        if (item.sub_location == null) {
                            item.sub_location = "";
                        }
                        if (item.is_deleted == 0) {
                            sw.Write(GetStringValue(item.locationName) + ",");
                            sw.Write(GetStringValue(item.sub_location) + ",");
                            sw.Write(GetStringValue(item.companyName) + ",");
                            sw.Write(GetStringValue(item.model) + ",");
                            sw.WriteLine(GetStringValue(item.serial));
                        }
                    }
                    break;
                case "totalValue":
                    foreach (var item in obj.Items) {
                        if (item.sub_location == null) {
                            item.sub_location = "";
                        }
                        if (item.is_deleted == 0) {
                            sw.Write(GetStringValue(item.categoryName) + ",");
                            sw.WriteLine(GetStringValue(item.cost.ToString("C") + ","));
                        }
                    }
                    break;
            }


        }

        private string GetStringValue(object value) {
            return value != null ? value.ToString() : "";
        }


        private decimal CalculateTotalCost(RootObject obj, string title) {
            decimal totalCost = 0;
            foreach (var item in obj.Items) {
                if (item.is_deleted == 0) {
                    totalCost += item.cost;
                }
            }

            return totalCost;
        }

        private void AddTotalCostRow(Table table, PdfFont font, decimal totalCost) {
            for (int i = 0; i < table.GetNumberOfColumns() - 2; i++) {
                table.AddCell(CreateCell("", font));
            }
            table.AddCell(CreateCell("Total Cost:", font));
            table.AddCell(CreateCell(totalCost.ToString("C"), font));
        }

        private void WriteTotalCostRow(StreamWriter sw, RootObject obj, string filter) {
            decimal totalCost = CalculateTotalCost(obj, filter);

            sw.Write("Total Cost:," + totalCost.ToString("C") + ",");
            sw.WriteLine();
        }


    }
}
