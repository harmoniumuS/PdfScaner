using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PdfScaner.Services
{
    public class PrintService
    {
        public PrintService(string inputText, ObservableCollection<string[]> tableData)
        {
            _inputText = inputText;
            _tableData = tableData;
        }
        private readonly string _inputText;
        private readonly ObservableCollection<string[]> _tableData;

        public void ExecutePrint()
        {
            var saveFileDialog = new SaveFileDialog
            {
                Filter = "Pdf Files (*.pdf)|*.pdf",
                FileName = $"{_inputText}.pdf"
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                var fileName = saveFileDialog.FileName;
                CreatePdf(fileName);
            }
        }

        private void CreatePdf(string fileName)
        {
            using (var writer = new PdfWriter(fileName))
            using (var pdf = new PdfDocument(writer))
            using (var document = new Document(pdf))
            {
                // Добавляем заголовок
                document.Add(new Paragraph(_inputText)
                    .SetFontSize(16)
                    .SetBold()
                    .SetTextAlignment(TextAlignment.LEFT)
                    .SetMarginBottom(10));

                var table = new Table(UnitValue.CreatePercentArray(_tableData.First().Length))
                    .UseAllAvailableWidth()
                    .SetHorizontalAlignment(HorizontalAlignment.CENTER);

                // Добавляем строки данных
                foreach (var row in _tableData)
                {
                    foreach (var cell in row)
                    {
                        var cellElement = new Cell()
                            .Add(new Paragraph(cell)
                                .SetFontSize(12)
                                .SetTextAlignment(TextAlignment.CENTER))
                            .SetBorder(new iText.Layout.Borders.SolidBorder(1));

                        table.AddCell(cellElement);
                    }
                }

                // Добавляем таблицу в документ
                document.Add(table);
            }
        }
    }

}
