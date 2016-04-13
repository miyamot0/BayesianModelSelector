using BayesianModeling.Utilities;
using Microsoft.Win32;
using System;
using System.Collections.ObjectModel;
using System.Windows;

namespace BayesianModeling.ViewModel
{
    class ResultsViewModel : ViewModelBase
    {

        public RelayCommand FileSaveCommand { get; set; }
        public RelayCommand FileCloseCommand { get; set; }

        public bool shuttingDown = false;
        public bool ShuttingDown
        {
            get { return shuttingDown; }
            set
            {
                shuttingDown = value;
                OnPropertyChanged("ShuttingDown");
            }
        }

        public ObservableCollection<RowViewModel> RowViewModels { get; set; }
        
        public ResultsViewModel()
        {
            RowViewModels = new ObservableCollection<RowViewModel>();

            FileSaveCommand = new RelayCommand(param => SaveFile(), param => true);
            FileCloseCommand = new RelayCommand(param => CloseProgram(), param => true);

            ShuttingDown = false;
        }

        private void CloseProgram()
        {
            ShuttingDown = true;
        }

        private void SaveFile()
        {
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.FileName = "Results";
            saveFileDialog1.Filter = "Excel file (*.xlsx)|*.xlsx|All files (*.*)|*.*";

            if (saveFileDialog1.ShowDialog() == true)
            {
                try
                {
                    OpenXMLHelper.ExportToExcel(new ObservableCollection<RowViewModel>(RowViewModels), saveFileDialog1.FileName);
                }
                catch (Exception e)
                {
                    MessageBox.Show("We weren't able to save.  Is the target file open or in use?");
                    Console.WriteLine(e.ToString());
                }
            }
        }
    }
}
