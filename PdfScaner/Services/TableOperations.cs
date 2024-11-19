using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PdfScaner.Services
{

    public static class TableOperations
    {

        // Добавление новой строки
        public static void AddRow(ObservableCollection<string[]> tableData)
        {
            tableData.Add(new string[] { "", "", "" }); 
        }

        // Удаление последней строки
        public static void RemoveRow(ObservableCollection<string[]> tableData)
        {
            if (tableData.Count > 0)
                tableData.RemoveAt(tableData.Count - 1); 
        }

        // Добавление нового столбца
        public static void AddColumn(ObservableCollection<string[]> tableData)
        {
            for (int i = 0; i < tableData.Count; i++)
            {
                var row = tableData[i];
                var newRow = row.ToList();
                newRow.Add(""); 
                tableData[i] = newRow.ToArray(); 
            }
        }

        // Удаление последнего столбца
        public static void RemoveColumn(ObservableCollection<string[]> tableData)
        {
            if (tableData.Count > 0 && tableData[0].Length > 1)
            {
                for (int i = 0; i < tableData.Count; i++)
                {
                    var row = tableData[i];
                    Array.Resize(ref row, row.Length - 1); 
                    tableData[i] = row; 
                }
            }
        }
      }
    }
