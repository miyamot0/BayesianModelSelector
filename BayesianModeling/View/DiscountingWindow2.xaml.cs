using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace BayesianModeling.View
{
    /// <summary>
    /// Interaction logic for DiscountingWindow2.xaml
    /// </summary>
    public partial class DiscountingWindow2 : Window
    {
        public MainWindowTwo mWindowRef;

        public DiscountingWindow2()
        {
            InitializeComponent();
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            mWindowRef.dataGrid.PreviewMouseUp += DataGrid_PreviewMouseUp;
        }

        private void DataGrid_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            Console.WriteLine("UP ");

            List<DataGridCellInfo> cells = mWindowRef.dataGrid.SelectedCells.ToList();

            var lowRow = cells.Min(i => mWindowRef.GetRowIndex(mWindowRef.dataGrid, i));
            var highRow = cells.Max(i => mWindowRef.GetRowIndex(mWindowRef.dataGrid, i));

            var lowCol = cells.Min(i => i.Column.DisplayIndex);
            var highCol = cells.Max(i => i.Column.DisplayIndex);

            Console.WriteLine("Row Min: " + lowRow + " Max: " + highRow);
            Console.WriteLine("Col Min: " + lowCol + " Max: " + highCol);

            mWindowRef.dataGrid.PreviewMouseUp -= DataGrid_PreviewMouseUp;

            //dataGrid.PreviewMouseUp -= DataGrid_PreviewMouseUp;
            //DataGridCell mCell = GetCell(dataGrid, GetRow(dataGrid, lowRow), lowCol);
            //System.Console.WriteLine("Output: " + ((TextBlock)(mCell.Content)).Text);

        }
    }
}
