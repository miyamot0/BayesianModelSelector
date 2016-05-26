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
    it under the terms of the GNU Library General Public License as published by
    the Free Software Foundation.

    EPPlus is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU Library General Public License
    along with EPPlus.  If not, see <http://epplus.codeplex.com/license>.

    This file uses EPP to leverage interactions with OOXML documents
     
*/

using BayesianModeling.ViewModel;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;

namespace BayesianModeling.Utilities
{
    public class OpenXMLHelper
    {
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
//                        ws.Cells[i + 1, j + 1].Style.
                        ws.Cells[i + 1, j + 1].Value = rowCollection[i].values[j];
                        //Console.WriteLine("Row: {0} Column: {1} Value: {2}", i, j, rowCollection[i].values[j].ToString());
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