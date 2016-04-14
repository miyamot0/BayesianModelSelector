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

using BayesianModeling.Mathematics;
using BayesianModeling.Utilities;
using BayesianModeling.View;
using RDotNet;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace BayesianModeling.ViewModel
{
    /// <summary>
    /// MVVM-ish Interaction logic for DiscountingWindow.xaml.  
    /// Separation of R-based calculations from View (as much as possible)
    /// </summary>
    class ViewModelDiscounting : ViewModelBase
    {
        public MainWindow mWindow { get; set; }
        public DiscountingWindow windowRef { get; set; }

        private string delays = "";
        public string Delays
        {
            get { return delays; }
            set
            {
                delays = value;
                OnPropertyChanged("Delays");
            }
        }

        private string values = "";
        public string Values
        {
            get { return values; }
            set
            {
                values = value;
                OnPropertyChanged("Values");
            }
        }

        private string maxValue = "";
        public string MaxValue
        {
            get { return maxValue; }
            set
            {
                maxValue = value;
                OnPropertyChanged("MaxValue");
            }
        }

        private double maxValueA = 0;

        private Brush delaysBrush = Brushes.White;
        public Brush DelaysBrush
        {
            get { return delaysBrush; }
            set
            {
                delaysBrush = value;
                OnPropertyChanged("DelaysBrush");
            }
        }

        private Brush valuesBrush = Brushes.White;
        public Brush ValuesBrush
        {
            get { return valuesBrush; }
            set
            {
                valuesBrush = value;
                OnPropertyChanged("ValuesBrush");
            }
        }

        int lowRowDelay = 0,
            highRowDelay = 0,
            lowColDelay = 0, 
            highColDelay = 0;

        int lowRowValue = 0,
            highRowValue = 0,
            lowColValue = 0,
            highColValue = 0;

        /* Math/Computation */

        REngine engine;

        /* Commands */

        public RelayCommand ViewLoadedCommand { get; set; }
        public RelayCommand ViewClosingCommand { get; set; }
        public RelayCommand GetDelaysRangeCommand { get; set; }
        public RelayCommand GetValuesRangeCommand { get; set; }
        public RelayCommand CalculateScoresCommand { get; set; }
        public RelayCommand FigureOutput { get; set; }
        public RelayCommand WorkbookOutput { get; set; }

        /* UI Logic */

        private bool outputFigures = false;
        private bool outputWorkbook = false;
        bool failed;

        /// <summary>
        /// Public constructor
        /// </summary>
        public ViewModelDiscounting()
        {
            ViewLoadedCommand = new RelayCommand(param => ViewLoaded(), param => true);
            ViewClosingCommand = new RelayCommand(param => ViewClosed(), param => true);
            GetDelaysRangeCommand = new RelayCommand(param => GetDelaysRange(), param => true);
            GetValuesRangeCommand = new RelayCommand(param => GetValuesRange(), param => true);
            CalculateScoresCommand = new RelayCommand(param => CalculateScores(), param => true);

            FigureOutput = new RelayCommand(param => UpdateFigureOutput(), param => true);
            WorkbookOutput = new RelayCommand(param => UpdateWorkbookOutput(), param => true);
        }

        /// <summary>
        /// Command-based update of UI logic in VM
        /// </summary>
        private void UpdateFigureOutput()
        {
            outputFigures = !outputFigures;
        }

        /// <summary>
        /// Command-based update of UI logic in VM
        /// </summary>
        private void UpdateWorkbookOutput()
        {
            outputWorkbook = !outputWorkbook;
        }

        /// <summary>
        /// Command-based update of UI logic during close.
        /// Will retain window position in Settings.settings
        /// </summary>
        private void ViewClosed()
        {
            Properties.Settings.Default.Save();
        }
        
        /// <summary>
        /// Command-based update of UI logic during open.
        /// Will re-check for R interactivity
        /// </summary>
        private void ViewLoaded()
        {

            mWindow.OutputEvents("---------------------------------------------------");
            failed = false;

            try
            {
                REngine.SetEnvironmentVariables();
                engine = REngine.GetInstance();
                engine.Initialize();
                engine.AutoPrint = false;
            }
            catch (Exception e)
            {
                mWindow.OutputEvents(e.ToString());
                failed = true;
            }

            if (failed)
            {
                mWindow.OutputEvents("R components modules were not found!");
                mWindow.OutputEvents("Calculation cannot continue");
                mWindow.OutputEvents("Connect to the internet and re-start the program");
                mWindow.OutputEvents("");
                mWindow.OutputEvents("");

                MessageBox.Show("Modules for R were not found.  Please connect to the internet and restart the program.");
            }
            else
            {
                mWindow.OutputEvents("All R system components modules loaded.");
                mWindow.OutputEvents("Loading Curve Fitting modules and R interface...");
                mWindow.OutputEvents("");
                mWindow.OutputEvents("");
            }

            DefaultFieldsToGray();
        }

        /// <summary>
        /// Function to update text field background.
        /// Text field background colors as RED indicates the field is actively waiting for select input
        /// </summary>
        private void DefaultFieldsToGray()
        {
            if (Values.Length < 1 || Values.ToLower().Contains("spreadsheet"))
            {
                ValuesBrush = Brushes.LightGray;
                Values = string.Empty;
            }

            if (Delays.Length < 1 || Delays.ToLower().Contains("spreadsheet"))
            {
                DelaysBrush = Brushes.LightGray;
                Delays = string.Empty;
            }
        }

        private void DataGrid_PreviewMouseUp_Delays(object sender, MouseButtonEventArgs e)
        {
            DataGrid grd = e.Source as DataGrid;
            if (grd == null)
                return;

            List<DataGridCellInfo> cells = mWindow.dataGrid.SelectedCells.ToList();

            lowRowDelay = cells.Min(i => DataGridTools.GetDataGridRowIndex(mWindow.dataGrid, i));
            highRowDelay = cells.Max(i => DataGridTools.GetDataGridRowIndex(mWindow.dataGrid, i));

            lowColDelay = cells.Min(i => i.Column.DisplayIndex);
            highColDelay = cells.Max(i => i.Column.DisplayIndex);


            if ((highColDelay - lowColDelay) > 0)
            {
                DefaultFieldsToGray();

                mWindow.dataGrid.PreviewMouseUp -= DataGrid_PreviewMouseUp_Delays;

                lowColDelay = -1;
                lowRowDelay = -1;
                highColDelay = -1;
                highRowDelay = -1;
                MessageBox.Show("Please select a single vertical column.  You can have many rows, but just one column of them.");

                return;
            }
            
            for (int i = lowRowDelay; i <= highRowDelay; i++)
            {
                DataGridCell mCell = DataGridTools.GetDataGridCell(mWindow.dataGrid, DataGridTools.GetDataGridRow(mWindow.dataGrid, i), lowColDelay);
                mCell.Background = Brushes.LightBlue;
                mCell = null;
            }

            mWindow.dataGrid.PreviewMouseUp -= DataGrid_PreviewMouseUp_Delays;

            DelaysBrush = Brushes.LightBlue;
            Delays = GetColumnName(lowColDelay) + lowRowDelay.ToString() + ":" + GetColumnName(highColDelay) + highRowDelay.ToString();
        }

        private void DataGrid_PreviewMouseUp_Values(object sender, MouseButtonEventArgs e)
        {
            DataGrid grd = e.Source as DataGrid;
            if (grd == null)
                return;


            List<DataGridCellInfo> cells = mWindow.dataGrid.SelectedCells.ToList();

            lowRowValue = cells.Min(i => DataGridTools.GetDataGridRowIndex(mWindow.dataGrid, i));
            highRowValue = cells.Max(i => DataGridTools.GetDataGridRowIndex(mWindow.dataGrid, i));

            lowColValue = cells.Min(i => i.Column.DisplayIndex);
            highColValue = cells.Max(i => i.Column.DisplayIndex);


            if ((highColValue - lowColValue) > 0)
            {
                DefaultFieldsToGray();

                mWindow.dataGrid.PreviewMouseUp -= DataGrid_PreviewMouseUp_Values;

                lowColValue = -1;
                lowRowValue = -1;
                highColValue = -1;
                highRowValue = -1;
                MessageBox.Show("Please select a single vertical column.  You can have many rows, but just one column of them.");

                return;
            }
            
            for (int i = lowRowValue; i <= highRowValue; i++)
            {
                DataGridCell mCell = DataGridTools.GetDataGridCell(mWindow.dataGrid, DataGridTools.GetDataGridRow(mWindow.dataGrid, i), lowColValue);
                mCell.Background = Brushes.LightGreen;
                mCell = null;
            }

            mWindow.dataGrid.PreviewMouseUp -= DataGrid_PreviewMouseUp_Values;

            ValuesBrush = Brushes.LightGreen;
            Values = GetColumnName(lowColValue) + lowRowValue.ToString() + ":" + GetColumnName(highColValue) + highRowValue.ToString();
        }

        public string GetColumnName(int index)
        {
            const string letters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";

            var value = "";

            if (index >= letters.Length)
                value += letters[index / letters.Length - 1];

            value += letters[index % letters.Length];

            return value;
        }

        /// <summary>
        /// Call window reference (shameful deviation from MVVM) for Unveil's range PickRange function.
        /// Successful (or failing) selections result in a range string in respective text fields for later parsing.
        /// </summary>
        private void GetDelaysRange()
        {
            DefaultFieldsToGray();

            if (Delays.Length > 0 && !Delays.ToLower().Contains("spreadsheet"))
            {
                for (int i = lowRowDelay; i <= highRowDelay; i++)
                {
                    DataGridCell mCell = DataGridTools.GetDataGridCell(mWindow.dataGrid, DataGridTools.GetDataGridRow(mWindow.dataGrid, i), lowColDelay);
                    mCell.Background = Brushes.Transparent;
                    mCell = null;
                }
            }

            DelaysBrush = Brushes.Yellow;
            Delays = "Select delays on spreadsheet";

            mWindow.dataGrid.PreviewMouseUp += DataGrid_PreviewMouseUp_Delays;

        }

        /// <summary>
        /// Call window reference (shameful deviation from MVVM) for Unveil's range PickRange function.
        /// Successful (or failing) selections result in a range string in respective text fields for later parsing.
        /// </summary>
        private void GetValuesRange()
        {
            DefaultFieldsToGray();

            if (Values.Length > 0 && !Values.ToLower().Contains("spreadsheet"))
            {
                for (int i = lowRowValue; i <= highRowValue; i++)
                {
                    DataGridCell mCell = DataGridTools.GetDataGridCell(mWindow.dataGrid, DataGridTools.GetDataGridRow(mWindow.dataGrid, i), lowColValue);
                    mCell.Background = Brushes.Transparent;
                    mCell = null;
                }
            }

            ValuesBrush = Brushes.Yellow;
            Values = "Select values on spreadsheet";

            mWindow.dataGrid.PreviewMouseUp += DataGrid_PreviewMouseUp_Values;
        }

        private List<double> GetRangedValues(int startRow, int endRow, int column)
        {
            List<double> mRange = new List<double>();

            DataGridCell mCell;
            double test;

            for (int i = startRow; i <= endRow; i++)
            {
                mCell = DataGridTools.GetDataGridCell(mWindow.dataGrid, DataGridTools.GetDataGridRow(mWindow.dataGrid, i), column);

                if (!Double.TryParse((((TextBlock)mCell.Content)).Text.ToString(), out test))
                {
                    return null;
                }
                else
                {
                    mRange.Add(test);
                }
            }

            return mRange;
        }

        /// <summary>
        /// Command-call to calculate based on supplied ranges and reference values (max value).
        /// Will reference user-selected options (figures, outputs, etc.) throughout calls to R
        /// </summary>
        private void CalculateScores()
        {
            if (failed) return;

            List<double> xRange = new List<double>();
            List<double> yRange = new List<double>();

            xRange = GetRangedValues(lowRowDelay, highRowDelay, lowColDelay);
            yRange = GetRangedValues(lowRowValue, highRowValue, lowColValue);

            if (xRange == null || yRange == null) return;

            mWindow.OutputEvents("---------------------------------------------------");
            mWindow.OutputEvents("Checking user-supplied ranges and reference points.");

            if (!double.TryParse(MaxValue, out maxValueA) || maxValueA == 0)
            {
                mWindow.OutputEvents("Error while validating the Delayed Amount.  Is this a non-zero number?");
                MessageBox.Show("Please review the the Delayed Amount number.  This must be a non-zero number.");
                return;
            }

            if (xRange.Count != yRange.Count)
            {
                mWindow.OutputEvents("Error while validating current ranges, Delay/Value ranges must be EQUAL in length for comparison.");
                mWindow.OutputEvents("Counts for Delays/Values were " + xRange.Count + " and " + yRange.Count + " respectively.");
                MessageBox.Show("Error while validating current ranges, Delay/Value ranges must be EQUAL in length for comparison.");
                return;
            }

            if ((yRange[0] / maxValueA) <= 0.1)
            {
                MessageBox.Show("There's a chance your max value is off (the initial value is <10% of the max already).  If this is expected, please disregard.");
                mWindow.OutputEvents("Initial indifference point was <10% of A.  This is irregular, please inspect.  If this is accurate, disregard.");
            }

            if (yRange[0] > maxValueA)
            {
                MessageBox.Show("There's a chance your max value is off (the initial value is greater than the max).  This shouldn't be possible.");
                mWindow.OutputEvents("Initial value is greater than A.  This shouldn't be possible.  Halting Computation.");
                return;
            }

            mWindow.OutputEvents("Inputs passed verification.");
            mWindow.OutputEvents("Figure output: " + outputFigures);
            mWindow.OutputEvents("Workbook output: " + outputWorkbook);
            mWindow.OutputEvents("Beginning Bayesian Computation...");

            try
            {
                engine.Evaluate("rm(list = setdiff(ls(), lsf.str()))");

                NumericVector delayValues = engine.CreateNumericVector(GetRangedValues(lowRowDelay, highRowDelay, lowColDelay).ToArray());
                engine.SetSymbol("mDelays", delayValues);

                List<double> yRangeMod = new List<double>(GetRangedValues(lowRowValue, highRowValue, lowColValue));

                for (int i = 0; i < yRangeMod.Count; i++)
                {
                    yRangeMod[i] = yRange[i] /= maxValueA;
                }

                NumericVector indiffValues = engine.CreateNumericVector(yRangeMod.ToArray());
                engine.SetSymbol("mIndiffs", indiffValues);

                List<double> sValues = new List<double>();
                foreach (double y in yRange)
                {
                    sValues.Add(1);
                }

                NumericVector sesValues = engine.CreateNumericVector(sValues.ToArray());
                engine.SetSymbol("mSes", sesValues);

                engine.Evaluate(BayesianModelSelection.GetFranckFunction());
                engine.Evaluate("datHack<-data.frame(X = mDelays, Y = mIndiffs, ses=mSes)");
                engine.Evaluate("output <- BDS(datHack)");

                DataFrame output = engine.Evaluate("output").AsDataFrame();

                engine.Evaluate("library(ggplot2)");
                engine.Evaluate("library(reshape2)");
                engine.Evaluate("library(gridExtra)");
                engine.Evaluate("ainslieK <- as.numeric(output[[2]]['Mazur.lnk'])");
                engine.Evaluate("samuelsonK <- as.numeric(output[[3]]['exp.lnk'])");
                engine.Evaluate("beta <- as.numeric(output[[9]]['BD.beta'])");
                engine.Evaluate("delta <- as.numeric(output[[9]]['BD.delta'])");
                engine.Evaluate("myersonK <- as.numeric(output[[4]]['MG.lnk'])");
                engine.Evaluate("myersonS <- as.numeric(output[[4]]['MG.s'])");
                engine.Evaluate("rachlinK <- as.numeric(output[[5]]['Rachlin.lnk'])");
                engine.Evaluate("rachlinS <- as.numeric(output[[5]]['Rachlin.s'])");

                NumericVector aSymbol = engine.Evaluate(maxValueA.ToString()).AsNumeric();
                engine.SetSymbol("A", aSymbol);

                engine.SetSymbol("mDelays", delayValues);

                indiffValues = engine.CreateNumericVector(GetRangedValues(lowRowValue, highRowValue, lowColValue));
                engine.SetSymbol("mIndiff", indiffValues);

                engine.Evaluate("endDelay <- max(mDelays)*10");
            }
            catch (ParseException pe)
            {
                Console.WriteLine(pe.ToString());
            }

            mWindow.OutputEvents("Computation successful!");
            mWindow.OutputEvents(" ");

            double noiseProb = double.Parse(engine.Evaluate("as.numeric(output[[1]]['noise.prob'])").AsVector().First().ToString(), System.Globalization.NumberStyles.Float);
            double hyperProb = double.Parse(engine.Evaluate("as.numeric(output[[2]]['Mazur.prob'])").AsVector().First().ToString(), System.Globalization.NumberStyles.Float);
            double exponProb = double.Parse(engine.Evaluate("as.numeric(output[[3]]['exp.prob'])").AsVector().First().ToString(), System.Globalization.NumberStyles.Float);
            double quasiProb = double.Parse(engine.Evaluate("as.numeric(output[[9]]['BD.prob'])").AsVector().First().ToString(), System.Globalization.NumberStyles.Float);
            double myerProb = double.Parse(engine.Evaluate("as.numeric(output[[4]]['MG.prob'])").AsVector().First().ToString(), System.Globalization.NumberStyles.Float);
            double rachProb = double.Parse(engine.Evaluate("as.numeric(output[[5]]['Rachlin.prob'])").AsVector().First().ToString(), System.Globalization.NumberStyles.Float);

            var dictionary = new Dictionary<string, double>();
            dictionary.Add("Noise Model", noiseProb);
            dictionary.Add("Exponential Model", exponProb);
            dictionary.Add("Hyperbolic Model", hyperProb);
            dictionary.Add("Quasi Hyperbolic Model", quasiProb);
            dictionary.Add("Hyperboloid (Myerson) Model", myerProb);
            dictionary.Add("Hyperboloid (Rachlin) Model", rachProb);

            var items = from pair in dictionary orderby pair.Value descending select pair;

            mWindow.OutputEvents("Results of Model competition (output Highest to Lowest):");
            mWindow.OutputEvents(" ");

            foreach (KeyValuePair<string, double> pair in items)
            {
                  mWindow.OutputEvents(pair.Key + ":  " + pair.Value + " ");
            }

            mWindow.OutputEvents(" ");
            mWindow.OutputEvents("Computation Completed");
            mWindow.OutputEvents("---------------------------------------------------");

            if (outputFigures)
            {
                mWindow.OutputEvents("Charting Started... Please wait... ");
                try
                {
                    engine.Evaluate(BayesianModelSelection.GetLogChartFunction());
                }
                catch (Exception e)
                {
                    mWindow.OutputEvents(e.ToString());
                }

                mWindow.OutputEvents("Charting Completed!");
            }

            if (outputWorkbook)
            {
                mWindow.OutputEvents("Outputting to workbook Started... Please wait... ");

                var mWin = new ResultsWindow();
                var mVM = new ResultsViewModel();
                mWin.DataContext = mVM;
                mWin.Owner = windowRef;
                mWin.Topmost = true;

                for (int i=0; i<35; i++)
                {
                    mVM.RowViewModels.Add(new RowViewModel());
                }

                mVM.RowViewModels[0].values[0] = "Results of Bayesian Model Selector";
                mVM.RowViewModels[1].values[0] = "Delays";
                mVM.RowViewModels[2].values[0] = "Values";

                for (int i = 0; i < xRange.Count; i++)
                {
                    mVM.RowViewModels[1].values[1+i] = xRange[i].ToString();
                    mVM.RowViewModels[2].values[1+i] = yRange[i].ToString();
                }

                mVM.RowViewModels[3].values[0] = "Delayed Value";
                mVM.RowViewModels[3].values[1] = MaxValue;

                mVM.RowViewModels[5].values[0] = "Results of Fittings:";

                mVM.RowViewModels[7].values[0] = "Exponential - ln(k): ";
                mVM.RowViewModels[7].values[1] = engine.Evaluate("as.numeric(output[[3]]['exp.lnk'])").AsVector().First().ToString();
                mVM.RowViewModels[9].values[0] = "Hyperbolic - ln(k): ";
                mVM.RowViewModels[9].values[1] = engine.Evaluate("as.numeric(output[[2]]['Mazur.lnk'])").AsVector().First().ToString();
                mVM.RowViewModels[11].values[0] = "Quasi-Hyperbolic (beta): ";
                mVM.RowViewModels[11].values[1] = engine.Evaluate("as.numeric(output[[9]]['BD.beta'])").AsVector().First().ToString();
                mVM.RowViewModels[12].values[0] = "Quasi-Hyperbolic (delta): ";
                mVM.RowViewModels[12].values[1] = engine.Evaluate("as.numeric(output[[9]]['BD.delta'])").AsVector().First().ToString();
                mVM.RowViewModels[14].values[0] = "Myerson-Hyperboloid - ln(k): ";
                mVM.RowViewModels[14].values[1] = engine.Evaluate("as.numeric(output[[4]]['MG.lnk'])").AsVector().First().ToString();
                mVM.RowViewModels[15].values[0] = "Myerson-Hyperboloid (s): ";
                mVM.RowViewModels[15].values[1] = engine.Evaluate("as.numeric(output[[4]]['MG.s'])").AsVector().First().ToString();
                mVM.RowViewModels[17].values[0] = "Rachlin-Hyperboloid (k): ";
                mVM.RowViewModels[17].values[1] = engine.Evaluate("as.numeric(output[[5]]['Rachlin.lnk'])").AsVector().First().ToString();
                mVM.RowViewModels[18].values[0] = "Rachlin-Hyperboloid (s): ";
                mVM.RowViewModels[18].values[1] = engine.Evaluate("as.numeric(output[[5]]['Rachlin.s'])").AsVector().First().ToString();

                mVM.RowViewModels[21].values[0] = "Model Competition (Best to Worst)";

                string left = string.Empty;
                string right = string.Empty;
                int row = 22;

                foreach (KeyValuePair<string, double> pair in items)
                {
                    mVM.RowViewModels[row].values[0] = pair.Key;
                    mVM.RowViewModels[row].values[1] = pair.Value.ToString();
                    row++;
                }

                mVM.RowViewModels[29].values[0] = "Most competitive model: ";
                mVM.RowViewModels[29].values[1] = items.First().Key.ToString();

                double ed50Best = engine.Evaluate("as.numeric(output[[8]]['lnED50.mostprob'])").AsNumeric().First();

                mVM.RowViewModels[30].values[0] = "ED50 of Most Competitive Model - ln(x): ";
                mVM.RowViewModels[30].values[1] = ed50Best.ToString();

                mWin.Show();

                mWindow.OutputEvents("Output Completed!");
            }

        }
    }
}
