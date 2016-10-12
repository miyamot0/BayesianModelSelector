using System.Windows;

namespace BayesianModeling.Dialogs
{
    /// <summary>
    /// Interaction logic for YesNoDialog.xaml
    /// </summary>
    public partial class YesNoDialog : Window
    {
        public string QuestionText
        {
            set { QuestionTextBox.Text = value; }
        }

        public bool ReturnedAnswer { get; set; }

        private void Button_Click_Yes(object sender, RoutedEventArgs e)
        {
            ReturnedAnswer = true;
            DialogResult = true;
        }

        private void Button_Click_No(object sender, RoutedEventArgs e)
        {
            ReturnedAnswer = false;
            DialogResult = true;
        }

        public YesNoDialog()
        {
            InitializeComponent();
            ReturnedAnswer = false;
        }
    }
}
