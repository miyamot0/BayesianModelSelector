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

    This file uses R.NET Community to leverage interactions with the R program

    ============================================================================

    R.NET Community is distributed under this license:

    Copyright (c) 2010, RecycleBin
    Copyright (c) 2014-2015 CSIRO

    All rights reserved.

    Redistribution and use in source and binary forms, with or without modification, 
    are permitted provided that the following conditions are met:

    * Redistributions of source code must retain the above copyright notice, this list 
    of conditions and the following disclaimer.

    * Redistributions in binary form must reproduce the above copyright notice, this 
    list of conditions and the following disclaimer in the documentation and/or other 
    materials provided with the distribution.

    THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND 
    ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED 
    WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. 
    IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, 
    INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT 
    NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR 
    PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, 
    WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) 
    ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY 
    OF SUCH DAMAGE.

        ============================================================================

    ClosedXML is distributed under this license:

    Copyright (c) 2010 Manuel De Leon

    Permission is hereby granted, free of charge, to any person obtaining a copy of 
    this software and associated documentation files (the "Software"), to deal in the 
    Software without restriction, including without limitation the rights to use, 
    copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the 
    Software, and to permit persons to whom the Software is furnished to do so, 
    subject to the following conditions:

    The above copyright notice and this permission notice shall be included in all 
    copies or substantial portions of the Software.

    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR 
    IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, 
    FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE 
    AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, 
    WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN 
    CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

 */

using BayesianModeling.Utilities;
using BayesianModeling.View;
using ClosedXML.Excel;
using Microsoft.VisualBasic.FileIO;
using Microsoft.Win32;
using RDotNet;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Windows;
using System.Windows.Threading;

namespace BayesianModeling.ViewModel
{
    class ViewModelMainWindow : ViewModelBase
    {
        public MainWindow MainWindow { get; set; }
        Thread loadThread;
        Window window;

        #region Observable Bindings

        public ObservableCollection<RowViewModel> RowViewModels { get; set; }

        public string title = "Bayesian Model Selection - New File";
        public string Title
        {
            get { return title; }
            set
            {
                title = value;
                OnPropertyChanged("Title");
            }
        }

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

        #endregion

        #region Commands

        /* IO Commands */

        public RelayCommand FileNewCommand { get; set; }
        public RelayCommand FileOpenCommand { get; set; }
        public RelayCommand FileSaveCommand { get; set; }
        public RelayCommand FileSaveAsCommand { get; set; }
        public RelayCommand FileCloseCommand { get; set; }
        public RelayCommand FileSaveNoDialogCommand { get; set; }

        /* Loading Commands */

        public RelayCommand ViewLoadedCommand { get; set; }
        public RelayCommand ViewClosingCommand { get; set; }

        /* Menu Commands */

        public RelayCommand DiscountingWindowCommand { get; set; }
        public RelayCommand BatchDiscountingWindowCommand { get; set; }
        public RelayCommand InformationWindowCommand { get; set; }

        public RelayCommand RLicenseWindowCommand { get; set; }
        public RelayCommand RdotNetLicenseWindowCommand { get; set; }
        public RelayCommand NlsLicenseWindowCommand { get; set; }
        public RelayCommand Ggplot2LicenseWindowCommand { get; set; }
        public RelayCommand GridExtraLicenseWindowCommand { get; set; }

        public RelayCommand Reshape2LicenseWindowCommand { get; set; }
        public RelayCommand GnomeIconLicenseWindowCommand { get; set; }
        public RelayCommand BDSLicenseWindowCommand { get; set; }
        public RelayCommand ClosedXMLLicenseWindowCommand { get; set; }

        /* Misc Commands */

        public RelayCommand SaveLogsWindowCommand { get; set; }
        public RelayCommand ClearLogsWindowCommand { get; set; }
        public RelayCommand DeleteSelectedCommand { get; set; }

        #endregion Commands

        #region Logic/Solving

        /* Logic */

        bool haveFileLoaded = false;
        string path = "";
        public static int RowSpans = 50;
        public static int ColSpans = 100;

        /* Math / Computation */

        REngine engine;

        #endregion

        public ViewModelMainWindow()
        {
            FileNewCommand = new RelayCommand(param => CreateNewFile(), param => true);
            FileOpenCommand = new RelayCommand(param => OpenFile(), param => true);
            FileSaveCommand = new RelayCommand(param => SaveFile(), param => true);
            FileSaveAsCommand = new RelayCommand(param => SaveFileAs(), param => true);
            FileCloseCommand = new RelayCommand(param => CloseProgram(), param => true);
            FileSaveNoDialogCommand = new RelayCommand(param => SaveFileWithoutDialog(), param => true);

            SaveLogsWindowCommand = new RelayCommand(param => SaveLogs(), param => true);
            ClearLogsWindowCommand = new RelayCommand(param => ClearLogs(), param => true);

            DeleteSelectedCommand = new RelayCommand(param => DeleteSelected(), param => true);

            ViewLoadedCommand = new RelayCommand(param => ViewLoaded(), param => true);
            ViewClosingCommand = new RelayCommand(param => ViewClosed(), param => true);
            DiscountingWindowCommand = new RelayCommand(param => OpenDiscountingWindow(), param => true);
            BatchDiscountingWindowCommand = new RelayCommand(param => OpenBatchDiscountingWindow(), param => true);
            InformationWindowCommand = new RelayCommand(param => OpenInformationWindow(), param => true);

            RLicenseWindowCommand = new RelayCommand(param => RLicenseInformationWindow(), param => true);
            RdotNetLicenseWindowCommand = new RelayCommand(param => RdotNetLicenseInformationWindow(), param => true);
            NlsLicenseWindowCommand = new RelayCommand(param => NlsLicenseInformationWindow(), param => true);
            Ggplot2LicenseWindowCommand = new RelayCommand(param => Ggplot2LicenseInformationWindow(), param => true);
            GridExtraLicenseWindowCommand = new RelayCommand(param => GridExtraLicenseInformationWindow(), param => true);
            
            Reshape2LicenseWindowCommand = new RelayCommand(param => Reshape2LicenseInformationWindow(), param => true);
            GnomeIconLicenseWindowCommand = new RelayCommand(param => GnomeIconLicenseInformationWindow(), param => true);

            BDSLicenseWindowCommand = new RelayCommand(param => BDSLicenseWindow(), param => true);
            ClosedXMLLicenseWindowCommand = new RelayCommand(param => ClosedXMLLicenseWindow(), param => true);

            RowViewModels = new ObservableCollection<RowViewModel>();

            ObservableCollection<RowViewModel> temp = new ObservableCollection<RowViewModel>();

            for (int i = 0; i < RowSpans; i++)
            {
                temp.Add(new RowViewModel());
            }

            /* Minor speedup, avoids many UI update calls */

            RowViewModels = new ObservableCollection<RowViewModel>(temp);
        }

        #region UI

        public void UpdateTitle(string title)
        {
            Title = title;
        }

        private void DeleteSelected()
        {
            if (MainWindow.dataGrid.SelectedCells.Count > 0)
            {
                foreach (System.Windows.Controls.DataGridCellInfo obj in MainWindow.dataGrid.SelectedCells)
                {
                    var mItem = obj.Item as RowViewModel;

                    if (mItem != null)
                    {
                        int x = RowViewModels.IndexOf(mItem);
                        RowViewModels[x].values[obj.Column.DisplayIndex] = "";
                        RowViewModels[x].ForcePropertyUpdate(obj.Column.DisplayIndex);
                    }
                }
            }
        }

        #endregion

        #region Licenses

        private void GnomeIconLicenseInformationWindow()
        {
            var window = new License();
            window.DataContext = new ViewModelLicense
            {
                licenseTitle = "License (GPLv2) - Gnome Icons",
                licenseText = Properties.Resources.License_Gnome_Icons
            };
            window.Show();
        }

        private void BDSLicenseWindow()
        {
            var window = new License();
            window.DataContext = new ViewModelLicense
            {
                licenseTitle = "License (GPLv2) - BDS Script",
                licenseText = Properties.Resources.License_BDS
            };
            window.Show();
        }

        private void ClosedXMLLicenseWindow()
        {
            var window = new License();
            window.DataContext = new ViewModelLicense
            {
                licenseTitle = "License (MIT) - Closed XML",
                licenseText = Properties.Resources.License_ClosedXML
            };
            window.Show();
        }

        private void NlsLicenseInformationWindow()
        {
            var window = new License();
            window.DataContext = new ViewModelLicense
            {
                licenseTitle = "License (GPLv2) - NLS",
                licenseText = Properties.Resources.License_NLS
            };
            window.Show();
        }

        private void RdotNetLicenseInformationWindow()
        {
            var window = new License();
            window.DataContext = new ViewModelLicense
            {
                licenseTitle = "License (GPLv2) - RdotNet",
                licenseText = Properties.Resources.License_RdotNet
            };
            window.Show();
        }

        private void RLicenseInformationWindow()
        {
            var window = new License();
            window.DataContext = new ViewModelLicense
            {
                licenseTitle = "License (GPLv2+) - R",
                licenseText = Properties.Resources.License_R
            };
            window.Show();
        }

        private void Ggplot2LicenseInformationWindow()
        {
            var window = new License();
            window.DataContext = new ViewModelLicense
            {
                licenseTitle = "License (GPLv2) - ggplot2",
                licenseText = Properties.Resources.License_ggplot2
            };
            window.Show();
        }

        private void GridExtraLicenseInformationWindow()
        {
            var window = new License();
            window.DataContext = new ViewModelLicense
            {
                licenseTitle = "License (GPLv2+) - gridExtra",
                licenseText = Properties.Resources.License_Gridextra
            };
            window.Show();
        }

        private void Reshape2LicenseInformationWindow()
        {
            var window = new License();
            window.DataContext = new ViewModelLicense
            {
                licenseTitle = "License (MIT) - reshape2",
                licenseText = Properties.Resources.License_reshape2
            };
            window.Show();
        }

        #endregion Licences

        #region Triggers

        private void ViewLoaded()
        {
            ShuttingDown = false;

            IntroWindow introWindow = new IntroWindow();
            introWindow.Owner = MainWindow;
            introWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            introWindow.Show();

            bool failed = false;

            SendMessageToOutput("Welcome to Bayesian discounting model selector!");
            SendMessageToOutput("");
            SendMessageToOutput("All view elements loaded");
            SendMessageToOutput("");

            StreamReader licenseFile = new StreamReader(@"LICENSE.txt");

            string line;

            while ((line = licenseFile.ReadLine()) != null)
            {
                SendMessageToOutput(line);
            }

            SendMessageToOutput("Loading R interop libraries (R.Net.Community)");

            try
            {
                REngine.SetEnvironmentVariables();

                SendMessageToOutput("Attempting to link with R installation.");

                engine = REngine.GetInstance();

                SendMessageToOutput("Attempting to Load core binaries...");

                engine.Initialize();
                engine.AutoPrint = false;

            }
            catch (Exception e)
            {
                SendMessageToOutput("R failed to load.  Error code: " + e.ToString());
                failed = true;
            }

            if (failed)
            {
                if (MessageBox.Show("R was not found on your computer.  Do you want to be directed to the R web site for more information?", "R Not Found", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                {
                    Process.Start("https://www.r-project.org/");
                }
            }
            else
            {
                if (engine.IsRunning)
                {
                    SendMessageToOutput("");
                    SendMessageToOutput("R is found and running");

                    engine.Evaluate("if (!require(ggplot2)) { install.packages('ggplot2', repos = 'http://cran.us.r-project.org') }");
                    engine.Evaluate("if (!require(reshape2)) { install.packages('reshape2', repos = 'http://cran.us.r-project.org') }");
                    engine.Evaluate("if (!require(gridExtra)) { install.packages('gridExtra', repos = 'http://cran.us.r-project.org') }");

                    SendMessageToOutput("All required packages have been found.  Ready to proceed.");
                }
                else
                {
                    SendMessageToOutput("R DLL's not found.");
                }

                SendMessageToOutput("");
                SendMessageToOutput("A listing of all referenced software, with licensing, has been displayed above.");
                SendMessageToOutput("TLDR: Bayesian Model Selector is made possible by the following software.");
                SendMessageToOutput("");
                SendMessageToOutput("R Statistical Package - GPL v2 Licensed. Copyright (C) 2000-16. The R Core Team");
                SendMessageToOutput("nls R Package - GPLv2 Licensed. Copyright (C) 1999-1999 Saikat DebRoy, Douglas M. Bates, Jose C. Pinheiro.");
                SendMessageToOutput("nls R Package - GPLv2 Licensed. Copyright (C) 2000-7. The R Core Team.");
                SendMessageToOutput("ggplot2 R Package - GPLv2 Licensed. Copyright (c) 2016, Hadley Wickham.");
                SendMessageToOutput("gridExtra R Package - GPLv2+ Licensed. Copyright (c) 2016, Baptiste Auguie.");
                SendMessageToOutput("reshape2 R Package - MIT Licensed. Copyright (c) 2014, Hadley Wickham.");
                SendMessageToOutput("ClosedXML - MIT Licensed. Copyright (c) 2010 Manuel De Leon.");
                SendMessageToOutput("BDS R Script - GPLv2 Licensed. Copyright (c) 2016, Chris Franck.");
                SendMessageToOutput("Gnome Icon Set - GPLv2 Licensed.");
                SendMessageToOutput("RdotNet: Interface for the R Statistical Package - New BSD License (BSD 2-Clause). Copyright(c) 2010, RecycleBin. All rights reserved.");
                SendMessageToOutput("");

                SendMessageToOutput("License information is also provided in Information > Licenses > ... as well as in the install directory of this program (under Resources).");
            }
        }

        private void ViewClosed()
        {
            Properties.Settings.Default.Save();
            engine.Dispose();
        }

        #endregion Triggers

        #region OpenWindows

        private void OpenDiscountingWindow()
        {
            var mWin = new DiscountingWindow();
            mWin.Topmost = true;
            mWin.Owner = MainWindow;
            mWin.DataContext = new ViewModelDiscounting()
            {
                mWindow = MainWindow,
                windowRef = mWin
            };
            mWin.Show();
        }

        private void OpenBatchDiscountingWindow()
        {
            var mWin = new BatchDiscountingWindow();
            mWin.Topmost = true;
            mWin.Owner = MainWindow;
            mWin.DataContext = new ViewModelBatchDiscounting()
            {
                mWindow = MainWindow,
                windowRef = mWin
            };
            mWin.Show();
        }

        private void OpenInformationWindow()
        {
            var mWin = new InformationWindow();
            mWin.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            mWin.Topmost = true;
            mWin.Show();
        }

        #endregion OpenWindows

        #region FileIO

        private void CreateNewFile()
        {
            loadThread = new Thread(new ThreadStart(ShowFileUIProgressWindow));
            loadThread.SetApartmentState(ApartmentState.STA);
            loadThread.IsBackground = true;
            loadThread.Start();

            RowViewModels.Clear();
            for (int i = 0; i < RowSpans; i++)
            {
                RowViewModels.Add(new RowViewModel());
            }

            UpdateTitle("New File");

            CloseFileUIProgressWindow();
        }

        private void SaveFile()
        {
            if (haveFileLoaded)
            {
                SaveFileWithoutDialog();
            }
            else
            {
                SaveFileDialog saveFileDialog1 = new SaveFileDialog();
                saveFileDialog1.FileName = title;
                saveFileDialog1.Filter = "Excel file (*.xlsx)|*.xlsx|All files (*.*)|*.*";

                if (saveFileDialog1.ShowDialog() == true)
                {
                    loadThread = new Thread(new ThreadStart(ShowFileUIProgressWindow));
                    loadThread.SetApartmentState(ApartmentState.STA);
                    loadThread.IsBackground = true;
                    loadThread.Start();

                    try
                    {
                        OpenXMLHelper.ExportToExcel(new ObservableCollection<RowViewModel>(RowViewModels), saveFileDialog1.FileName);

                        UpdateTitle(saveFileDialog1.SafeFileName);

                        path = Path.GetDirectoryName(saveFileDialog1.FileName);

                        haveFileLoaded = true;
                    }
                    catch (Exception e)
                    {
                        MessageBox.Show("We weren't able to save.  Is the target file open or in use?");
                        Console.WriteLine(e.ToString());
                    }

                    CloseFileUIProgressWindow();
                }
            }
        }

        private void SaveFileAs()
        {
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();

            saveFileDialog1.FileName = title;
            saveFileDialog1.Filter = "Excel file (*.xlsx)|*.xlsx|All files (*.*)|*.*";

            if (saveFileDialog1.ShowDialog() == true)
            {
                loadThread = new Thread(new ThreadStart(ShowFileUIProgressWindow));
                loadThread.SetApartmentState(ApartmentState.STA);
                loadThread.IsBackground = true;
                loadThread.Start();

                try
                {
                    OpenXMLHelper.ExportToExcel(new ObservableCollection<RowViewModel>(RowViewModels), saveFileDialog1.FileName);

                    UpdateTitle(saveFileDialog1.SafeFileName);

                    path = Path.GetDirectoryName(saveFileDialog1.FileName);

                    haveFileLoaded = true;

                }
                catch (Exception e)
                {
                    MessageBox.Show("We weren't able to save.  Is the target file open or in use?");
                    Console.WriteLine(e.ToString());
                    haveFileLoaded = false;
                }

                CloseFileUIProgressWindow();

            }
        }

        private void SaveFileWithoutDialog()
        {
            if (haveFileLoaded)
            {
                loadThread = new Thread(new ThreadStart(ShowFileUIProgressWindow));
                loadThread.SetApartmentState(ApartmentState.STA);
                loadThread.IsBackground = true;
                loadThread.Start();

                try
                {
                    OpenXMLHelper.ExportToExcel(new ObservableCollection<RowViewModel>(RowViewModels), Path.Combine(path, title));

                    UpdateTitle(title);

                }
                catch (Exception e)
                {
                    MessageBox.Show("We weren't able to save.  Is the target file open or in use?");
                    Console.WriteLine(e.ToString());
                }

                CloseFileUIProgressWindow();
            }
        }

        void ShowFileUIProgressWindow()
        {
            window = new ProgressDialog("Processing", "File operations ongoing...");
            window.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            window.Show();
            Dispatcher.Run();
        }

        void CloseFileUIProgressWindow()
        {
            window.Dispatcher.Invoke(DispatcherPriority.Normal, new ThreadStart(window.Close));
        }

        private void OpenFile()
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();

            openFileDialog1.Filter = "Spreadsheet Files (XLSX, CSV)|*.xlsx;*.csv";
            openFileDialog1.Title = "Select an Excel File";

            if (openFileDialog1.ShowDialog() == true)
            {
                loadThread = new Thread(new ThreadStart(ShowFileUIProgressWindow));
                loadThread.SetApartmentState(ApartmentState.STA);
                loadThread.IsBackground = true;
                loadThread.Start();

                string mExt = Path.GetExtension(openFileDialog1.FileName);

                path = Path.GetDirectoryName(openFileDialog1.FileName);

                try
                {
                    if (mExt.Equals(".xlsx"))
                    {

                        using (var wb = new XLWorkbook(@openFileDialog1.FileName))
                        {
                            var ws = wb.Worksheets;
                            var sheet = ws.Worksheet(1);
                            var range = sheet.RangeUsed();
                            var table = range.AsTable();

                            RowViewModels.Clear();

                            foreach (var row in table.Rows())
                            {
                                RowViewModel mModel = new RowViewModel();

                                for (int i = 0; i <= row.CellCount(); i++)
                                {
                                    mModel.values[i] = row.Cell(i).Value.ToString();
                                }

                                RowViewModels.Add(mModel);
                            }

                            UpdateTitle(openFileDialog1.SafeFileName);
                            haveFileLoaded = true;

                        }

                    }
                    else if (mExt.Equals(".csv"))
                    {
                        using (TextFieldParser parser = new TextFieldParser(@openFileDialog1.FileName))
                        {
                            parser.TextFieldType = FieldType.Delimited;
                            parser.SetDelimiters(",");

                            RowViewModels.Clear();

                            while (!parser.EndOfData)
                            {
                                string[] fields = parser.ReadFields();

                                RowViewModel mModel = new RowViewModel();
                                for (int i = 0; i < fields.Length && i < 100; i++)
                                {
                                    mModel.values[i] = fields[i];
                                }
                                RowViewModels.Add(mModel);

                            }

                            UpdateTitle(openFileDialog1.SafeFileName);
                            haveFileLoaded = true;
                        }

                    }

                }
                catch 
                {
                    MessageBox.Show("We weren't able to open the file.  Is the target file open or in use?");
                }

                CloseFileUIProgressWindow();
            }
        }

        private void CloseProgram()
        {
            ShuttingDown = true;
        }

        private void SaveLogs()
        {
            MainWindow.SaveLogs();
        }

        private void ClearLogs()
        {
            MainWindow.ClearLogs();
        }

        #endregion FileIO
        
        public void SendMessageToOutput(string message)
        {
            MainWindow.OutputEvents(message);
        }

    }
}
