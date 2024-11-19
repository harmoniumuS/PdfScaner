using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Kernel.Colors;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using PdfScaner.Services;
using iText.Layout.Properties;

namespace PdfScaner.ViewModels
{

    public class MainViewModel : INotifyPropertyChanged
    {
        public MainViewModel()
        {
            TableData = new ObservableCollection<string[]>
        {
            new string[] { "", "", "" } 
        };

            // Инициализация команд true-добавление,false-удаление
            PrintCommand = new RelayCommand(Print);
            AddRowCommand = new RelayCommand(() => ModifyRow(true)); 
            RemoveRowCommand = new RelayCommand(() => ModifyRow(false)); 
            AddColumnCommand = new RelayCommand(() => ModifyColumn(true)); 
            RemoveColumnCommand = new RelayCommand(() => ModifyColumn(false)); 
        }

        public ObservableCollection<string[]> TableData { get; set; }
        public ICommand PrintCommand { get; }
        public ICommand AddRowCommand { get; }
        public ICommand AddColumnCommand { get; }
        public ICommand RemoveRowCommand { get; }
        public ICommand RemoveColumnCommand { get; }

        public string InputText
        {
            get => _inputText;
            set
            {
                _inputText = value;
                OnPropertyChanged();
                LabelText = _inputText;
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
        private string _inputText;
        private string _labelText;

        private void Print()
        {
            var printService = new PrintService(InputText, TableData);
            printService.ExecutePrint();
            Application.Current.Shutdown();
        }

        // Универсальный метод для добавления/удаления строк
        private void ModifyRow(bool add)
        {
            if (add)
                TableOperations.AddRow(TableData);
            else
                TableOperations.RemoveRow(TableData);

            UpdateTable();
        }

        // Универсальный метод для добавления/удаления столбцов
        private void ModifyColumn(bool add)
        {
            if (add)
                TableOperations.AddColumn(TableData);
            else
                TableOperations.RemoveColumn(TableData);

            UpdateTable();
        }

        // Метод для обновления привязки и столбцов
        private void UpdateTable()
        {
            OnPropertyChanged(nameof(TableData)); 
            UpdateColumns(); 
        }

        // Обновление столбцов в DataGrid
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
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


    }
}