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
                catch
                {
                    MessageBox.Show("We weren't able to save.  Is the target file open or in use?");
                }
            }
        }
    }
}
