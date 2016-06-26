using BayesianModeling.ViewModel;
using System.Windows;

namespace BayesianModeling.View
{
    /// <summary>
    /// Interaction logic for UnifiedDiscountingWindow.xaml
    /// </summary>
    public partial class UnifiedDiscountingWindow : Window
    {
        public UnifiedDiscountingWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var viewModel = (ViewModelUnifiedDiscounting)DataContext;

            if (viewModel.ViewLoadedCommand.CanExecute(null))
                viewModel.ViewLoadedCommand.Execute(null);
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            var viewModel = (ViewModelUnifiedDiscounting)DataContext;

            if (viewModel.ViewClosingCommand.CanExecute(null))
                viewModel.ViewClosingCommand.Execute(null);
        }

        private void TextBox_MouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var viewModel = (ViewModelUnifiedDiscounting)DataContext;

            if (viewModel.DelayRangeCommand.CanExecute(null))
                viewModel.DelayRangeCommand.Execute(null);
        }

        private void TextBox_MouseUp_1(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var viewModel = (ViewModelUnifiedDiscounting)DataContext;

            if (viewModel.ValueRangeCommand.CanExecute(null))
                viewModel.ValueRangeCommand.Execute(null);
        }
    }
}
