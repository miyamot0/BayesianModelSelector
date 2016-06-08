using System.Windows;

namespace BayesianModeling.View
{
    /// <summary>
    /// Interaction logic for InitialLicense.xaml
    /// </summary>
    public partial class InitialLicense : Window
    {
        public InitialLicense()
        {
            InitializeComponent();
        }

        private void agreeButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        private void disagreeButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}
