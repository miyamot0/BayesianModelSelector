/*
 * Shawn Gilroy, 2016
 * Bayesian Model Selection Application
 * Based on conceptual work developed by Franck, C. T., Koffarnus, M. N., House, L. L. & Bickel, W. (2015)
 */

using BayesianModeling.ViewModel;
using System.Windows;

namespace BayesianModeling
{
    /// <summary>
    /// Interaction logic for App.xaml.  Overrides startup to launch core View/ViewModel
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            /*
             Deviation from MVVM
             Hackish workaround to access Unveil's spreadsheet view
            */
            MainWindow window = new MainWindow();
            window.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            window.DataContext = new ViewModelMainWindow
            {
                _interface = window,
                MainWindow = window,
                mTextBox = window.outputWindow2,
                sv = window.Scroller2
            };
            window.Show();
        }
    }
}
