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

*/

using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace BayesianModeling.Utilities
{
    public class DataGridTools
    {
        static string letters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";

        /// <summary>
        /// Conversion of integer to alphabet-based index (similar to Spreadsheet column indexing workflow).
        /// <param name="index">
        /// Takes integer, converting to referenced letters string, concatenating as necessary
        /// </param>
        /// </summary>
        public static string GetColumnName(int index)
        {
            var value = "";

            if (index >= letters.Length)
            {
                // Add front-based character based on # of iterations past alphabet length
                value = value + letters[index / letters.Length - 1];
            }

            // Add trailing character based on modulus (% 26) index
            value = value + letters[index % letters.Length];

            return value;
        }

        /// <summary>
        /// Lookup of grid row index based on the current grid and cell supplied.
        /// <param name="dataGrid">
        /// The current data grid to be traversed
        /// </param>
        /// <param name="dataGridCellInfo">
        /// Object representing the current cell being referenced, be to used to traverse datagrid collection
        /// </param>
        /// <returns>
        /// Row index, -1 if null
        /// </returns>
        /// </summary>
        public static int GetDataGridRowIndex(DataGrid dataGrid, DataGridCellInfo dataGridCellInfo)
        {
            DataGridRow gridRow = dataGrid.ItemContainerGenerator.ContainerFromItem(dataGridCellInfo.Item) as DataGridRow;
            return (gridRow != null) ? gridRow.GetIndex() : -1;
        }

        /// <summary>
        /// Lookup of grid row index based on the index supplied.
        /// <param name="dataGrid">
        /// The current data grid to be traversed
        /// </param>
        /// <param name="index">
        /// Integer indicating the referenced index in datagrid collection
        /// </param>
        /// <returns>
        /// DataGridRow, as per the supplied index
        /// </returns>
        /// </summary>
        public static DataGridRow GetDataGridRow(DataGrid dataGrid, int index)
        {
            return (DataGridRow) dataGrid.ItemContainerGenerator.ContainerFromIndex(index);
        }

        /// <summary>
        /// Lookup of grid cell based on the grid, row and column supplied.
        /// <param name="dataGrid">
        /// The current data grid to be traversed
        /// </param>
        /// <param name="gridRow">
        /// The current row to be referenced
        /// </param>
        /// <param name="gridColumn">
        /// The column index to be referenced
        /// </param>
        /// <returns>
        /// DataGridCell, as per the supplied grid, row and column index
        /// </returns>
        /// </summary>
        public static DataGridCell GetDataGridCell(DataGrid dataGrid, DataGridRow gridRow, int gridColumn)
        {
            DataGridCellsPresenter gridPresenter = GetGridPresenter(gridRow);
            DataGridCell gridCell = (DataGridCell) gridPresenter.ItemContainerGenerator.ContainerFromIndex(gridColumn);
            return gridCell;
        }

        /// <summary>0
        /// Traverse the visual tree of the supplied parent (a data grid itself).
        /// <param name="parent">
        /// Takes visual parent, a datagrid containing rows (among other things)
        /// </param>
        /// <param name="visualCollection">
        /// Collection carried throughout iterations, collecting DataGridRows throughout
        /// </param>
        /// <returns>
        /// Collection of DataGridRows
        /// </returns>
        /// </summary>
        public static List<DataGridRow> GetDataGridChildren(object parent, List<DataGridRow> visualCollection)
        {
            DependencyObject child;
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount((DependencyObject)parent); i++)
            {
                child = VisualTreeHelper.GetChild((DependencyObject)parent, i);

                if (child.GetType() == typeof(DataGridRow))
                {
                    visualCollection.Add((DataGridRow)child);
                }
                else
                {
                    GetDataGridChildren(child, visualCollection);
                }
            }

            return visualCollection;
        }

        /// <summary>
        /// Traverse the visual tree of the supplied parent, returning the DataGridCells Presenter.
        /// <param name="index">
        /// Takes visual parent, iterating through children as needed until DGP found
        /// </param>
        /// <returns>
        /// DataGridCellsPresenter within supplied parent
        /// </returns>
        /// </summary>
        public static DataGridCellsPresenter GetGridPresenter(Visual parent) 
        {
            DataGridCellsPresenter cellPresenter = null;
            Visual visual;

            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
            {
                visual = (Visual) VisualTreeHelper.GetChild(parent, i);

                cellPresenter = visual as DataGridCellsPresenter;

                if (cellPresenter != null)
                {
                    break;
                }
                else
                {
                    cellPresenter = GetGridPresenter(visual);
                }
            }

            return cellPresenter;
        }
    }
}
