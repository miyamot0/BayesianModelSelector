/*
 * Shawn Gilroy, 2016
 * Main View Model, initiates core methods, handles IO 
 * and passes information to log viewer.
 * 
 */

using BayesianModeling.Utilities;
using BayesianModeling.View;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.Win32;
using RDotNet;
using Small_N_Stats.Interface;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using static BayesianModeling.Events.PublishSubscribe;

namespace BayesianModeling.ViewModel
{
    class ViewModelMainWindow : ViewModelBase, OutputWindowInterface
    {
        public event PubSubEventHandler<object> OutputEventHandler;
        public event PubSubEventHandler<object> SaveLogsEventHandler;
        public event PubSubEventHandler<object> ClearLogsEventHandler;
        public event PubSubEventHandler<object> PromptForREventHandler;

        public MainWindow MainWindow { get; set; }

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
        public RelayCommand ReogridLicenseWindowCommand { get; set; }
        public RelayCommand NlsLicenseWindowCommand { get; set; }
        public RelayCommand Ggplot2LicenseWindowCommand { get; set; }
        public RelayCommand Reshape2LicenseWindowCommand { get; set; }
        public RelayCommand GridExtraLicenseWindowCommand { get; set; }
        public RelayCommand GnomeIconLicenseWindowCommand { get; set; }

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
            ReogridLicenseWindowCommand = new RelayCommand(param => ReogridLicenseInformationWindow(), param => true);
            NlsLicenseWindowCommand = new RelayCommand(param => NlsLicenseInformationWindow(), param => true);

            Ggplot2LicenseWindowCommand = new RelayCommand(param => Ggplot2LicenseInformationWindow(), param => true);
            Reshape2LicenseWindowCommand = new RelayCommand(param => Reshape2LicenseInformationWindow(), param => true);
            GridExtraLicenseWindowCommand = new RelayCommand(param => GridExtraLicenseInformationWindow(), param => true);
            GnomeIconLicenseWindowCommand = new RelayCommand(param => GnomeIconLicenseInformationWindow(), param => true);

            RowViewModels = new ObservableCollection<RowViewModel>();

            PubSub<object>.AddEvent("OutputEventHandler", OutputEventHandler);
            PubSub<object>.AddEvent("SaveLogsEventHandler", SaveLogsEventHandler);
            PubSub<object>.AddEvent("ClearLogsEventHandler", ClearLogsEventHandler);
            PubSub<object>.AddEvent("PromptForREventHandler", PromptForREventHandler);
        }
        
        #region UI

        public void UpdateTitle(string title)
        {
            Title = "Bayesian Model Selection - " + title;
        }

        // TODO MVVM this

        private void DeleteSelected()
        {
            if (MainWindow.dataGrid.SelectedCells.Count > 0)
            {
                foreach (System.Windows.Controls.DataGridCellInfo obj in MainWindow.dataGrid.SelectedCells)
                {
                    int x = (RowViewModels.IndexOf((RowViewModel)obj.Item));
                    RowViewModels[x].values[obj.Column.DisplayIndex] = "";
                    RowViewModels[x].ForcePropertyUpdate(obj.Column.DisplayIndex);
                }
            }
        }

        #endregion

        #region Licenses

        private void GnomeIconLicenseInformationWindow()
        {
            var window = new View.License();
            window.DataContext = new ViewModelLicense
            {
                licenseTitle = "License - Gnome Icons",
                licenseText = Properties.Resources.License_Gnome_Icons
            };
            window.Show();
        }

        private void NlsLicenseInformationWindow()
        {
            var window = new View.License();
            window.DataContext = new ViewModelLicense
            {
                licenseTitle = "License - NLS",
                licenseText = Properties.Resources.License_NLS
            };
            window.Show();
        }

        private void ReogridLicenseInformationWindow()
        {
            var window = new View.License();
            window.DataContext = new ViewModelLicense
            {
                licenseTitle = "License - Reogrid",
                licenseText = Properties.Resources.License_ReogridSpreadsheet
            };
            window.Show();
        }

        private void RdotNetLicenseInformationWindow()
        {
            var window = new View.License();
            window.DataContext = new ViewModelLicense
            {
                licenseTitle = "License - RdotNet",
                licenseText = Properties.Resources.License_RdotNet
            };
            window.Show();
        }

        private void RLicenseInformationWindow()
        {
            var window = new View.License();
            window.DataContext = new ViewModelLicense
            {
                licenseTitle = "License - R",
                licenseText = Properties.Resources.License_R
            };
            window.Show();
        }

        private void Ggplot2LicenseInformationWindow()
        {
            var window = new View.License();
            window.DataContext = new ViewModelLicense
            {
                licenseTitle = "License - ggplot2",
                licenseText = Properties.Resources.License_ggplot2
            };
            window.Show();
        }

        private void Reshape2LicenseInformationWindow()
        {
            var window = new View.License();
            window.DataContext = new ViewModelLicense
            {
                licenseTitle = "MIT License - reshape2",
                licenseText = Properties.Resources.License_reshape2
            };
            window.Show();
        }

        private void GridExtraLicenseInformationWindow()
        {
            var window = new View.License();
            window.DataContext = new ViewModelLicense
            {
                licenseTitle = "License - gridExtra",
                licenseText = Properties.Resources.License_gridExtra
            };
            window.Show();
        }

        #endregion Licences

        #region Triggers

        private void ViewLoaded()
        {
            ShuttingDown = false;

            for (int i=0; i<RowSpans; i++)
            {
                RowViewModels.Add(new RowViewModel());
            }

            //await TaskEx.Delay(2000);

            if (Properties.Settings.Default.Updated)
            {
                IntroWindow introWindow = new IntroWindow();
                introWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                introWindow.ShowDialog();

                Properties.Settings.Default.Upgrade();
                Properties.Settings.Default.Updated = false;
                Properties.Settings.Default.Save();
            }


            bool failed = false;

            SendMessageToOutput("Welcome to Bayesian discounting model selector!");
            SendMessageToOutput("All view elements loaded");

            SendMessageToOutput("Loading R interop libraries (R.Net.Community)");

            try
            {
                REngine.SetEnvironmentVariables();

                SendMessageToOutput("Displaying R.Net.Community License:");
                SendMessageToOutput("");
                SendMessageToOutput("R.Net.Community version 1.6.5, Copyright 2011-2014 RecycleBin, Copyright 2014-2015 CSIRO");
                SendMessageToOutput("R.Net.Community comes with ABSOLUTELY NO WARRANTY; for details select Information > Licenses > R.Net");
                SendMessageToOutput("This is free software, and you are welcome to redistribute it under certain conditions; see license for details.");
                SendMessageToOutput("");
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
                    SendMessageToOutput("Linking to R (R Statistical Package)");
                    SendMessageToOutput("Displaying R License:");
                    SendMessageToOutput("");

                    /* Interactive post for R */

                    SendMessageToOutput("R Copyright (C) 2016 R Core Team");
                    SendMessageToOutput("This program comes with ABSOLUTELY NO WARRANTY;");
                    SendMessageToOutput("This is free software, and you are welcome to redistribute it");
                    SendMessageToOutput("under certain conditions; for details select Information > Licenses > R.");
                    SendMessageToOutput("");
                    SendMessageToOutput("");

                    /* Loading R packages for analyses */

                    /* Interactive post for nls package */

                    SendMessageToOutput("Package nls found");
                    SendMessageToOutput("Displaying nls License:");
                    SendMessageToOutput("Copyright (C) 1999-1999 Saikat DebRoy, Douglas M. Bates, Jose C. Pinheiro, Copyright (C) 2000-7 The R Core Team");
                    SendMessageToOutput("# File src/library/stats/R/nls.R");
                    SendMessageToOutput("# Part of the R package, http://www.R-project.org");
                    SendMessageToOutput("#");
                    SendMessageToOutput("# Copyright (C) 1999-1999 Saikat DebRoy, Douglas M. Bates, Jose C. Pinheiro");
                    SendMessageToOutput("# Copyright (C) 2000-7    The R Core Team");
                    SendMessageToOutput("#");
                    SendMessageToOutput("# This program is free software; you can redistribute it and/or modify");
                    SendMessageToOutput("# it under the terms of the GNU General Public License as published by");
                    SendMessageToOutput("# the Free Software Foundation; either version 2 of the License, or");
                    SendMessageToOutput("#  (at your option) any later version.");
                    SendMessageToOutput("#");
                    SendMessageToOutput("# This program is distributed in the hope that it will be useful,");
                    SendMessageToOutput("# but WITHOUT ANY WARRANTY; without even the implied warranty of");
                    SendMessageToOutput("# MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the");
                    SendMessageToOutput("# GNU General Public License for more details.");
                    SendMessageToOutput("#");
                    SendMessageToOutput("# A copy of the GNU General Public License is available at");
                    SendMessageToOutput("# http://www.r-project.org/Licenses/");
                    SendMessageToOutput("");
                    SendMessageToOutput("###");
                    SendMessageToOutput("###            Nonlinear least squares for R");
                    SendMessageToOutput("###");
                    SendMessageToOutput("For details select Information > Licenses > ggplot2.");
                    SendMessageToOutput("");
                    SendMessageToOutput("");

                    SendMessageToOutput("Checking for required packages: ");
                    engine.Evaluate("if (!require(ggplot2)) { install.packages('ggplot2', repos = 'http://cran.us.r-project.org') }");

                    /* Interactive post for ggplot2 package */

                    SendMessageToOutput("Package ggplot2 found/loaded");
                    SendMessageToOutput("Displaying ggplot2 License:");
                    SendMessageToOutput("ggplot2 Copyright (C) 2016 Hadley Wickham, Winston Chang");
                    SendMessageToOutput("This program comes with ABSOLUTELY NO WARRANTY;");
                    SendMessageToOutput("This is free software, and you are welcome to redistribute it");
                    SendMessageToOutput("under certain conditions; for details select Information > Licenses > ggplot2.");
                    SendMessageToOutput("");
                    SendMessageToOutput("");

                    /* Interactive post for reshape2 package */

                    engine.Evaluate("if (!require(reshape2)) { install.packages('reshape2', repos = 'http://cran.us.r-project.org') }");
                    SendMessageToOutput("Package reshape2 found/loaded");
                    SendMessageToOutput("Displaying reshape2 License:");
                    SendMessageToOutput("reshape2 Copyright 2008-2014 Hadley Wickham");
                    SendMessageToOutput("reshape2 is released under the MIT license.");
                    SendMessageToOutput("This program comes with ABSOLUTELY NO WARRANTY;");
                    SendMessageToOutput("This is free software, and you are welcome to redistribute it");
                    SendMessageToOutput("under certain conditions; for details select Information > Licenses > reshape2.");
                    SendMessageToOutput("");
                    SendMessageToOutput("");

                    /* Interactive post for grid package */

                    engine.Evaluate("if (!require(grid)) { install.packages('grid', repos = 'http://cran.us.r-project.org') }");

                    /* Interactive post for gridExtra package */

                    engine.Evaluate("if (!require(gridExtra)) { install.packages('gridExtra', repos = 'http://cran.us.r-project.org') }");
                    SendMessageToOutput("Package gridExtra found/loaded");
                    SendMessageToOutput("Displaying gridExtra License:");
                    SendMessageToOutput("gridExtra Copyright (C) 2016 Baptiste Auguie, Anton Antonov");
                    SendMessageToOutput("This program comes with ABSOLUTELY NO WARRANTY;");
                    SendMessageToOutput("This is free software, and you are welcome to redistribute it");
                    SendMessageToOutput("under certain conditions; for details select Information > Licenses > gridExtra.");
                    SendMessageToOutput("");
                    SendMessageToOutput("");

                    SendMessageToOutput("All required packages have been found.  Ready to proceed.");
                }
                else
                {
                    SendMessageToOutput("R DLL's not found.");
                }

                SendMessageToOutput("A listing of all referenced software, with licensing, has been displayed above.");
                SendMessageToOutput("TLDR: Bayesian Model Selector is made possible by the following software.");
                SendMessageToOutput("");
                SendMessageToOutput("R Statistical Package - GPLv2 Licensed. Copyright (C) 2000-16. The R Core Team");
                SendMessageToOutput("nls R Package - GPLv2 Licensed. Copyright (C) 1999-1999 Saikat DebRoy, Douglas M. Bates, Jose C. Pinheiro.");
                SendMessageToOutput("nls R Package - GPLv2 Licensed. Copyright (C) 2000-7. The R Core Team.");
                SendMessageToOutput("ggplot2 R Package - GPLv2 Licensed. Copyright (c) 2016, Hadley Wickham.");
                SendMessageToOutput("reshape2 R Package - MIT Licensed. Copyright (c) 2014, Hadley Wickham.");
                SendMessageToOutput("gridExtra R Package - GPLv2 Licensed. Copyright (c) Baptiste Auguie.");
                SendMessageToOutput("Gnome Icon Set - GPLv2 Licensed.");
                SendMessageToOutput("RdotNet: Interface for the R Statistical Package - New BSD License (BSD 2-Clause). Copyright(c) 2010, RecycleBin. All rights reserved.");
                SendMessageToOutput("");

                SendMessageToOutput("License information is also provided in Information > Licenses > ... as well as");
                SendMessageToOutput("in the install directory of this program (under Resources).");
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
            mWin.DataContext = new ViewModelDiscounting()
            {
                mWindow = MainWindow,
                mInterface = this,
                windowRef = mWin
            };
            mWin.Show();
        }

        private void OpenBatchDiscountingWindow()
        {
            var mWin = new BatchDiscountingWindow();
            mWin.Topmost = true;
            mWin.DataContext = new ViewModelBatchDiscounting()
            {
                mWindow = MainWindow,
                mInterface = this,
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
            RowViewModels.Clear();
            for (int i = 0; i < RowSpans; i++)
            {
                RowViewModels.Add(new RowViewModel());
            }

            UpdateTitle("New File");
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
                    try
                    {
                        OpenXMLHelper.ExportToExcel(new ObservableCollection<RowViewModel>(RowViewModels), saveFileDialog1.FileName);

                        UpdateTitle(saveFileDialog1.SafeFileName);

                        haveFileLoaded = true;
                    }
                    catch (Exception e)
                    {
                        MessageBox.Show("We weren't able to save.  Is the target file open or in use?");
                        Console.WriteLine(e.ToString());
                    }
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
                try
                {
                    OpenXMLHelper.ExportToExcel(new ObservableCollection<RowViewModel>(RowViewModels), saveFileDialog1.FileName);

                    UpdateTitle(saveFileDialog1.SafeFileName);

                    haveFileLoaded = true;

                }
                catch (Exception e)
                {
                    MessageBox.Show("We weren't able to save.  Is the target file open or in use?");
                    Console.WriteLine(e.ToString());
                    haveFileLoaded = false;
                }

            }
        }

        private void SaveFileWithoutDialog()
        {
            if (haveFileLoaded)
            {
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
            }
        }

        private void OpenFile()
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();

            openFileDialog1.Filter = "xlsx Files|*.xlsx";
            openFileDialog1.Title = "Select an Excel File";

            if (openFileDialog1.ShowDialog() == true)
            {
                try
                {
                    using (SpreadsheetDocument spreadSheetDocument = SpreadsheetDocument.Open(@openFileDialog1.FileName, false))
                    {
                        WorkbookPart workbookPart = spreadSheetDocument.WorkbookPart;
                        IEnumerable<Sheet> sheets = spreadSheetDocument.WorkbookPart.Workbook.GetFirstChild<Sheets>().Elements<Sheet>();
                        string relationshipId = sheets.First().Id.Value;
                        WorksheetPart worksheetPart = (WorksheetPart)spreadSheetDocument.WorkbookPart.GetPartById(relationshipId);
                        Worksheet workSheet = worksheetPart.Worksheet;
                        SheetData sheetData = workSheet.GetFirstChild<SheetData>();
                        IEnumerable<Row> rows = sheetData.Descendants<Row>();

                        RowViewModels.Clear();

                        foreach (Row row in rows)
                        {
                            RowViewModel mModel = new RowViewModel();
                            //TODO fix hacky limit
                            for (int i = 1; i < row.Descendants<Cell>().Count() && i < 100; i++)
                            {
                                mModel.values[i-1] = GetCellValue(spreadSheetDocument, row.Descendants<Cell>().ElementAt(i - 1));
                            }

                            RowViewModels.Add(mModel);

                        }

                        UpdateTitle(openFileDialog1.SafeFileName);
                        haveFileLoaded = true;
                    }
                }
                catch (Exception e)
                {
                    MessageBox.Show("We weren't able to open the file.  Is the target file open or in use?");
                    Console.WriteLine(e.ToString());
                }
            }
        }

        public static string GetCellValue(SpreadsheetDocument document, Cell cell)
        {
            SharedStringTablePart stringTablePart = document.WorkbookPart.SharedStringTablePart;
            string value = cell.CellValue.InnerXml;

            if (cell.DataType != null && cell.DataType.Value == CellValues.SharedString)
            {
                return stringTablePart.SharedStringTable.ChildElements[Int32.Parse(value)].InnerText;
            }
            else
            {
                return value;
            }
        }

        private void CloseProgram()
        {
            ShuttingDown = true;
        }

        #endregion FileIO

        #region Pub 'n Sub

        public void SendMessageToOutput(string message)
        {
            PubSub<object>.RaiseEvent("OutputEventHandler", this, new PubSubEventArgs<object>(message));
        }

        private void SaveLogs()
        {
            PubSub<object>.RaiseEvent("SaveLogsEventHandler", this, new PubSubEventArgs<object>(""));
        }

        private void ClearLogs()
        {
            PubSub<object>.RaiseEvent("ClearLogsEventHandler", this, new PubSubEventArgs<object>(""));
        }

        private void PromptRNeeded()
        {
            PubSub<object>.RaiseEvent("PromptForREventHandler", this, new PubSubEventArgs<object>(""));
        }

        #endregion

    }
}
