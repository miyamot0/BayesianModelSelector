using BayesianModeling.Interfaces;
using BayesianModeling.Utilities;
using BayesianModeling.View;
using RDotNet;
using Small_N_Stats.Interface;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System;

namespace BayesianModeling.ViewModel
{
    class ViewModelMainWindow : ViewModelBase, OutputWindowInterface
    {
        public MainWindow MainWindow { get; set; }
        public SpreadsheetInterface _interface { get; set; }

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
        public RelayCommand InformationWindowCommand { get; set; }

        public RelayCommand RLicenseWindowCommand { get; set; }
        public RelayCommand RdotNetLicenseWindowCommand { get; set; }
        public RelayCommand ReogridLicenseWindowCommand { get; set; }
        public RelayCommand NlsLicenseWindowCommand { get; set; }

        /* Output View */

        public RichTextBox mTextBox { get; set; }
        private FlowDocument fd;
        public ScrollViewer sv;

        /* Logic */

        bool haveFileLoaded = false;
        string title = "New File";
        string path = "";

        /* Math/Computation */

        REngine engine;

        public ViewModelMainWindow()
        {
            FileNewCommand = new RelayCommand(param => CreateNewFile(), param => true);
            FileOpenCommand = new RelayCommand(param => OpenFile(), param => true);
            FileSaveCommand = new RelayCommand(param => SaveFile(), param => true);
            FileSaveAsCommand = new RelayCommand(param => SaveFileAs(), param => true);
            FileCloseCommand = new RelayCommand(param => CloseProgram(), param => true);
            FileSaveNoDialogCommand = new RelayCommand(param => SaveFileWithoutDialog(), param => true);
            ViewLoadedCommand = new RelayCommand(param => ViewLoaded(), param => true);
            ViewClosingCommand = new RelayCommand(param => ViewClosed(), param => true);
            DiscountingWindowCommand = new RelayCommand(param => OpenDiscountingWindow(), param => true);
            InformationWindowCommand = new RelayCommand(param => OpenInformationWindow(), param => true);

            RLicenseWindowCommand = new RelayCommand(param => RLicenseInformationWindow(), param => true);
            RdotNetLicenseWindowCommand = new RelayCommand(param => RdotNetLicenseInformationWindow(), param => true);
            ReogridLicenseWindowCommand = new RelayCommand(param => ReogridLicenseInformationWindow(), param => true);
            NlsLicenseWindowCommand = new RelayCommand(param => NlsLicenseInformationWindow(), param => true);
        }

        private void NlsLicenseInformationWindow()
        {
            var window = new License();
            window.DataContext = new ViewModelLicense
            {
                licenseTitle = "License - NLS",
                licenseText = Properties.Resources.License_NLS
            };
            window.Show();
        }

        private void ReogridLicenseInformationWindow()
        {
            var window = new License();
            window.DataContext = new ViewModelLicense
            {
                licenseTitle = "License - Reogrid",
                licenseText = Properties.Resources.License_ReogridSpreadsheet
            };
            window.Show();
        }

        private void RdotNetLicenseInformationWindow()
        {
            var window = new License();
            window.DataContext = new ViewModelLicense
            {
                licenseTitle = "License - RdotNet",
                licenseText = Properties.Resources.License_RdotNet
            };
            window.Show();
        }

        private void RLicenseInformationWindow()
        {
            var window = new License();
            window.DataContext = new ViewModelLicense
            {
                licenseTitle = "License - R",
                licenseText = Properties.Resources.License_R
            };
            window.Show();
        }

        private void ViewLoaded()
        {
            SendMessageToOutput("Loading core modules...");

            REngine.SetEnvironmentVariables();
            engine = REngine.GetInstance();
            engine.Initialize();

            SendMessageToOutput("R interop modules loaded.");

            if (engine.IsRunning)
            {
                SendMessageToOutput("R is found and running");
                SendMessageToOutput("Checking for required packages...");
                engine.Evaluate("if (!require(minpack.lm)) { install.packages('minpack.lm', repos = 'http://cran.us.r-project.org') }");
                SendMessageToOutput("minpack.lm found");
                engine.Evaluate("if (!require(ggplot2)) { install.packages('ggplot2', repos = 'http://cran.us.r-project.org') }");
                SendMessageToOutput("ggplot2 found");
                engine.Evaluate("if (!require(reshape2)) { install.packages('reshape2', repos = 'http://cran.us.r-project.org') }");
                SendMessageToOutput("reshape2 found");
                engine.Evaluate("if (!require(gridExtra)) { install.packages('gridExtra', repos = 'http://cran.us.r-project.org') }");
                SendMessageToOutput("gridExtra found");
                SendMessageToOutput("All required packages have been found.  Ready to proceed.");
            }
            else
            {
                SendMessageToOutput("R DLL's not found.");
                MessageBox.Show("Please download and install the R statistical package to continue.  This is required.");
            }
        }

        private void ViewClosed()
        {
            Properties.Settings.Default.Save();
        }

        private void OpenDiscountingWindow()
        {
            var mWin = new DiscountingWindow();
            mWin.Owner = MainWindow;
            mWin.Topmost = true;
            mWin.DataContext = new ViewModelDiscounting()
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
            mWin.Owner = MainWindow;
            mWin.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            mWin.Topmost = true;
            mWin.Show();
        }

        private void CreateNewFile()
        {
            title = "New File";
            haveFileLoaded = _interface.NewFile();
        }

        private void SaveFile()
        {
            if (haveFileLoaded)
            {
                SaveFileWithoutDialog();
            }
            else
            {
                title = _interface.SaveFileWithDialog(title);

                if (title != null)
                {
                    haveFileLoaded = true;
                }

            }
        }

        private void SaveFileAs()
        {
            title = _interface.SaveFileAs(title);

            if (title != null)
            {
                haveFileLoaded = true;
            }
        }

        private void SaveFileWithoutDialog()
        {
            if (haveFileLoaded)
            {
                _interface.SaveFile(path, title);
            }
        }

        private void OpenFile()
        {
            string[] returned = _interface.OpenFile();

            if (returned != null)
            {
                title = returned[0];
                path = returned[1];

                haveFileLoaded = true;
            }
        }

        private void CloseProgram()
        {
            _interface.ShutDown();
        }

        private void ParagraphReporting(Paragraph results)
        {
            fd = mTextBox.Document;
            fd.Blocks.Add(results);
            mTextBox.ScrollToEnd();
            sv.ScrollToEnd();
        }

        private void CallBack()
        {
            _interface.UpdateTitle(title);
        }

        public void SendMessageToOutput(string message)
        {
            Paragraph para = new Paragraph();
            para.Inlines.Add(message);
            ParagraphReporting(para);
        }
        
    }
}
