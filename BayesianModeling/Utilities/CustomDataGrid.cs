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
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows;
using System.ComponentModel;
using System.Windows.Data;
using System;
using System.Windows.Controls.Primitives;

namespace BayesianModeling.Utilities
{
    public class CustomDataGrid : DataGrid
    {
        public CustomDataGrid()
        {
            CommandManager.RegisterClassCommandBinding(typeof(CustomDataGrid), 
                new CommandBinding(ApplicationCommands.Paste,
                new ExecutedRoutedEventHandler(OnExecutedPaste),
                new CanExecuteRoutedEventHandler(OnCanExecutePaste)));
        }

        public static DependencyProperty RowNumber = DependencyProperty.RegisterAttached("DisplayRowNumbers",
            typeof(bool),
            typeof(CustomDataGrid),
            new FrameworkPropertyMetadata(false, ChangeRowNumberEvent));

        public static bool GetDisplayRowNumbers(DependencyObject sender)
        {
            return ((bool) sender.GetValue(RowNumber));
        }

        public static void SetDisplayRowNumbers(DependencyObject sender, bool value)
        {
            sender.SetValue(RowNumber, value);
        }

        private static void ChangeRowNumberEvent(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            if ((bool)args.NewValue == false)
                return;

            ((DataGrid)sender).LoadingRow += (object target, DataGridRowEventArgs eArgs) =>
            {
                eArgs.Row.Header = eArgs.Row.GetIndex();
            };

            ((DataGrid)sender).ItemContainerGenerator.ItemsChanged += (object target, ItemsChangedEventArgs eArgs) =>
            {
                DataGridTools.GetDataGridChildren(((DataGrid)sender), new List<DataGridRow>()).ForEach(d => d.Header = d.GetIndex());
            };
        }

        private static void OnCanExecutePaste(object sender, CanExecuteRoutedEventArgs paras)
        {
            ((CustomDataGrid)sender).OnCanExecutePaste(paras);
        }

        protected virtual void OnCanExecutePaste(CanExecuteRoutedEventArgs paras)
        {
            paras.CanExecute = (CurrentCell != null);
            paras.Handled = true;
        }

        private static void OnExecutedPaste(object sender, ExecutedRoutedEventArgs paras)
        {
            ((CustomDataGrid)sender).OnExecutedPaste(paras);
        }

        protected virtual void OnExecutedPaste(ExecutedRoutedEventArgs paras)
        {
            List<string[]> rowData = ClipboardTools.ReadAndParseClipboardData();

            int lowRow = Items.IndexOf(CurrentItem),
                highRow = Items.Count - 1,
                lowCol = Columns.IndexOf(CurrentColumn),
                highCol = Columns.Count - 1,
                pasteContentRowIterator = 0,
                pasteContentColumnIterator = 0;

            for (int i = lowRow; (i <= highRow) && (pasteContentRowIterator < rowData.Count); i++)
            {
                if (i == highRow)
                {
                    (CollectionViewSource.GetDefaultView(Items) as IEditableCollectionView).AddNew();
                    if (pasteContentRowIterator + 1 < rowData.Count)
                    {
                        highRow = Items.Count - 1;
                    }
                }

                pasteContentColumnIterator = 0;

                for (int j = lowCol; (j < highCol) && (pasteContentColumnIterator < rowData[pasteContentRowIterator].Length); j++)
                {
                    string propertyName = ((ColumnFromDisplayIndex(j) as DataGridBoundColumn).Binding as Binding).Path.Path;

                    if ((Items[i].GetType().GetProperty(propertyName)) != null)
                    {
                        object convertedValue = Convert.ChangeType(rowData[pasteContentRowIterator][pasteContentColumnIterator],
                            (Items[i].GetType().GetProperty(propertyName)).PropertyType);

                        Items[i].GetType().GetProperty(propertyName).SetValue(Items[i], convertedValue, null);
                    }

                    pasteContentColumnIterator++;
                }

                pasteContentRowIterator++;
            }
        }
    }
}
