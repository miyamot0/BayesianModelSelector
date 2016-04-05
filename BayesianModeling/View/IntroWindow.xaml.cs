using System.Diagnostics;
using System.Windows;
using System.Deployment.Application;
using System;

namespace BayesianModeling.View
{
    /// <summary>
    /// Interaction logic for IntroWindow.xaml
    /// </summary>
    public partial class IntroWindow : Window
    {
        public IntroWindow()
        {
            InitializeComponent();
        }

        private void Hyperlink_RequestNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
            e.Handled = true;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (ApplicationDeployment.IsNetworkDeployed)
            {
                Version ver = ApplicationDeployment.CurrentDeployment.CurrentVersion;
                versionLabel.Content = "Version: " + string.Format("{0}.{1}.{2}.{3}", ver.Major, ver.Minor, ver.Build, ver.Revision);
            }
            else
            {
                versionLabel.Content = "Not Published";
            }
        }
    }
}
