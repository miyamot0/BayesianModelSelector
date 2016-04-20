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

        public static string GetColumnName(int index)
        {
            var value = "";

            if (index >= letters.Length)
            {
                value = value + letters[index / letters.Length - 1];
            }

            value = value + letters[index % letters.Length];

            return value;
        }

        public static int GetDataGridRowIndex(DataGrid dataGrid, DataGridCellInfo dataGridCellInfo)
        {
            DataGridRow gridRow = dataGrid.ItemContainerGenerator.ContainerFromItem(dataGridCellInfo.Item) as DataGridRow;
            return (gridRow != null) ? gridRow.GetIndex() : -1;
        }

        public static DataGridRow GetDataGridRow(DataGrid dataGrid, int index)
        {
            return (DataGridRow) dataGrid.ItemContainerGenerator.ContainerFromIndex(index);
        }

        public static DataGridCell GetDataGridCell(DataGrid dataGrid, DataGridRow gridRow, int gridColumn)
        {
            DataGridCellsPresenter gridPresenter = GetGridPresenter(gridRow);
            DataGridCell gridCell = (DataGridCell) gridPresenter.ItemContainerGenerator.ContainerFromIndex(gridColumn);
            return gridCell;
        }

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
                else if (child != null)
                {
                    GetDataGridChildren(child, visualCollection);
                }
            }

            return visualCollection;
        }

        public static DataGridCellsPresenter GetGridPresenter(Visual parent) 
        {
            DataGridCellsPresenter cellPresenter = null;
            Visual visual;

            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
            {
                visual = (Visual) VisualTreeHelper.GetChild(parent, i);
                cellPresenter = visual as DataGridCellsPresenter;

                if (cellPresenter == null)
                {
                    cellPresenter = GetGridPresenter(visual);
                }
                else
                {
                    break;
                }
            }
            return cellPresenter;
        }
    }
}
