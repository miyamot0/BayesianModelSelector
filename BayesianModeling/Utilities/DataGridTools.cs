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
            object rawRow = dataGrid.ItemContainerGenerator.ContainerFromItem(dataGridCellInfo.Item);
            DataGridRow gridRow = rawRow as DataGridRow;

            if (gridRow != null)
            {
                return gridRow.GetIndex();
            }
            else
            {
                return -1;
            }
        }

        public static DataGridRow GetDataGridRow(DataGrid dataGrid, int index)
        {
            object rawRow = dataGrid.ItemContainerGenerator.ContainerFromIndex(index);
            DataGridRow gridRow = rawRow as DataGridRow;
            return gridRow;
        }

        public static DataGridCell GetDataGridCell(DataGrid dataGrid, DataGridRow gridRow, int gridColumn)
        {
            DataGridCellsPresenter gridPresenter = GetVisualChild<System.Windows.Controls.Primitives.DataGridCellsPresenter>(gridRow);
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

        public static T GetVisualChild<T>(Visual parent) where T : Visual
        {
            T child = default(T);
            int numVisuals = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < numVisuals; i++)
            {
                Visual v = (Visual)VisualTreeHelper.GetChild(parent, i);
                child = v as T;
                if (child == null)
                {
                    child = GetVisualChild<T>(v);
                }
                if (child != null)
                {
                    break;
                }
            }
            return child;
        }
    }
}
