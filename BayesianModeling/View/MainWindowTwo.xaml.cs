using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace BayesianModeling.View
{
    /// <summary>
    /// Interaction logic for MainWindowTwo.xaml
    /// </summary>
    public partial class MainWindowTwo : Window
    {
        public MainWindowTwo()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            DataTable dt = new DataTable();

            dt.Columns.Add("*");
            dt.Columns[0].DataType = System.Type.GetType("System.Int32");
            dt.Columns[0].AutoIncrement = true;
            dt.Columns[0].AutoIncrementSeed = 1;
            dt.Columns[0].AutoIncrementStep = 1;
            dt.Columns[0].ReadOnly = true;

            var MyStyle = new Style(typeof(DataGridCell))
            {
                Setters = {
                    new Setter(TextBlock.TextAlignmentProperty, TextAlignment.Center),
                    new Setter(TextBlock.IsEnabledProperty, false),
                }
            };

            for (int i=0; i<26; i++)
            {
                dt.Columns.Add(GetColumnName(i));
            }

            for (int i=0; i<26; i++)
            {
                dt.Rows.Add();
            }

            dataGrid.ColumnWidth = 100;

            dataGrid.ItemsSource = dt.AsDataView();
            dataGrid.Columns[0].CellStyle = MyStyle;


            DiscountingWindow2 mText = new DiscountingWindow2();
            mText.mWindowRef = this;
            mText.Topmost = true;
            mText.Show();
//            dataGrid.PreviewMouseUp += DataGrid_PreviewMouseUp;

        }

        public int GetRowIndex(DataGrid dataGrid, DataGridCellInfo dataGridCellInfo)
        {
            DataGridRow dgrow = (DataGridRow)dataGrid.ItemContainerGenerator.ContainerFromItem(dataGridCellInfo.Item);
            if (dgrow != null)
                return dgrow.GetIndex();

            return -1;
        }

        private void DataGrid_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            Console.WriteLine("UP ");

            List<DataGridCellInfo> cells = dataGrid.SelectedCells.ToList();

            var lowRow = cells.Min(i => GetRowIndex(dataGrid, i));
            var highRow = cells.Max(i => GetRowIndex(dataGrid, i));

            var lowCol = cells.Min(i => i.Column.DisplayIndex);
            var highCol = cells.Max(i => i.Column.DisplayIndex);

            Console.WriteLine("Row Min: " + lowRow + " Max: " + highRow);
            Console.WriteLine("Col Min: " + lowCol + " Max: " + highCol);

            //dataGrid.PreviewMouseUp -= DataGrid_PreviewMouseUp;

            DataGridCell mCell = GetCell(dataGrid, GetRow(dataGrid, lowRow), lowCol);

            Console.WriteLine("Output: " + ((TextBlock)(mCell.Content)).Text);

        }

        public DataGridRow GetRow(DataGrid grid, int index)
        {
            DataGridRow row = (DataGridRow)grid.ItemContainerGenerator.ContainerFromIndex(index);
            if (row == null)
            {
                // May be virtualized, bring into view and try again.
                grid.UpdateLayout();
                grid.ScrollIntoView(grid.Items[index]);
                row = (DataGridRow)grid.ItemContainerGenerator.ContainerFromIndex(index);
            }
            return row;
        }

        public DataGridCell GetCell(DataGrid grid, DataGridRow row, int column)
        {
            if (row != null)
            {
                DataGridCellsPresenter presenter = GetVisualChild<DataGridCellsPresenter>(row);

                if (presenter == null)
                {
                    grid.ScrollIntoView(row, grid.Columns[column]);
                    presenter = GetVisualChild<DataGridCellsPresenter>(row);
                }

                DataGridCell cell = (DataGridCell)presenter.ItemContainerGenerator.ContainerFromIndex(column);
                return cell;
            }
            return null;
        }
        

        public DataGridCell TryToFindGridCell(DataGrid grid, DataGridCellInfo cellInfo)
        {
            DataGridCell result = null;
            DataGridRow row = (DataGridRow)grid.ItemContainerGenerator.ContainerFromItem(cellInfo.Item);
            if (row != null)
            {
                int columnIndex = grid.Columns.IndexOf(cellInfo.Column);
                if (columnIndex > -1)
                {
                    DataGridCellsPresenter presenter = GetVisualChild<DataGridCellsPresenter>(row);
                    result = presenter.ItemContainerGenerator.ContainerFromIndex(columnIndex) as DataGridCell;
                }
            }
            return result;
        }
        

        public T GetVisualChild<T>(Visual parent) where T : Visual
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

        public string GetColumnName(int index)
        {
            const string letters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";

            var value = "";

            if (index >= letters.Length)
                value += letters[index / letters.Length - 1];

            value += letters[index % letters.Length];

            return value;
        }
    }
}
