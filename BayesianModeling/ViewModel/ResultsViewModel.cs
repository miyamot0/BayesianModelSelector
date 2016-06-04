//----------------------------------------------------------------------------------------------
// <copyright file="ResultsViewModel.cs" 
// Copyright 2016 Shawn Gilroy
//
// This file is part of Bayesian Model Selector.
//
// Bayesian Model Selector is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, version 2.
//
// Bayesian Model Selector is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with Bayesian Model Selector.  If not, see http://www.gnu.org/licenses/. 
// </copyright>
//
// <summary>
// The Bayesian Model Selector is a tool to assist researchers in behavior economics.
// 
// Email: shawn(dot)gilroy(at)temple.edu
//
// Bayesian Model Selector utilizes EPPlus to leverage interactions with XML file formats
//
//    EPPlus is distributed under the GPL license, version 2:
//
//    Copyright (c) 2016 Jan Källman
//
//    EPPlus is free software: you can redistribute it and/or modify
//    it under the terms of the GNU General Public License as published by
//    the Free Software Foundation, version 2.
//
//    EPPlus is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//    GNU General Public License for more details.
//
//    You should have received a copy of the GNU General Public License
//    along with EPPlus.  If not, see <http://epplus.codeplex.com/license>.
//
// </summary>
//----------------------------------------------------------------------------------------------

using BayesianModeling.Utilities;
using Microsoft.Win32;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;

namespace BayesianModeling.ViewModel
{
    class ResultsViewModel : ViewModelBase
    {
        
        public ObservableCollection<RowViewModel> RowViewModels { get; set; }

        /* IO Commands */
        
        public RelayCommand FileSaveCommand { get; set; }
        public RelayCommand FileCloseCommand { get; set; }

        public ResultsViewModel()
        {
            RowViewModels = new ObservableCollection<RowViewModel>();

            FileSaveCommand = new RelayCommand(param => SaveFile(), param => true);
            FileCloseCommand = new RelayCommand(param => CloseProgramWindow(param), param => true);
        }

        /// <summary>
        /// Save the current row's to file
        /// </summary>
        private void SaveFile()
        {
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.FileName = "Results";
            saveFileDialog1.Filter = "Excel file (*.xlsx)|*.xlsx|CSV file (*.csv)|*.csv|All files (*.*)|*.*";

            if (saveFileDialog1.ShowDialog() == true)
            {
                try
                {
                    string mExt = Path.GetExtension(saveFileDialog1.FileName);

                    if (mExt.Equals(".xlsx"))
                    {
                        OpenXMLHelper.ExportToExcel(new ObservableCollection<RowViewModel>(RowViewModels), saveFileDialog1.FileName);
                    }
                    else if (mExt.Equals(".csv"))
                    {
                        OpenXMLHelper.ExportToCSV(new ObservableCollection<RowViewModel>(RowViewModels), saveFileDialog1.FileName);
                    }
                }
                catch (Exception e)
                {
                    MessageBox.Show("We weren't able to save.  Is the target file either open, missing or in use?");
                    Logging.SubmitError("SaveFile", e.ToString());
                    Console.WriteLine(e.ToString());
                }
            }
        }

        /// <summary>
        /// Shutdown command, receiving window as command parameter
        /// </summary>
        /// <param name="param"></param>
        private void CloseProgramWindow(object param)
        {
            var windowObj = param as Window;

            if (windowObj != null)
            {
                windowObj.Close();
            }
        }
    }
}
