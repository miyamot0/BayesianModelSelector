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

    EPPlus is distributed under this license:

    Copyright (c) 2016 Jan Källman

    EPPlus is free software: you can redistribute it and/or modify
    it under the terms of the GNU Library General Public License as published by
    the Free Software Foundation.

    EPPlus is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU Library General Public License
    along with EPPlus.  If not, see <http://epplus.codeplex.com/license>.

    This file uses EPP to leverage interactions with OOXML documents

 */

using BayesianModeling.Utilities;
using BayesianModeling.View;
using Microsoft.VisualBasic.FileIO;
using Microsoft.Win32;
using OfficeOpenXml;
using RDotNet;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace BayesianModeling.ViewModel
{
    class ViewModelMainWindow : ViewModelBase
    {
        public MainWindow MainWindow { get; set; }
        Thread loadThread;
        Window window;

        #region Observable Bindings

        private ObservableCollection<RowViewModel> rowViewModels { get; set; }
        public ObservableCollection<RowViewModel> RowViewModels
        {
            get { return rowViewModels; }
            set
            {
                rowViewModels = value;
                OnPropertyChanged("RowViewModels");
            }
        }

        private ObservableCollection<MenuItem> recentStuff { get; set; }
        public ObservableCollection<MenuItem> RecentStuff
        {
            get { return recentStuff; }
            set
            {
                recentStuff = value;
                OnPropertyChanged("RecentStuff");
            }
        }

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

        #endregion

        #region Commands

        /* IO Commands */

        public RelayCommand FileNewCommand { get; set; }
        public RelayCommand FileOpenCommand { get; set; }
        public RelayCommand FileOpenNoDialogCommand { get; set; }
        public RelayCommand FileSaveCommand { get; set; }
        public RelayCommand FileSaveAsCommand { get; set; }
        public RelayCommand FileCloseCommand { get; set; }
        public RelayCommand FileSaveNoDialogCommand { get; set; }
        public RelayCommand RecentsClearCommand { get; set; }

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
        public RelayCommand ScalesLicenseWindowCommand { get; set; }
        public RelayCommand GnomeIconLicenseWindowCommand { get; set; }
        public RelayCommand BDSLicenseWindowCommand { get; set; }
        public RelayCommand EPPLicenseWindowCommand { get; set; }

        /* Misc Commands */

        public RelayCommand SaveLogsWindowCommand { get; set; }
        public RelayCommand ClearLogsWindowCommand { get; set; }
        public RelayCommand DeleteSelectedCommand { get; set; }
        public RelayCommand CutSelectedCommand { get; set; }

        #endregion Commands

        #region Logic/Solving

        /* Logic */

        bool haveFileLoaded = false;
        bool failed = false;
        string path = "";
        public static int RowSpans = 50;
        public static int ColSpans = 100;
        private string workingSheet = "";
        string[] recentsArray;

        /* Math / Computation */

        REngine engine;

        #endregion

        public ViewModelMainWindow()
        {
            #region FileCommands

            FileNewCommand = new RelayCommand(param => CreateNewFile(), param => true);
            FileOpenCommand = new RelayCommand(param => OpenFile(), param => true);
            FileSaveCommand = new RelayCommand(param => SaveFile(), param => true);
            FileSaveAsCommand = new RelayCommand(param => SaveFileAs(), param => true);
            FileCloseCommand = new RelayCommand(param => CloseProgramWindow(param), param => true);

            FileSaveNoDialogCommand = new RelayCommand(param => SaveFileWithoutDialog(), param => true);
            FileOpenNoDialogCommand = new RelayCommand(param => FileOpenNoDialog(param), param => true);

            RecentsClearCommand = new RelayCommand(param => ClearRecents(), param => true);

            RecentStuff = new ObservableCollection<MenuItem>();

            recentsArray = Properties.Settings.Default.RecentFiles.Trim().Split(';');

            List<string> workingRecents = recentsArray.Select(item => item).Where(item => item.Trim().Length > 1).ToList();

            if (workingRecents != null && workingRecents.Count > 0)
            {
                RecentStuff.Clear();

                foreach (string recentFileLocation in workingRecents)
                {
                    if (recentFileLocation.Trim().Length < 2)
                    {
                        continue;
                    }

                    RecentStuff.Add(new MenuItem
                    {
                        Header = recentFileLocation,
                        Command = FileOpenNoDialogCommand,
                        CommandParameter = recentFileLocation
                    });
                }
            }

            RecentStuff.Add(new MenuItem
            {
                Header = "Clear Recents",
                Command = RecentsClearCommand
            });

            #endregion

            #region LogCommands

            SaveLogsWindowCommand = new RelayCommand(param => SaveLogs(), param => true);
            ClearLogsWindowCommand = new RelayCommand(param => ClearLogs(), param => true);

            #endregion

            #region GridCommands

            DeleteSelectedCommand = new RelayCommand(param => DeleteSelected(), param => true);
            CutSelectedCommand = new RelayCommand(param => CutSelected(), param => true);

            #endregion

            #region TriggerCommands

            ViewLoadedCommand = new RelayCommand(param => ViewLoaded(), param => true);
            ViewClosingCommand = new RelayCommand(param => ViewClosed(), param => true);

            #endregion

            #region UICommands

            DiscountingWindowCommand = new RelayCommand(param => OpenDiscountingWindow(), param => true);
            BatchDiscountingWindowCommand = new RelayCommand(param => OpenBatchDiscountingWindow(), param => true);
            InformationWindowCommand = new RelayCommand(param => OpenInformationWindow(), param => true);

            #endregion

            #region LicenseCommands

            RLicenseWindowCommand = new RelayCommand(param => RLicenseInformationWindow(), param => true);
            RdotNetLicenseWindowCommand = new RelayCommand(param => RdotNetLicenseInformationWindow(), param => true);
            NlsLicenseWindowCommand = new RelayCommand(param => NlsLicenseInformationWindow(), param => true);
            Ggplot2LicenseWindowCommand = new RelayCommand(param => Ggplot2LicenseInformationWindow(), param => true);
            GridExtraLicenseWindowCommand = new RelayCommand(param => GridExtraLicenseInformationWindow(), param => true);            
            Reshape2LicenseWindowCommand = new RelayCommand(param => Reshape2LicenseInformationWindow(), param => true);
            ScalesLicenseWindowCommand = new RelayCommand(param => ScalesLicenseInformationWindow(), param => true);
            
            GnomeIconLicenseWindowCommand = new RelayCommand(param => GnomeIconLicenseInformationWindow(), param => true);
            BDSLicenseWindowCommand = new RelayCommand(param => BDSLicenseWindow(), param => true);
            EPPLicenseWindowCommand = new RelayCommand(param => EPPLicenseWindow(), param => true);

            #endregion

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

        /// <summary>
        /// Clears the recents list, saving a blank string to settings
        /// </summary>
        private void ClearRecents()
        {
            Properties.Settings.Default.RecentFiles = "";
            Properties.Settings.Default.Save();

            RecentStuff.Clear();
            RecentStuff.Add(new MenuItem
            {
                Header = "Clear Recents",
                Command = RecentsClearCommand
            });
        }

        /// <summary>
        /// Adds a recently opened/saved file to the recent lists, if not already present
        /// </summary>
        /// <param name="filePath">
        /// Path to recently opened/saved file
        /// </param>
        private void AddToRecents(string filePath)
        {
            recentsArray = Properties.Settings.Default.RecentFiles.Split(';');

            List<string> workingRecents = recentsArray.Select(item => item).Where(item => item.Trim().Length > 1).ToList();

            if (!workingRecents.Contains(filePath))
            {
                workingRecents.Add(filePath);
                Properties.Settings.Default.RecentFiles = string.Join(";", workingRecents.ToArray());
                Properties.Settings.Default.Save();

                RecentStuff.Clear();

                foreach (string recentFileLocation in workingRecents)
                {
                    if (recentFileLocation.Trim().Length < 2)
                    {
                        continue;
                    }

                    RecentStuff.Add(new MenuItem
                    {
                        Header = recentFileLocation,
                        Command = FileOpenNoDialogCommand,
                        CommandParameter = recentFileLocation
                    });
                }

                RecentStuff.Add(new MenuItem
                {
                    Header = "Clear Recents",
                    Command = RecentsClearCommand
                });
            }
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
        /// Loop through selected/highlighted cells, clear cell contents through bound collections
        /// </summary>
        private void DeleteSelected()
        {
            if (MainWindow.dataGrid.SelectedCells.Count > 0)
            {
                foreach (DataGridCellInfo obj in MainWindow.dataGrid.SelectedCells)
                {
                    var rvm = obj.Item as RowViewModel;

                    if (rvm != null)
                    {
                        int x = RowViewModels.IndexOf(rvm);
                        RowViewModels[x].values[obj.Column.DisplayIndex] = "";
                        RowViewModels[x].ForcePropertyUpdate(obj.Column.DisplayIndex);
                    }
                }
            }
        }

        /// <summary>
        /// Cut cells after copying to clipboard
        /// </summary>
        private void CutSelected()
        {
            if (MainWindow.dataGrid.SelectedCells.Count > 0)
            {
                List<string> holdPreClip = new List<string>();

                foreach (DataGridCellInfo obj in MainWindow.dataGrid.SelectedCells)
                {
                    var rvm = obj.Item as RowViewModel;

                    if (rvm != null)
                    {
                        int x = RowViewModels.IndexOf(rvm);
                        holdPreClip.Add(RowViewModels[x].values[obj.Column.DisplayIndex]);
                    }
                }

                string holdClip = string.Join("\t", holdPreClip);
                Clipboard.SetText(holdClip);

                foreach (DataGridCellInfo obj in MainWindow.dataGrid.SelectedCells)
                {
                    var rvm = obj.Item as RowViewModel;

                    if (rvm != null)
                    {
                        int x = RowViewModels.IndexOf(rvm);
                        RowViewModels[x].values[obj.Column.DisplayIndex] = "";
                        RowViewModels[x].ForcePropertyUpdate(obj.Column.DisplayIndex);
                    }
                }
            }
        }

        #endregion

        #region Licenses

        /// <summary>
        /// License window
        /// </summary>
        private void GnomeIconLicenseInformationWindow()
        {
            var window = new License();
            window.DataContext = new ViewModelLicense
            {
                licenseTitle = "License (GPLv2) - Gnome Icons",
                licenseText = Properties.Resources.License_Gnome_Icons
            };
            window.Owner = MainWindow;
            window.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            window.Show();
        }

        /// <summary>
        /// License window
        /// </summary>
        private void BDSLicenseWindow()
        {
            var window = new License();
            window.DataContext = new ViewModelLicense
            {
                licenseTitle = "License (GPLv2) - BDS Script",
                licenseText = Properties.Resources.License_BDS
            };
            window.Owner = MainWindow;
            window.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            window.Show();
        }

        /// <summary>
        /// License window
        /// </summary>
        private void EPPLicenseWindow()
        {
            var window = new License();
            window.DataContext = new ViewModelLicense
            {
                licenseTitle = "License (LGPL) - EPPlus",
                licenseText = Properties.Resources.License_EPPlus
            };
            window.Owner = MainWindow;
            window.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            window.Show();
        }

        /// <summary>
        /// License window
        /// </summary>
        private void NlsLicenseInformationWindow()
        {
            var window = new License();
            window.DataContext = new ViewModelLicense
            {
                licenseTitle = "License (GPLv2) - NLS",
                licenseText = Properties.Resources.License_NLS
            };
            window.Owner = MainWindow;
            window.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            window.Show();
        }

        /// <summary>
        /// License window
        /// </summary>
        private void RdotNetLicenseInformationWindow()
        {
            var window = new License();
            window.DataContext = new ViewModelLicense
            {
                licenseTitle = "License (GPLv2) - RdotNet",
                licenseText = Properties.Resources.License_RdotNet
            };
            window.Owner = MainWindow;
            window.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            window.Show();
        }

        /// <summary>
        /// License window
        /// </summary>
        private void RLicenseInformationWindow()
        {
            var window = new License();
            window.DataContext = new ViewModelLicense
            {
                licenseTitle = "License (GPLv2+) - R",
                licenseText = Properties.Resources.License_R
            };
            window.Owner = MainWindow;
            window.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            window.Show();
        }

        /// <summary>
        /// License window
        /// </summary>
        private void Ggplot2LicenseInformationWindow()
        {
            var window = new License();
            window.DataContext = new ViewModelLicense
            {
                licenseTitle = "License (GPLv2) - ggplot2",
                licenseText = Properties.Resources.License_ggplot2
            };
            window.Owner = MainWindow;
            window.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            window.Show();
        }

        /// <summary>
        /// License window
        /// </summary>
        private void GridExtraLicenseInformationWindow()
        {
            var window = new License();
            window.DataContext = new ViewModelLicense
            {
                licenseTitle = "License (GPLv2+) - gridExtra",
                licenseText = Properties.Resources.License_Gridextra
            };
            window.Owner = MainWindow;
            window.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            window.Show();
        }

        /// <summary>
        /// License window
        /// </summary>
        private void Reshape2LicenseInformationWindow()
        {
            var window = new License();
            window.DataContext = new ViewModelLicense
            {
                licenseTitle = "License (MIT) - reshape2",
                licenseText = Properties.Resources.License_reshape2
            };
            window.Owner = MainWindow;
            window.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            window.Show();
        }
        
        /// <summary>
        /// License window
        /// </summary>
        private void ScalesLicenseInformationWindow()
        {
            var window = new License();
            window.DataContext = new ViewModelLicense
            {
                licenseTitle = "License (MIT) - scales",
                licenseText = "blank atm"
            };
            window.Owner = MainWindow;
            window.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            window.Show();
        }

        #endregion Licences

        #region Triggers

        /// <summary>
        /// Loaded event trigger
        /// </summary>
        private void ViewLoaded()
        {
            IntroWindow introWindow = new IntroWindow();
            introWindow.Owner = MainWindow;
            introWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            introWindow.Show();

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
                SendMessageToOutput("R Installation not found, please install in order to continue.");
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
                SendMessageToOutput("scales R Package - MIT Licensed. Copyright (c) 2010-2014, Hadley Wickham.");
                SendMessageToOutput("EPPlus - LGPL Licensed. Copyright (c) 2016 Jan Källman.");
                SendMessageToOutput("BDS R Script - GPLv2 Licensed. Copyright (c) 2016, Chris Franck.");
                SendMessageToOutput("Gnome Icon Set - GPLv2 Licensed.");
                SendMessageToOutput("RdotNet: Interface for the R Statistical Package - New BSD License (BSD 2-Clause). Copyright(c) 2010, RecycleBin. All rights reserved.");
                SendMessageToOutput("");

                SendMessageToOutput("License information is also provided in Information > Licenses > ... as well as in the install directory of this program (under Resources).");
            }
        }

        /// <summary>
        /// Closed event trigger
        /// </summary>
        private void ViewClosed()
        {
            Properties.Settings.Default.Save();
            engine.Dispose();
        }

        #endregion Triggers

        #region OpenWindows

        /// <summary>
        /// Single mode analysis window
        /// </summary>
        private void OpenDiscountingWindow()
        {
            if (failed)
            {
                SendMessageToOutput("R Installation not found, please install in order to continue.");
                if (MessageBox.Show("R was not found on your computer.  Do you want to be directed to the R web site for more information?", "R Not Found", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                {
                    Process.Start("https://www.r-project.org/");
                }

                return;
            }

            var mWin = new DiscountingWindow();
            mWin.Owner = MainWindow;
            mWin.DataContext = new ViewModelDiscounting()
            {
                mWindow = MainWindow,
                windowRef = mWin
            };
            mWin.Show();
        }

        /// <summary>
        /// Batch mode analysis window
        /// </summary>
        private void OpenBatchDiscountingWindow()
        {
            if (failed)
            {
                SendMessageToOutput("R Installation not found, please install in order to continue.");
                if (MessageBox.Show("R was not found on your computer.  Do you want to be directed to the R web site for more information?", "R Not Found", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                {
                    Process.Start("https://www.r-project.org/");
                }

                return;
            }

            var mWin = new BatchDiscountingWindow();
            mWin.Owner = MainWindow;
            mWin.DataContext = new ViewModelBatchDiscounting()
            {
                mWindow = MainWindow,
                windowRef = mWin
            };
            mWin.Show();
        }

        /// <summary>
        /// Information window
        /// </summary>
        private void OpenInformationWindow()
        {
            var mWin = new InformationWindow();
            mWin.Owner = MainWindow;
            mWin.Show();
        }

        #endregion OpenWindows

        #region FileIO

        /// <summary>
        /// Creates new spreadsheet, not really "file"
        /// </summary>
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
            workingSheet = "Sheet1";

            haveFileLoaded = false;

            CloseFileUIProgressWindow();
        }

        /// <summary>
        /// Saves file, usually from Ctrl+S binding
        /// </summary>
        private void SaveFile()
        {
            MainWindow.dataGrid.CommitEdit();

            if (haveFileLoaded)
            {
                SaveFileWithoutDialog();
            }
            else
            {
                SaveFileDialog saveFileDialog1 = new SaveFileDialog();
                saveFileDialog1.FileName = title;
                saveFileDialog1.Filter = "Excel file (*.xlsx)|*.xlsx|CSV file (*.csv)|*.csv|All files (*.*)|*.*";

                if (saveFileDialog1.ShowDialog() == true)
                {
                    try
                    {
                        string mExt = Path.GetExtension(saveFileDialog1.FileName);

                        path = Path.GetDirectoryName(saveFileDialog1.FileName);

                        if (mExt.Equals(".xlsx"))
                        {
                            loadThread = new Thread(new ThreadStart(ShowFileUIProgressWindow));
                            loadThread.SetApartmentState(ApartmentState.STA);
                            loadThread.IsBackground = true;
                            loadThread.Start();

                            OpenXMLHelper.ExportToExcel(new ObservableCollection<RowViewModel>(RowViewModels), saveFileDialog1.FileName);

                            CloseFileUIProgressWindow();
                        }
                        else if (mExt.Equals(".csv"))
                        {
                            OpenXMLHelper.ExportToCSV(new ObservableCollection<RowViewModel>(RowViewModels), saveFileDialog1.FileName);
                        }

                        UpdateTitle(saveFileDialog1.SafeFileName);

                        path = Path.GetDirectoryName(saveFileDialog1.FileName);

                        haveFileLoaded = true;
                        
                        AddToRecents(@saveFileDialog1.FileName);
                    }
                    catch (Exception e)
                    {
                        MessageBox.Show("We weren't able to save.  Is the target file either open, missing or in use?");
                        Console.WriteLine(e.ToString());
                    }

                    workingSheet = "Model Selector";
                }
            }
        }

        /// <summary>
        /// Saves file with a dialog 
        /// </summary>
        private void SaveFileAs()
        {
            MainWindow.dataGrid.CommitEdit();

            SaveFileDialog saveFileDialog1 = new SaveFileDialog();

            saveFileDialog1.FileName = title;
            saveFileDialog1.Filter = "Excel file (*.xlsx)|*.xlsx|CSV file (*.csv)|*.csv|All files (*.*)|*.*";

            if (saveFileDialog1.ShowDialog() == true)
            {
                string mExt = Path.GetExtension(saveFileDialog1.FileName);

                path = Path.GetDirectoryName(saveFileDialog1.FileName);

                try
                {
                    if (mExt.Equals(".xlsx"))
                    {
                        loadThread = new Thread(new ThreadStart(ShowFileUIProgressWindow));
                        loadThread.SetApartmentState(ApartmentState.STA);
                        loadThread.IsBackground = true;
                        loadThread.Start();

                        OpenXMLHelper.ExportToExcel(new ObservableCollection<RowViewModel>(RowViewModels), saveFileDialog1.FileName);

                        CloseFileUIProgressWindow();
                    }
                    else if (mExt.Equals(".csv"))
                    {
                        OpenXMLHelper.ExportToCSV(new ObservableCollection<RowViewModel>(RowViewModels), saveFileDialog1.FileName);
                    }

                    workingSheet = "Model Selector";

                    UpdateTitle(saveFileDialog1.SafeFileName);

                    path = Path.GetDirectoryName(saveFileDialog1.FileName);

                    haveFileLoaded = true;

                    AddToRecents(@saveFileDialog1.FileName);
                }
                catch (Exception e)
                {
                    MessageBox.Show("We weren't able to save.  Is the target file either open, missing or in use?");
                    Console.WriteLine(e.ToString());
                    haveFileLoaded = false;
                }

            }
        }

        /// <summary>
        /// Saves file without a dialog call
        /// </summary>
        private void SaveFileWithoutDialog()
        {
            MainWindow.dataGrid.CommitEdit();

            if (haveFileLoaded)
            {
                try
                {
                    string mExt = Path.GetExtension(Path.Combine(path, title));

                    path = Path.GetDirectoryName(Path.Combine(path, title));

                    if (mExt.Equals(".xlsx"))
                    {
                        loadThread = new Thread(new ThreadStart(ShowFileUIProgressWindow));
                        loadThread.SetApartmentState(ApartmentState.STA);
                        loadThread.IsBackground = true;
                        loadThread.Start();

                        OpenXMLHelper.ExportToExcel(new ObservableCollection<RowViewModel>(RowViewModels), Path.Combine(path, title));

                        CloseFileUIProgressWindow();
                    }
                    else if (mExt.Equals(".csv"))
                    {
                        OpenXMLHelper.ExportToCSV(new ObservableCollection<RowViewModel>(RowViewModels), Path.Combine(path, title));
                    }

                    UpdateTitle(title);

                }
                catch (Exception e)
                {
                    MessageBox.Show("We weren't able to save.  Is the target file either open, missing or in use?");
                    Console.WriteLine(e.ToString());
                }
            }
        }

        /// <summary>
        /// Opens a file with a dialog
        /// </summary>
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
                        FileInfo existingFile = new FileInfo(openFileDialog1.FileName);

                        using (ExcelPackage package = new ExcelPackage(existingFile))
                        {
                            var wsMult = package.Workbook.Worksheets;

                            List<string> workSheets = new List<string>();

                            foreach (ExcelWorksheet sheetPeek in wsMult)
                            {
                                workSheets.Add(sheetPeek.Name);
                            }

                            string[] workSheetsArray = workSheets.ToArray();

                            var sheetWindow = new SelectionWindow(workSheetsArray, workSheetsArray[0]);
                            sheetWindow.Title = "Pick a sheet";
                            sheetWindow.MessageLabel.Text = "Select which spreadsheet to load";
                            sheetWindow.Owner = MainWindow;
                            sheetWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                            sheetWindow.Topmost = true;

                            int output = -1;

                            if (sheetWindow.ShowDialog() == true)
                            {
                                output = sheetWindow.MessageOptions.SelectedIndex + 1;

                                workingSheet = workSheetsArray[sheetWindow.MessageOptions.SelectedIndex];
                            }

                            if (output == -1)
                            {
                                return;
                            }

                            var ws = package.Workbook.Worksheets[sheetWindow.MessageOptions.SelectedIndex + 1];

                            RowViewModels.Clear();

                            int currRows = 50;

                            ObservableCollection<RowViewModel> temp = new ObservableCollection<RowViewModel>();
                            for (int i = 0; i < currRows; i++)
                            {
                                temp.Add(new RowViewModel());
                            }

                            var cellsUsed = ws.Cells;

                            foreach (var cell in cellsUsed)
                            {
                                var colStr = DataGridTools.GetColumnIndex(new String(cell.Address.ToCharArray().Where(c => !Char.IsDigit(c)).ToArray()));
                                var rowStr = int.Parse(new String(cell.Address.ToCharArray().Where(c => Char.IsDigit(c)).ToArray()));

                                if (rowStr >= currRows)
                                {
                                    while (currRows < rowStr)
                                    {
                                        temp.Add(new RowViewModel());
                                        currRows++;
                                    }
                                }

                                if (colStr - 1 >= ColSpans)
                                {
                                    continue;
                                }

                                if (cell.Text.Length > 0)
                                {
                                    temp[rowStr - 1].values[colStr] = cell.Text;
                                }
                            }

                            RowViewModels = new ObservableCollection<RowViewModel>(temp);

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

                            workingSheet = "Model Selector";

                            UpdateTitle(openFileDialog1.SafeFileName);
                            haveFileLoaded = true;
                        }

                    }

                    AddToRecents(@openFileDialog1.FileName);
                }
                catch (IOException e)
                {
                    CloseFileUIProgressWindow();
                    Console.WriteLine(e.ToString());
                    MessageBox.Show("We weren't able to save.  Is the target file either open, missing or in use?");
                }
                catch (Exception e)
                {
                    CloseFileUIProgressWindow();
                    Console.WriteLine(e.ToString());
                    MessageBox.Show("We weren't able to save.  Is the target file either open, missing or in use?");
                }

                CloseFileUIProgressWindow();
            }
        }

        /// <summary>
        /// Opens a file without a dialog window
        /// </summary>
        /// <param name="filePath">
        /// path to the file to be opened
        /// </param>
        private void OpenFileNoDialog(string filePath)
        {
            loadThread = new Thread(new ThreadStart(ShowFileUIProgressWindow));
            loadThread.SetApartmentState(ApartmentState.STA);
            loadThread.IsBackground = true;
            loadThread.Start();

            string mExt = Path.GetExtension(@filePath);

            path = Path.GetDirectoryName(@filePath);

            try
            {
                if (mExt.Equals(".xlsx"))
                {
                    FileInfo existingFile = new FileInfo(filePath);

                    using (ExcelPackage package = new ExcelPackage(existingFile))
                    {
                        var wsMult = package.Workbook.Worksheets;

                        List<string> workSheets = new List<string>();

                        foreach (ExcelWorksheet sheetPeek in wsMult)
                        {
                            workSheets.Add(sheetPeek.Name);
                        }

                        string[] workSheetsArray = workSheets.ToArray();

                        var sheetWindow = new SelectionWindow(workSheetsArray, workSheetsArray[0]);
                        sheetWindow.Title = "Pick a sheet";
                        sheetWindow.MessageLabel.Text = "Select which spreadsheet to load";
                        sheetWindow.Owner = MainWindow;
                        sheetWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                        sheetWindow.Topmost = true;

                        int output = -1;

                        if (sheetWindow.ShowDialog() == true)
                        {
                            output = sheetWindow.MessageOptions.SelectedIndex + 1;

                            workingSheet = workSheetsArray[sheetWindow.MessageOptions.SelectedIndex];
                        }

                        if (output == -1)
                        {
                            return;
                        }

                        var ws = package.Workbook.Worksheets[sheetWindow.MessageOptions.SelectedIndex + 1];

                        RowViewModels.Clear();

                        int currRows = 50;

                        ObservableCollection<RowViewModel> temp = new ObservableCollection<RowViewModel>();
                        for (int i = 0; i < currRows; i++)
                        {
                            temp.Add(new RowViewModel());
                        }

                        var cellsUsed = ws.Cells;

                        foreach (var cell in cellsUsed)
                        {
                            var colStr = DataGridTools.GetColumnIndex(new String(cell.Address.ToCharArray().Where(c => !Char.IsDigit(c)).ToArray()));
                            var rowStr = int.Parse(new String(cell.Address.ToCharArray().Where(c => Char.IsDigit(c)).ToArray()));

                            if (rowStr >= currRows)
                            {
                                while (currRows < rowStr)
                                {
                                    temp.Add(new RowViewModel());
                                    currRows++;
                                }
                            }

                            if (colStr - 1 >= ColSpans)
                            {
                                continue;
                            }

                            if (cell.Text.Length > 0)
                            {
                                temp[rowStr - 1].values[colStr] = cell.Text;
                            }

                        }

                        RowViewModels = new ObservableCollection<RowViewModel>(temp);

                        UpdateTitle(existingFile.Name);
                        haveFileLoaded = true;
                    }
                }
                else if (mExt.Equals(".csv"))
                {
                    using (TextFieldParser parser = new TextFieldParser(@filePath))
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

                        workingSheet = "Model Selector";

                        UpdateTitle(Path.GetFileName(@filePath));
                        haveFileLoaded = true;
                    }

                }

                AddToRecents(@filePath);
            }
            catch (IOException e)
            {
                CloseFileUIProgressWindow();
                Console.WriteLine(e.ToString());
                MessageBox.Show("We weren't able to save.  Is the target file either open, missing or in use?");
            }
            catch (Exception e)
            {
                CloseFileUIProgressWindow();
                Console.WriteLine(e.ToString());
                MessageBox.Show("We weren't able to save.  Is the target file either open, missing or in use?");
            }

            CloseFileUIProgressWindow();
        }

        /// <summary>
        /// Method for opening file w/o dialog
        /// </summary>
        /// <param name="param">
        /// Command parameter 
        /// </param>
        private void FileOpenNoDialog(object param)
        {
            string path = param as string;

            if (path != null)
            {
                OpenFileNoDialog(path);
            }
        }

        /// <summary>
        /// Shows progress bar on another thread
        /// </summary>
        void ShowFileUIProgressWindow()
        {
            window = new ProgressDialog("Processing", "File operations ongoing...");
            window.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            window.Show();
            Dispatcher.Run();
        }

        /// <summary>
        /// Closes progress bar on another thread
        /// </summary>
        void CloseFileUIProgressWindow()
        {
            window.Dispatcher.Invoke(DispatcherPriority.Normal, new ThreadStart(window.Close));
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

        #endregion FileIO

        #region Logging

        public void SendMessageToOutput(string message)
        {
            MainWindow.OutputEvents(message);
        }

        private void SaveLogs()
        {
            MainWindow.SaveLogs();
        }

        private void ClearLogs()
        {
            MainWindow.ClearLogs();
        }

        #endregion Logging

    }
}
