using System;
using System.Threading;
using System.Windows;
using System.Windows.Threading;

namespace BayesianModeling.View
{
    /// <summary>
    /// Interaction logic for ProgressDialog.xaml
    /// </summary>
    public partial class ProgressDialog : Window
    {
        public ProgressDialog(string text, string title)
        {
            InitializeComponent();
            ProgressText.Text = text;
            Title = title;
        }

        public void FinishedWithLoad()
        {
            Dispatcher.Invoke(new Action(() => Thread.Sleep(1000)),  DispatcherPriority.Background);
            Close();
        }

        public void UpdateProgress(int max, int curr)
        {
            ProgressBar.Value = (int) (((double) curr / (double) max)*100);
        }
    }
}
