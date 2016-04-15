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
using System.Windows.Controls;

namespace BayesianModeling.ViewModel
{
    /// <summary>
    /// MVVM-ish Interaction logic for BatchDiscountingWindow.xaml.  
    /// Separation of R-based calculations from View (as much as possible)
    /// </summary>
    class ViewModelBatchDiscounting : ViewModelBase
    {
        public MainWindow mWindow { get; set; }
        public BatchDiscountingWindow windowRef { get; set; }

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

        private string delayedValue = "";
        public string DelayedValue
        {
            get { return delayedValue; }
            set
            {
                delayedValue = value;
                OnPropertyChanged("DelayedValue");
            }
        }

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

        private Brush delayedValueBrush = Brushes.White;
        public Brush DelayedValueBrush
        {
            get { return delayedValueBrush; }
            set
            {
                delayedValueBrush = value;
                OnPropertyChanged("DelayedValueBrush");
            }
        }

        private double MaxValueA = 0;

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
//        public RelayCommand GetDelayedValuesRangeCommand { get; set; }
        public RelayCommand CalculateScoresCommand { get; set; }
        public RelayCommand FigureOutput { get; set; }
        public RelayCommand WorkbookOutput { get; set; }

        /* UI Logic */

        bool failed;

        /// <summary>
        /// Public constructor
        /// </summary>
        public ViewModelBatchDiscounting()
        {
            ViewLoadedCommand = new RelayCommand(param => ViewLoaded(), param => true);
            ViewClosingCommand = new RelayCommand(param => ViewClosed(), param => true);
            GetDelaysRangeCommand = new RelayCommand(param => GetDelaysRange(), param => true);
            GetValuesRangeCommand = new RelayCommand(param => GetValuesRange(), param => true);
//            GetDelayedValuesRangeCommand = new RelayCommand(param => GetDelayedValuesRange(), param => true);
            CalculateScoresCommand = new RelayCommand(param => CalculateScores(), param => true);            
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
            DelayedValue = "1";

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

            if (DelayedValue.Length < 1 || DelayedValue.ToLower().Contains("spreadsheet"))
            {
                DelayedValueBrush = Brushes.LightGray;
                DelayedValue = string.Empty;
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


            if ((highColValue - lowColValue) < 2)
            {
                DefaultFieldsToGray();

                mWindow.dataGrid.PreviewMouseUp -= DataGrid_PreviewMouseUp_Values;

                lowColValue = -1;
                lowRowValue = -1;
                highColValue = -1;
                highRowValue = -1;
                MessageBox.Show("Please select multiple vertical columns of equal length.");

                return;
            }

            for (int i = lowRowValue; i <= highRowValue; i++)
            {
                for (int j = lowColValue; j <= highColValue; j++)
                {
                    DataGridCell mCell = DataGridTools.GetDataGridCell(mWindow.dataGrid, DataGridTools.GetDataGridRow(mWindow.dataGrid, i), j);
                    mCell.Background = Brushes.LightGreen;
                    mCell = null;
                }
            }

            mWindow.dataGrid.PreviewMouseUp -= DataGrid_PreviewMouseUp_Values;

            ValuesBrush = Brushes.LightGreen;
            Values = GetColumnName(lowColValue) + lowRowValue.ToString() + ":" + GetColumnName(highColValue) + highRowValue.ToString();
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
                    for (int j = lowColValue; j <= highColValue; j++)
                    {
                        DataGridCell mCell = DataGridTools.GetDataGridCell(mWindow.dataGrid, DataGridTools.GetDataGridRow(mWindow.dataGrid, i), j);
                        mCell.Background = Brushes.Transparent;
                        mCell.Style = null;
                        mCell = null;
                    }
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
        /// A method for submitting a string-encoded range and returning the value of the cells selected.
        /// </summary>
        /// <param name="range">
        /// List of double values returned for use as delay or value points in Computation
        /// </param>
        public double[,] ParseBulkRange(int lowRowValue, int highRowValue, int lowColValue, int highColValue)
        {
            double[,] mDouble = null;
            DataGridCell mCell;
            double test;

            int mRows = (highRowValue - lowRowValue) + 1;
            int mCols = (highColValue - lowColValue) + 1;

            mDouble = new double[mCols, mRows];

            try
            {

                for (int i = lowRowValue; i <= highRowValue; i++)
                {

                    for (int j = lowColValue; j <= highColValue; j++)
                    {
                        mCell = DataGridTools.GetDataGridCell(mWindow.dataGrid, DataGridTools.GetDataGridRow(mWindow.dataGrid, i), j);

                        if (!Double.TryParse((((TextBlock)mCell.Content)).Text.ToString(), out test))
                        {
                            return null;
                        }
                        else
                        {
                            mDouble[j - lowColValue, i - lowRowValue] = test;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                return null;
            }

            return mDouble;
        }

        /// <summary>
        /// A method for submitting a string-encoded range and returning the value of the cells selected.
        /// </summary>
        /// <param name="range">
        /// List of double values returned for use as delay or value points in Computation
        /// </param>
        public string[,] ParseBulkRangeStrings(int lowRowValue, int highRowValue, int lowColValue, int highColValue)
        {
            string[,] mDouble = null;
            DataGridCell mCell;

            int mRows = (highRowValue - lowRowValue) + 1;
            int mCols = (highColValue - lowColValue) + 1;

            mDouble = new string[mCols, mRows];

            try
            {

                for (int i = lowRowValue; i <= highRowValue; i++)
                {

                    for (int j = lowColValue; j <= highColValue; j++)
                    {
                        mCell = DataGridTools.GetDataGridCell(mWindow.dataGrid, DataGridTools.GetDataGridRow(mWindow.dataGrid, i), j);
                        mDouble[j - lowColValue, i - lowRowValue] = (((TextBlock)mCell.Content)).Text.ToString();
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                return null;
            }

            return mDouble;
        }

        /// <summary>
        /// Command-call to calculate based on supplied ranges and reference values (max value).
        /// Will reference user-selected options (figures, outputs, etc.) throughout calls to R
        /// </summary>
        private void CalculateScores()
        {

            /*
             
            if (xRange.Count != yRange.Count)
            {
                mWindow.OutputEvents("Error while validating current ranges, Delay/Value ranges must be EQUAL in length for comparison.");
                mWindow.OutputEvents("Counts for Delays/Values were " + xRange.Count + " and " + yRange.Count + " respectively.");
                MessageBox.Show("Error while validating current ranges, Delay/Value ranges must be EQUAL in length for comparison.");
                return;
            }
             
             */


            if (failed) return;

            mWindow.OutputEvents("---------------------------------------------------");
            mWindow.OutputEvents("Checking user-supplied ranges and reference points.");

            if (!double.TryParse(DelayedValue, out MaxValueA) || MaxValueA == 0)
            {
                mWindow.OutputEvents("Error while validating the Delayed Amount.  Is this a non-zero number?");
                MessageBox.Show("Please review the the Delayed Amount number.  This must be a non-zero number.");
                return;
            }

            List<double> xRange = new List<double>();
            xRange = GetRangedValues(lowRowDelay, highRowDelay, lowColDelay);

            if (xRange == null)
            {
                mWindow.OutputEvents("Error while validating the Delays.  There cannot be any blank, null or non-numeric fields.");
                MessageBox.Show("Please review the the Delays column.  There cannot be any blank, null or non-numeric fields.");
                return;
            }

            List<double> yRange = new List<double>();

            string[,] wholeRange = ParseBulkRangeStrings(lowRowValue, highRowValue, lowColValue, highColValue);

            if (wholeRange == null)
            {
                mWindow.OutputEvents("There were items that failed validation in the Indifference Point values.  Are any fields blank or not numeric?");
                MessageBox.Show("There were items that failed validation in the Indifference Point values.");
                return;
            }

            List<double> xRangeShadow = new List<double>();
            double holder;

            yRange.Clear();
            xRangeShadow.Clear();

            for (int i = 0; i < wholeRange.GetLength(1); i++)
            {
                if (double.TryParse(wholeRange[0, i], out holder))
                {
                    yRange.Add(holder);
                    xRangeShadow.Add(xRange[i]);
                }
            }

            if ((yRange[0] / MaxValueA) <= 0.1)
            {
                MessageBox.Show("There's a chance your max value is off (the initial value is <10% of the max already).  If this is expected, please disregard.");
                mWindow.OutputEvents("Initial indifference point was <10% of A.  This is irregular, please inspect.  If this is accurate, disregard.");
            }

            if (yRange[0] > MaxValueA)
            {
                MessageBox.Show("Your Delayed Amount appears incorrect (the first Indifference Point is greater than the Delayed Amount).  This shouldn't be possible.");
                mWindow.OutputEvents("Initial indifference point is greater than Delayed Amount.  This shouldn't be possible.  Halting Computation.");
                return;
            }

            mWindow.OutputEvents("All inputs passed verification.");
            mWindow.OutputEvents("---------------------------------------------------");
            mWindow.OutputEvents("Beginning Batched Computations...");

            var mWin = new ResultsWindow();
            var mVM = new ResultsViewModel();
            mWin.DataContext = mVM;
            mWin.Owner = windowRef;
            mWin.Topmost = true;

            for (int i = 0; i < 35; i++)
            {
                mVM.RowViewModels.Add(new RowViewModel());
            }

            for (int mIndex = 0; mIndex < wholeRange.GetLength(0); mIndex++)
            {
                engine.Evaluate("rm(list = setdiff(ls(), lsf.str()))");

                yRange.Clear();
                xRangeShadow.Clear();

                for (int i = 0; i < wholeRange.GetLength(1); i++)
                {

                    if (double.TryParse(wholeRange[mIndex, i], out holder))
                    {
                        yRange.Add(holder);
                        xRangeShadow.Add(xRange[i]);
                    }
                }

                try
                {
                    NumericVector delayValues = engine.CreateNumericVector(xRangeShadow.ToArray());
                    engine.SetSymbol("mDelays", delayValues);

                    List<double> yRangeMod = new List<double>(yRange);

                    for (int i = 0; i < yRangeMod.Count; i++)
                    {
                        yRangeMod[i] = yRange[i] /= MaxValueA;
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

                    if (mIndex == 0)
                    {
                        engine.Evaluate(BayesianModelSelection.GetFranckFunction());
                    }

                    engine.Evaluate("datHack<-data.frame(X = mDelays, Y = mIndiffs, ses=mSes)");
                    engine.Evaluate("output <- BDS(datHack)");
                    engine.Evaluate("ainslieK <- as.numeric(output[[2]]['Mazur.lnk'])");
                    engine.Evaluate("samuelsonK <- as.numeric(output[[3]]['exp.lnk'])");
                    engine.Evaluate("beta <- as.numeric(output[[9]]['BD.beta'])");
                    engine.Evaluate("delta <- as.numeric(output[[9]]['BD.delta'])");
                    engine.Evaluate("myersonK <- as.numeric(output[[4]]['MG.lnk'])");
                    engine.Evaluate("myersonS <- as.numeric(output[[4]]['MG.s'])");
                    engine.Evaluate("rachlinK <- as.numeric(output[[5]]['Rachlin.lnk'])");
                    engine.Evaluate("rachlinS <- as.numeric(output[[5]]['Rachlin.s'])");

                }
                catch (ParseException pe)
                {
                    Console.WriteLine(pe.ToString());
                }

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
                
                mVM.RowViewModels[0].values[mIndex+1] = "Series #" + (int)(mIndex + 1);

                if (mIndex == 0)
                {
                    mVM.RowViewModels[1].values[0] = "Results of Fittings:";
                    mVM.RowViewModels[3].values[0] = "Exponential - ln(k): ";
                    mVM.RowViewModels[5].values[0] = "Hyperbolic - ln(k): ";
                    mVM.RowViewModels[7].values[0] = "Quasi-Hyperbolic - beta: ";
                    mVM.RowViewModels[8].values[0] = "Quasi-Hyperbolic - delta: ";
                    mVM.RowViewModels[10].values[0] = "Myerson-Hyperboloid - ln(k): ";
                    mVM.RowViewModels[11].values[0] = "Myerson-Hyperboloid - s: ";
                    mVM.RowViewModels[13].values[0] = "Rachlin-Hyperboloid - ln(k): ";
                    mVM.RowViewModels[14].values[0] = "Rachlin-Hyperboloid - s): ";
                    mVM.RowViewModels[18].values[0] = "Model Competition (#1)";
                    mVM.RowViewModels[19].values[0] = "#2";
                    mVM.RowViewModels[20].values[0] = "#3";
                    mVM.RowViewModels[21].values[0] = "#4";
                    mVM.RowViewModels[22].values[0] = "#5";
                    mVM.RowViewModels[23].values[0] = "#6";
                    mVM.RowViewModels[25].values[0] = "Most competitive model: ";
                    mVM.RowViewModels[26].values[0] = "ED50 of Most Competitive Model - ln(x): ";
                }
               
                int row = 18;
                foreach (KeyValuePair<string, double> pair in items)
                {
                    mVM.RowViewModels[row].values[mIndex + 1] = pair.Key + " - (" + pair.Value.ToString("0.000") + ")";
                    row++;
                }
                
                mVM.RowViewModels[3].values[mIndex + 1] = engine.Evaluate("as.numeric(output[[3]]['exp.lnk'])").AsVector().First().ToString();
                mVM.RowViewModels[5].values[mIndex + 1] = engine.Evaluate("as.numeric(output[[2]]['Mazur.lnk'])").AsVector().First().ToString();
                mVM.RowViewModels[7].values[mIndex + 1] = engine.Evaluate("as.numeric(output[[9]]['BD.beta'])").AsVector().First().ToString();
                mVM.RowViewModels[8].values[mIndex + 1] = engine.Evaluate("as.numeric(output[[9]]['BD.delta'])").AsVector().First().ToString();
                mVM.RowViewModels[10].values[mIndex + 1] = engine.Evaluate("as.numeric(output[[4]]['MG.lnk'])").AsVector().First().ToString();
                mVM.RowViewModels[11].values[mIndex + 1] = engine.Evaluate("as.numeric(output[[4]]['MG.s'])").AsVector().First().ToString();
                mVM.RowViewModels[13].values[mIndex + 1] = engine.Evaluate("as.numeric(output[[5]]['Rachlin.lnk'])").AsVector().First().ToString();
                mVM.RowViewModels[14].values[mIndex + 1] = engine.Evaluate("as.numeric(output[[5]]['Rachlin.s'])").AsVector().First().ToString();

                double ed50Best = engine.Evaluate("as.numeric(output[[8]]['lnED50.mostprob'])").AsNumeric().First();

                mVM.RowViewModels[25].values[mIndex + 1] = items.First().Key.ToString();
                mVM.RowViewModels[26].values[mIndex + 1] = ed50Best.ToString();

                mWindow.OutputEvents("Computation #" + ((int)mIndex + (int)1) + " of " + wholeRange.GetLength(0) + " Completed!");

            }

            mWindow.OutputEvents("Final Calculations Completed!");
            mWin.Show();
        }
    }
}
