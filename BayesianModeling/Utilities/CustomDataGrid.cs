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
using System.ComponentModel;
using System.Windows.Data;
using System.Windows.Controls.Primitives;

namespace BayesianModeling.Utilities
{
    public class CustomDataGrid : DataGrid
    {
        /// <summary>
        /// Extension of DataGrid control to include row numbers and on-demand paste operations
        /// </summary>
        public CustomDataGrid()
        {
            CommandManager.RegisterClassCommandBinding(typeof(CustomDataGrid),
                new CommandBinding(ApplicationCommands.Paste,
                new ExecutedRoutedEventHandler(OnExecutedPaste),
                new CanExecuteRoutedEventHandler(OnCanExecutePaste)));

            LoadingRow += (object target, DataGridRowEventArgs eArgs) =>
            {
                eArgs.Row.Header = eArgs.Row.GetIndex();
            };

            ItemContainerGenerator.ItemsChanged += (object target, ItemsChangedEventArgs eArgs) =>
            {
                foreach (var item in Items)
                {
                    DataGridRow row = (DataGridRow) ItemContainerGenerator.ContainerFromItem(item);

                    if (row != null)
                    {
                        row.Header = row.GetIndex();
                    }
                }
            };
        }

        /// <summary>
        /// Command binding event call to CanExecute parameters
        /// </summary>
        private static void OnCanExecutePaste(object sender, CanExecuteRoutedEventArgs paras)
        {
            ((CustomDataGrid)sender).OnCanExecutePaste(paras);
        }

        /// <summary>
        /// Method returning (if not null) that action can be executed and is handled
        /// </summary>
        protected virtual void OnCanExecutePaste(CanExecuteRoutedEventArgs paras)
        {
            paras.CanExecute = (CurrentCell != null);
            paras.Handled = true;
        }

        /// <summary>
        /// Command binding event call to Execute parameters/methods
        /// </summary>
        private static void OnExecutedPaste(object sender, ExecutedRoutedEventArgs paras)
        {
            ((CustomDataGrid)sender).OnExecutedPaste(paras);
        }

        /// <summary>
        /// Paste operation, parsing clipboard information into collection of string arrays, iterating through cells and updating as needed
        /// </summary>
        protected virtual void OnExecutedPaste(ExecutedRoutedEventArgs paras)
        {
            List<string[]> rowData = ClipboardTools.ReadAndParseClipboardData();

            int lowRow = Items.IndexOf(CurrentItem),        // Current highlighted cell's row
                highRow = Items.Count - 1,                  // Highest row in table
                lowCol = Columns.IndexOf(CurrentColumn),    // Current highlighted cell's column
                highCol = Columns.Count - 1,                // Highest column index in tabl
                pasteContentRowIterator = 0,
                pasteContentColumnIterator = 0;

            var rowSource = (IEditableCollectionView)CollectionViewSource.GetDefaultView(Items);

            for (int i = lowRow; (i <= highRow) && (pasteContentRowIterator < rowData.Count); i++)
            {
                if (i == highRow)
                {
                    rowSource.AddNew();                     // Add new rows if more space needed
                    if (pasteContentRowIterator + 1 < rowData.Count)
                    {
                        highRow++;                          // Update table row max
                    }
                }

                pasteContentColumnIterator = 0;

                for (int j = lowCol; (j < highCol) && (pasteContentColumnIterator < rowData[pasteContentRowIterator].Length); j++)
                {
                    Items[i].GetType().GetProperty(DataGridTools.GetColumnName(j)).SetValue(Items[i], rowData[pasteContentRowIterator][pasteContentColumnIterator], null);
                    pasteContentColumnIterator++;
                }

                pasteContentRowIterator++;
            }
        }
    }
}
