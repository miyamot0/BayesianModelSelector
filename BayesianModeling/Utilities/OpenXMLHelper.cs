/* 
    Copyright 2016 Shawn Gilroy

    This file is part of Bayesian Model Selector.

    Bayesian Model Selector is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, version 2.

    Bayesian Model Selector is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with Bayesian Model Selector.  If not, see <http://www.gnu.org/licenses/gpl-2.0.html>.

    This project utilizes EPPlus to leverage interactions with OpenXML formats

    ============================================================================

    EPPlus is distributed under this license:

    Copyright (c) 2016 Jan Källman

    EPPlus is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, version 2.

    EPPlus is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with EPPlus.  If not, see <http://epplus.codeplex.com/license>.
     
*/

using BayesianModeling.View;
using BayesianModeling.ViewModel;
using Microsoft.VisualBasic.FileIO;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;

namespace BayesianModeling.Utilities
{
    public class OpenXMLHelper
    {

        /// <summary>
        /// Reads from CSV file
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns>
        /// Observable collection for main view
        /// </returns>
        public static ObservableCollection<RowViewModel> ReadFromCSVFile(string filePath)
        {
            ObservableCollection<RowViewModel> temp = new ObservableCollection<RowViewModel>();

            using (TextFieldParser parser = new TextFieldParser(filePath))
            {
                parser.TextFieldType = FieldType.Delimited;
                parser.SetDelimiters(",");

                while (!parser.EndOfData)
                {
                    string[] fields = parser.ReadFields();

                    RowViewModel mModel = new RowViewModel();
                    for (int i = 0; i < fields.Length && i < 100; i++)
                    {
                        mModel.values[i] = fields[i];
                    }

                    temp.Add(mModel);

                }
            }

            return temp;
        }

        /// <summary>
        /// Reads from XLSX file
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns>
        /// Observable collection for main view
        /// </returns>
        public static ObservableCollection<RowViewModel> ReadFromExcelFile(string filePath, out string sheet)
        {
            FileInfo existingFile = new FileInfo(filePath);
            ObservableCollection<RowViewModel> temp = new ObservableCollection<RowViewModel>();
            sheet = string.Empty;

            using (ExcelPackage package = new ExcelPackage(existingFile))
            {
                var wsMult = package.Workbook.Worksheets;

                List<string> workSheets = new List<string>();

                foreach (ExcelWorksheet sheetPeek in wsMult)
                {
                    workSheets.Add(sheetPeek.Name);
                }

                string[] workSheetsArray = workSheets.ToArray();

                var sheetWindow = new SelectionWindow(workSheetsArray, workSheetsArray[0]);
                sheetWindow.Title = "Pick a sheet";
                sheetWindow.MessageLabel.Text = "Select which spreadsheet to load";
                sheetWindow.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                sheetWindow.Topmost = true;

                int output = -1;

                if (sheetWindow.ShowDialog() == true)
                {
                    output = sheetWindow.MessageOptions.SelectedIndex + 1;

                    sheet = workSheetsArray[sheetWindow.MessageOptions.SelectedIndex];
                }

                if (output == -1)
                {
                    return null;
                }

                var ws = package.Workbook.Worksheets[sheetWindow.MessageOptions.SelectedIndex + 1];

                int currRows = 50;

                for (int i = 0; i < currRows; i++)
                {
                    temp.Add(new RowViewModel());
                }

                var cellsUsed = ws.Cells;

                foreach (var cell in cellsUsed)
                {
                    var colStr = DataGridTools.GetColumnIndex(new String(cell.Address.ToCharArray().Where(c => !Char.IsDigit(c)).ToArray()));
                    var rowStr = int.Parse(new String(cell.Address.ToCharArray().Where(c => Char.IsDigit(c)).ToArray()));

                    if (rowStr >= currRows)
                    {
                        while (currRows < rowStr)
                        {
                            temp.Add(new RowViewModel());
                            currRows++;
                        }
                    }

                    if (colStr - 1 >= 99)
                    {
                        continue;
                    }

                    if (cell.Text.Length > 0)
                    {
                        temp[rowStr - 1].values[colStr] = cell.Text;
                    }
                }

            }

            return temp;
        }

        /// <summary>
        /// Write contents of RowModels to spreadsheet
        /// <param name="rowCollection">
        /// Contents of data grid
        /// </param>
        /// <param name="filePath">
        /// Output location for .csv file
        /// </param>
        /// </summary>
        public static void ExportToCSV(ObservableCollection<RowViewModel> rowCollection, string filePath)
        {
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }

            using (var csv = new StreamWriter(filePath, true))
            {
                foreach (RowViewModel rvm in rowCollection)
                {
                    var newLine = string.Join(",", rvm.values);
                    csv.WriteLine(newLine);
                }

                csv.Close();
            }
        }

        /// <summary>
        /// Write contents of RowModels to spreadsheet
        /// <param name="rowCollection">
        /// Contents of data grid
        /// </param>
        /// <param name="filePath">
        /// Output location for .xlsx file
        /// </param>
        /// </summary>
        public static void ExportToExcel(ObservableCollection<RowViewModel> rowCollection, string filePath)
        {
            FileInfo newFile = new FileInfo(filePath);
            if (newFile.Exists)
            {
                newFile.Delete();
                newFile = new FileInfo(filePath);
            }

            using (ExcelPackage package = new ExcelPackage(newFile))
            {
                var wsMult = package.Workbook.Worksheets;

                List<string> workSheets = new List<string>();

                foreach (ExcelWorksheet sheetPeek in wsMult)
                {
                    workSheets.Add(sheetPeek.Name);
                }

                string[] workSheetsArray = workSheets.ToArray();

                int position = Array.IndexOf(workSheetsArray, "Model Selector");

                ExcelWorksheet ws;

                if (position > -1)
                {
                    ws = package.Workbook.Worksheets[position + 1];
                }
                else
                {
                    ws = package.Workbook.Worksheets.Add("Model Selector");
                }

                for (int i = 0; i < rowCollection.Count; i++)
                {
                    for (int j = 0; j < 100; j++)
                    {
                        ws.Cells[i + 1, j + 1].Value = rowCollection[i].values[j];
                    }
                }

                package.Compression = CompressionLevel.Default;
                package.Save();

            }
        }

        /// <summary>
        /// Write contents of RowModels to spreadsheet
        /// <param name="rowCollection">
        /// Contents of data grid
        /// </param>
        /// <param name="filePath">
        /// Output location for .xlsx file
        /// </param>
        /// <param name="worksheetName">
        /// Specific sheet to update
        /// </param>
        /// </summary>
        public static void ExportToExcel(ObservableCollection<RowViewModel> rowCollection, string filePath, string worksheetName)
        {
            FileInfo newFile = new FileInfo(filePath);
            if (newFile.Exists)
            {
                newFile.Delete();
                newFile = new FileInfo(filePath);
            }

            using (ExcelPackage package = new ExcelPackage(newFile))
            {
                var wsMult = package.Workbook.Worksheets;

                List<string> workSheets = new List<string>();

                foreach (ExcelWorksheet sheetPeek in wsMult)
                {
                    workSheets.Add(sheetPeek.Name);
                }

                string[] workSheetsArray = workSheets.ToArray();

                int position = Array.IndexOf(workSheetsArray, worksheetName);

                ExcelWorksheet ws;

                if (position > -1)
                {
                    ws = package.Workbook.Worksheets[position + 1];
                }
                else
                {
                    ws = package.Workbook.Worksheets.Add(worksheetName);
                }

                for (int i = 0; i < rowCollection.Count; i++)
                {
                    for (int j = 0; j < 100; j++)
                    {
                        ws.Cells[i + 1, j + 1].Value = rowCollection[i].values[j].ToString();
                    }
                }

                package.Save();

            }
        }
    }
}
