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

    This project utilizes ClosedXML to leverage interactions with OpenXML formats

    ============================================================================

    ClosedXML is distributed under this license:

    Copyright (c) 2010 Manuel De Leon

    Permission is hereby granted, free of charge, to any person obtaining a copy of 
    this software and associated documentation files (the "Software"), to deal in the 
    Software without restriction, including without limitation the rights to use, 
    copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the 
    Software, and to permit persons to whom the Software is furnished to do so, 
    subject to the following conditions:

    The above copyright notice and this permission notice shall be included in all 
    copies or substantial portions of the Software.

    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR 
    IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, 
    FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE 
    AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, 
    WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN 
    CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
     
*/

using BayesianModeling.ViewModel;
using ClosedXML.Excel;
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
            XLWorkbook wb;

            if (File.Exists(filePath))
            {
                wb = new XLWorkbook(@filePath);
            }
            else
            {
                wb = new XLWorkbook();
            }

            IXLWorksheet ws;

            if (!wb.TryGetWorksheet("Model Selector", out ws))
            {
                ws = wb.AddWorksheet("Model Selector");
            }

            for (int i = 0; i < rowCollection.Count; i++)
            {
                for (int j = 0; j < 100; j++)
                {
                    ws.Cell(i + 1, j + 1).Value = rowCollection[i].values[j].ToString();
                }
            }

            wb.SaveAs(filePath);
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
            XLWorkbook wb;

            if (File.Exists(filePath))
            {
                wb = new XLWorkbook(@filePath);
            }
            else
            {
                wb = new XLWorkbook();
            }

            IXLWorksheet ws;

            if (!wb.TryGetWorksheet(worksheetName, out ws))
            {
                ws = wb.AddWorksheet(worksheetName);
            }

            for (int i = 0; i < rowCollection.Count; i++)
            {
                for (int j = 0; j < 100; j++)
                {
                    ws.Cell(i + 1, j + 1).Value = rowCollection[i].values[j].ToString();
                }
            }

            wb.SaveAs(filePath);
        }
    }
}