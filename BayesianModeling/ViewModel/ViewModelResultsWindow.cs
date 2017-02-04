//----------------------------------------------------------------------------------------------
// <copyright file="ViewModelMainWindow.cs" 
// Copyright 2016 Shawn Gilroy
//
// This file is part of Discounting Model Selector.
//
// Discounting Model Selector is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, version 2.
//
// Discounting Model Selector is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with Discounting Model Selector.  If not, see http://www.gnu.org/licenses/. 
// </copyright>
//
// <summary>
// The Discounting Model Selector is a tool to assist researchers in behavior economics.
// 
// Email: shawn(dot)gilroy(at)temple.edu
//
// Discounting Model Selector utilizes R.Net Community to communicate with the R program
//
//    R.NET Community is distributed under this license:
//    
//    Copyright(c) 2010, RecycleBin
//    Copyright(c) 2014-2015 CSIRO
//    
//    All rights reserved.
//    
//    Redistribution and use in source and binary forms, with or without modification, 
//    are permitted provided that the following conditions are met:
//
//    Redistributions of source code must retain the above copyright notice, this list
//    of conditions and the following disclaimer.
//
//    Redistributions in binary form must reproduce the above copyright notice, this 
//    list of conditions and the following disclaimer in the documentation and/or other
//    materials provided with the distribution.
//
//    THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
//    ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
//    WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. 
//    IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, 
//    INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT
//    NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR
//    PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, 
//    WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) 
//    ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY
//    OF SUCH DAMAGE.
//
// Discounting Model Selector utilizes Reogrid to leverage to load, save, and display data
//
//    Reogrid is distributed under this license:
//
//    MIT License
//    
//    Copyright(c) 2013-2016 Jing<lujing at unvell.com>
//    Copyright(c) 2013-2016 unvell.com, All rights reserved.
//    
//    Permission is hereby granted, free of charge, to any person obtaining a copy
//    of this software and associated documentation files (the "Software"), to deal
//    in the Software without restriction, including without limitation the rights
//    to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//    copies of the Software, and to permit persons to whom the Software is
//    furnished to do so, subject to the following conditions:
//    
//    The above copyright notice and this permission notice shall be included in all
//    copies or substantial portions of the Software.
//    
//    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//    IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//    FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//    AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//    LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//    OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
//    SOFTWARE.
//
// </summary>
//----------------------------------------------------------------------------------------------

using BayesianModeling.Utilities;
using Microsoft.Win32;
using System.IO;
using System.Windows;
using unvell.ReoGrid;

namespace BayesianModeling.ViewModel
{
    class ViewModelResultsWindow : ViewModelBase
    {
        #region Observable Bindings

        /// <summary>
        /// Bindable title
        /// </summary>
        public string title = "Discounting Model Selection - Results";
        public string Title
        {
            get { return title; }
            set
            {
                title = value;
                OnPropertyChanged("Title");
            }
        }

        /// <summary>
        /// Bindable workbook
        /// </summary>
        private ReoGridControl resultsBook;
        public ReoGridControl ResultsBook
        {
            get
            {
                return resultsBook;
            }
            set
            {
                resultsBook = value;
                OnPropertyChanged("ResultsBook");
            }
        }

        #endregion

        #region Commands

        /* IO Commands */

        public RelayCommand ResultsFileSaveCommand { get; set; }
        public RelayCommand ResultsFileSaveAsCommand { get; set; }
        public RelayCommand ResultsFileCloseCommand { get; set; }

        #endregion Commands

        /// <summary>
        /// Constructor
        /// </summary>
        public ViewModelResultsWindow()
        {
            #region FileCommands

            ResultsFileSaveCommand = new RelayCommand(param => SaveFile(), param => true);
            ResultsFileSaveAsCommand = new RelayCommand(param => SaveFileAs(), param => true);
            ResultsFileCloseCommand = new RelayCommand(param => CloseProgramWindow(param), param => true);

            #endregion
        }

        /// <summary>
        /// Update window title through bound object
        /// </summary>
        /// <param name="title">
        /// File name to be used in title (string)
        /// </param>
        public void UpdateTitle(string title)
        {
            Title = title;
        }

        /// <summary>
        /// Saves file, usually from Ctrl+S binding
        /// </summary>
        private void SaveFile()
        {
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.FileName = title;
            saveFileDialog1.Filter = "Excel file (*.xlsx)|*.xlsx|CSV file (*.csv)|*.csv|All files (*.*)|*.*";

            if (saveFileDialog1.ShowDialog() == true)
            {
                try
                {
                    string mExt = Path.GetExtension(saveFileDialog1.FileName);

                    if (mExt.Equals(".xlsx"))
                    {
                        ResultsBook.Save(saveFileDialog1.FileName, unvell.ReoGrid.IO.FileFormat.Excel2007);
                    }
                    else if (mExt.Equals(".csv"))
                    {
                        ResultsBook.Save(saveFileDialog1.FileName, unvell.ReoGrid.IO.FileFormat.CSV);
                    }
                    else
                    {
                        return;
                    }

                    UpdateTitle(saveFileDialog1.SafeFileName);
                }
                catch
                {
                    MessageBox.Show("We weren't able to save.  Is the target file either open, missing or in use?");
                }
            }
        }

        /// <summary>
        /// Saves file with a dialog 
        /// </summary>
        private void SaveFileAs()
        {
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();

            saveFileDialog1.FileName = title;
            saveFileDialog1.Filter = "Excel file (*.xlsx)|*.xlsx|CSV file (*.csv)|*.csv|All files (*.*)|*.*";

            if (saveFileDialog1.ShowDialog() == true)
            {
                string mExt = Path.GetExtension(saveFileDialog1.FileName);

                try
                {
                    if (mExt.Equals(".xlsx"))
                    {
                        ResultsBook.Save(saveFileDialog1.FileName, unvell.ReoGrid.IO.FileFormat.Excel2007);
                    }
                    else if (mExt.Equals(".csv"))
                    {
                        ResultsBook.Save(saveFileDialog1.FileName, unvell.ReoGrid.IO.FileFormat.CSV);
                    }
                    else
                    {
                        return;
                    }

                    UpdateTitle(saveFileDialog1.SafeFileName);
                }
                catch
                {
                    MessageBox.Show("We weren't able to save.  Is the target file either open, missing or in use?");
                }
            }
        }

        /// <summary>
        /// Shutdown event
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
