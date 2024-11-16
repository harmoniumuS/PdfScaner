using iText.Kernel.Pdf;
using iText.Layout;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reflection.Metadata;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;
using System.Windows.Input;
using iText.Bouncycastleconnector;
using iText.Layout.Element;
using System.Windows;
using iText.Kernel.Colors;
using Microsoft.Win32;
using System.Windows.Controls;
using System.Windows.Data;


namespace PdfScaner.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        public string InputText
        {
            get => _inputText;
            set
            {
                _inputText = value;
                OnPropertyChanged();
                _labelText = _inputText;
            }
        }
        public string LabelText
        {
            get => _labelText;
            set
            {
                _labelText = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public ObservableCollection<string[]> TableData { get; set; }

        public ICommand PrintCommand { get; }
        public ICommand AddRowCommand { get; }
        public ICommand AddColumnCommand { get; }


        private string _inputText;
        private string _labelText;

        public MainViewModel()
        {
            PrintCommand = new RelayCommand(Print);
            AddRowCommand = new RelayCommand(AddRow);
            AddColumnCommand = new RelayCommand(AddColumn);

            TableData = new ObservableCollection<string[]>
            {
                new string[] { "", "", ""},
                new string[] { "", "", ""},
                new string[] { "", "", ""},
            };
        }


        private void Print()
        {
            try
            {   //Используем для выбора места сохранения файла
                var saveFileDialog = new SaveFileDialog
                {
                    Filter = "Pdf Files (*.pdf|*.pdf)",
                    FileName = $"{InputText}.pdf"
                };
                //Если пользователь выбрал путь, продолжаем создание файла
                if (saveFileDialog.ShowDialog() == true)
                {
                    var fileName = saveFileDialog.FileName;
                    using (var writer = new PdfWriter(fileName))
                    using (var pdf = new PdfDocument(writer))
                    {
                        var document = new iText.Layout.Document(pdf);

                        // Добавляем основное название
                        document.Add(new iText.Layout.Element.Paragraph(InputText)
                            .SetFontSize(16)
                            .SetBold()
                            .SetTextAlignment(iText.Layout.Properties.TextAlignment.CENTER));

                        // Создаем таблицу с количеством колонок, равным количеству элементов в первой строке
                        var table = new iText.Layout.Element.Table(TableData[0].Length);
                        table.SetHorizontalAlignment(iText.Layout.Properties.HorizontalAlignment.CENTER);



                        // Добавляем заголовки столбцов
                        for (int i = 0; i < TableData[0].Length; i++)
                        {
                            var headerCell = new Cell()
                                .Add(new iText.Layout.Element.Paragraph(TableData[0][i])
                                .SetTextAlignment(iText.Layout.Properties.TextAlignment.CENTER))
                                .SetBold()
                                .SetBackgroundColor(new DeviceRgb(200, 200, 200)); // Фон для заголовков
                            table.AddCell(headerCell);
                        }

                        // Заполняем таблицу данными (начиная с 1, чтобы не добавить повторно заголовки)kkl
                        for (int i = 1; i < TableData.Count; i++)  // Начинаем с 1, чтобы пропустить первую строку (заголовки)
                        {
                            var row = TableData[i];
                            foreach (var cell in row)
                            {
                                var tableCell = new Cell()
                                    .Add(new iText.Layout.Element.Paragraph(cell)
                                    .SetTextAlignment(iText.Layout.Properties.TextAlignment.CENTER));

                                table.AddCell(tableCell);
                            }
                        }

                        // Добавляем таблицу в документ
                        document.Add(table);
                    }
                }


                // Завершаем приложение после создания PDF
                Application.Current.Shutdown();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при создании: {ex.Message}");
                throw;
            }
        }
        private void AddRow()
        {
            var newRow = TableData.FirstOrDefault().Select(cell => string.Empty).ToArray();
            if (newRow != null)
            {
                TableData.Add(newRow);
            }
            OnPropertyChanged(nameof(TableData));
        }

        private void AddColumn()
        {
            // Добавляем новый столбец в данные
            var newTableData = new ObservableCollection<string[]>();

            foreach (var row in TableData)
            {
                var rowList = row.ToList();
                rowList.Add(string.Empty);  // Добавляем пустой столбец
                newTableData.Add(rowList.ToArray());  // Добавляем обновленную строку в новую коллекцию
            }

            // Заменяем старую коллекцию новой
            TableData.Clear();
            foreach (var row in newTableData)
            {
                TableData.Add(row);
            }

            // Обновляем столбцы в DataGrid
            UpdateColumns();
        }

        private void UpdateColumns()
        {
            var dataGrid = Application.Current.MainWindow.FindName("DataGridControl") as DataGrid;
            if (dataGrid != null)
            {
                dataGrid.Columns.Clear();

                int columnCount = TableData.FirstOrDefault()?.Length ?? 0;
                for (int i = 0; i < columnCount; i++)
                {
                    var column = new DataGridTextColumn
                    {
                        Header = $"Column {i + 1}",
                        Binding = new Binding($"[{i}]")
                    };

                    dataGrid.Columns.Add(column);
                }
            }
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}
