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
using System.Collections.ObjectModel;

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

        private bool advancedMenu = false;
        public bool AdvancedMenu
        {
            get { return advancedMenu; }
            set
            {
                advancedMenu = value;
                OnPropertyChanged("AdvancedMenu");
            }
        }

        private bool noiseModel = true;
        public bool NoiseModel
        {
            get { return noiseModel; }
            set
            {
                noiseModel = value;
                OnPropertyChanged("NoiseModel");
            }
        }

        private bool exponentialModel = true;
        public bool ExponentialModel
        {
            get { return exponentialModel; }
            set
            {
                exponentialModel = value;
                OnPropertyChanged("ExponentialModel");
            }
        }

        private bool hyperbolicModel = true;
        public bool HyperbolicModel
        {
            get { return hyperbolicModel; }
            set
            {
                hyperbolicModel = value;
                OnPropertyChanged("HyperbolicModel");
            }
        }

        private bool quasiHyperbolicModel = true;
        public bool QuasiHyperbolicModel
        {
            get { return quasiHyperbolicModel; }
            set
            {
                quasiHyperbolicModel = value;
                OnPropertyChanged("QuasiHyperbolicModel");
            }
        }

        private bool myerHyperboloidModel = true;
        public bool MyerHyperboloidModel
        {
            get { return myerHyperboloidModel; }
            set
            {
                myerHyperboloidModel = value;
                OnPropertyChanged("MyerHyperboloidModel");
            }
        }

        private bool rachHyperboloidModel = true;
        public bool RachHyperboloidModel
        {
            get { return rachHyperboloidModel; }
            set
            {
                rachHyperboloidModel = value;
                OnPropertyChanged("RachHyperboloidModel");
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

        int lowRowDelay = -1,
            highRowDelay = -1,
            lowColDelay = -1,
            highColDelay = -1;

        int lowRowValue = -1,
            highRowValue = -1,
            lowColValue = -1,
            highColValue = -1;

        /* Math/Computation */

        REngine engine;

        /* Commands */

        public RelayCommand ViewLoadedCommand { get; set; }
        public RelayCommand ViewClosingCommand { get; set; }

        public RelayCommand DelayRangeCommand { get; set; }
        public RelayCommand ValueRangeCommand { get; set; }

        public RelayCommand GetDelaysRangeCommand { get; set; }
        public RelayCommand GetValuesRangeCommand { get; set; }
        public RelayCommand CalculateScoresCommand { get; set; }
        public RelayCommand FigureOutput { get; set; }
        public RelayCommand WorkbookOutput { get; set; }

        public RelayCommand AdvancedSettings { get; set; }

        public RelayCommand NoiseModelCommand { get; set; }
        public RelayCommand ExponentialModelCommand { get; set; }
        public RelayCommand HyperbolicModelCommand { get; set; }
        public RelayCommand QuasiHyperbolicModelCommand { get; set; }
        public RelayCommand MyersonHyperboloidModelCommand { get; set; }
        public RelayCommand RachlinHyperboloidModelCommand { get; set; }

        /* UI Logic */

        bool failed; 

        /// <summary>
        /// Public constructor
        /// </summary>
        public ViewModelBatchDiscounting()
        {
            NoiseModel = HyperbolicModel = ExponentialModel = QuasiHyperbolicModel = MyerHyperboloidModel = RachHyperboloidModel = true;

            ViewLoadedCommand = new RelayCommand(param => ViewLoaded(), param => true);
            ViewClosingCommand = new RelayCommand(param => ViewClosed(), param => true);
            GetDelaysRangeCommand = new RelayCommand(param => GetDelaysRange(), param => true);
            GetValuesRangeCommand = new RelayCommand(param => GetValuesRange(), param => true);
            CalculateScoresCommand = new RelayCommand(param => CalculateScores(), param => true);

            AdvancedSettings = new RelayCommand(param => UpdateSettings(), param => true);

            DelayRangeCommand = new RelayCommand(param => GetCustomDelays(), param => true);
            ValueRangeCommand = new RelayCommand(param => GetCustomValues(), param => true);

            NoiseModelCommand = new RelayCommand(param => UpdateNoise(), param => true);
            ExponentialModelCommand = new RelayCommand(param => UpdateExponential(), param => true);
            HyperbolicModelCommand = new RelayCommand(param => UpdateHyperbolic(), param => true);
            QuasiHyperbolicModelCommand = new RelayCommand(param => UpdateQuasiHyperbolic(), param => true);
            MyersonHyperboloidModelCommand = new RelayCommand(param => UpdateMyersonHyperboloid(), param => true);
            RachlinHyperboloidModelCommand = new RelayCommand(param => UpdateRachlinHyperboloid(), param => true);
        }

        /// <summary>
        /// Query user for a range
        /// </summary>
        private void GetCustomDelays()
        {
            var mWin = new RangePrompt();
            mWin.Topmost = true;
            mWin.Owner = windowRef;
            mWin.WindowStartupLocation = WindowStartupLocation.CenterOwner;

            if (mWin.ShowDialog() == true)
            {
                string[] addresses = mWin.ResponseText.Split(':');

                if (addresses.Length != 2) return;

                var firstChars = new String(addresses[0].ToCharArray().Where(c => !Char.IsDigit(c)).ToArray());
                var firstNums = new String(addresses[0].ToCharArray().Where(c => Char.IsDigit(c)).ToArray());

                var secondChars = new String(addresses[1].ToCharArray().Where(c => !Char.IsDigit(c)).ToArray());
                var secondNums = new String(addresses[1].ToCharArray().Where(c => Char.IsDigit(c)).ToArray());

                int fNum, sNum;

                if (int.TryParse(firstNums, out fNum) && int.TryParse(secondNums, out sNum) && firstChars.Length > 0 && secondChars.Length > 0)
                {
                    if ((sNum - fNum) == 0)
                    {
                        DelaysBrush = Brushes.LightBlue;
                        Delays = firstChars + firstNums + ":" + secondChars + secondNums;

                        lowColDelay = DataGridTools.GetColumnIndex(firstChars);
                        highColDelay = DataGridTools.GetColumnIndex(secondChars);

                        lowRowDelay = fNum;
                        highRowDelay = sNum;
                    }
                    else
                    {
                        MessageBox.Show("Please ensure that only a single row is selected");
                    }
                }
                else
                {
                    MessageBox.Show("Parse error!");
                }
            }
        }

        /// <summary>
        /// Query user for a range
        /// </summary>
        private void GetCustomValues()
        {
            var mWin = new RangePrompt();
            mWin.Topmost = true;
            mWin.Owner = windowRef;
            mWin.WindowStartupLocation = WindowStartupLocation.CenterOwner;

            if (mWin.ShowDialog() == true)
            {
                string[] addresses = mWin.ResponseText.Split(':');

                if (addresses.Length != 2) return;

                var firstChars = new String(addresses[0].ToCharArray().Where(c => !Char.IsDigit(c)).ToArray());
                var firstNums = new String(addresses[0].ToCharArray().Where(c => Char.IsDigit(c)).ToArray());

                var secondChars = new String(addresses[1].ToCharArray().Where(c => !Char.IsDigit(c)).ToArray());
                var secondNums = new String(addresses[1].ToCharArray().Where(c => Char.IsDigit(c)).ToArray());

                int fNum, sNum;

                if (int.TryParse(firstNums, out fNum) && int.TryParse(secondNums, out sNum) && firstChars.Length > 0 && secondChars.Length > 0)
                {
                    if ((sNum - fNum) > 1)
                    {
                        ValuesBrush = Brushes.LightGreen;
                        Values = firstChars + firstNums + ":" + secondChars + secondNums;

                        lowColValue = DataGridTools.GetColumnIndex(firstChars);
                        highColValue = DataGridTools.GetColumnIndex(secondChars);

                        lowRowValue = fNum;
                        highRowValue = sNum;
                    }
                    else
                    {
                        MessageBox.Show("Please ensure that only a single row is selected");
                    }
                }
                else
                {
                    MessageBox.Show("Parse error!");
                }
            }
        }

        /// <summary>
        /// Command-based update of UI logic in VM
        /// </summary>
        private void UpdateSettings()
        {
            if (!AdvancedMenu)
            {
                AdvancedMenu = !AdvancedMenu;
            }
        }

        /// <summary>
        /// Command-based update of UI logic in VM
        /// </summary>
        public void UpdateNoise()
        {
            NoiseModel = !NoiseModel;
        }

        /// <summary>
        /// Command-based update of UI logic in VM
        /// </summary>
        public void UpdateExponential()
        {
            ExponentialModel = !ExponentialModel;
        }

        /// <summary>
        /// Command-based update of UI logic in VM
        /// </summary>
        public void UpdateHyperbolic()
        {
            HyperbolicModel = !HyperbolicModel;
        }

        /// <summary>
        /// Command-based update of UI logic in VM
        /// </summary>
        public void UpdateQuasiHyperbolic()
        {
            QuasiHyperbolicModel = !QuasiHyperbolicModel;
        }

        /// <summary>
        /// Command-based update of UI logic in VM
        /// </summary>
        public void UpdateMyersonHyperboloid()
        {
            MyerHyperboloidModel = !MyerHyperboloidModel;
        }

        /// <summary>
        /// Command-based update of UI logic in VM
        /// </summary>
        public void UpdateRachlinHyperboloid()
        {
            RachHyperboloidModel = !RachHyperboloidModel;
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

        /// <summary>
        /// Delegate after highlighting takes place on datagrid (call back specific to delays).
        /// </summary>
        private void DataGrid_PreviewMouseUp_Delays(object sender, MouseButtonEventArgs e)
        {
            List<DataGridCellInfo> cells = mWindow.dataGrid.SelectedCells.ToList();
            var itemSource = mWindow.dataGrid.ItemsSource as ObservableCollection<RowViewModel>;

            if (cells.Count < 1 || itemSource.Count < 1) return;

            lowRowDelay = cells.Min(i => DataGridTools.GetIndexViewModel((RowViewModel)i.Item, itemSource));
            highRowDelay = cells.Max(i => DataGridTools.GetIndexViewModel((RowViewModel)i.Item, itemSource));

            lowColDelay = cells.Min(i => i.Column.DisplayIndex);
            highColDelay = cells.Max(i => i.Column.DisplayIndex);


            if ((highRowDelay - lowRowDelay) > 0)
            {
                DefaultFieldsToGray();

                mWindow.dataGrid.PreviewMouseUp -= DataGrid_PreviewMouseUp_Delays;

                lowColDelay = -1;
                lowRowDelay = -1;
                highColDelay = -1;
                highRowDelay = -1;

                MessageBox.Show("Please select a single horizontal row (increasing, from left to right).  You can have many columns, but just one row.");

                return;
            }

            mWindow.dataGrid.PreviewMouseUp -= DataGrid_PreviewMouseUp_Delays;

            DelaysBrush = Brushes.LightBlue;
            Delays = DataGridTools.GetColumnName(lowColDelay) + lowRowDelay.ToString() + ":" + DataGridTools.GetColumnName(highColDelay) + highRowDelay.ToString();
        }

        /// <summary>
        /// Call window reference (shameful deviation from MVVM) for Unveil's range PickRange function.
        /// Successful (or failing) selections result in a range string in respective text fields for later parsing.
        /// </summary>
        private void GetDelaysRange()
        {
            DefaultFieldsToGray();

            DelaysBrush = Brushes.Yellow;
            Delays = "Select delays on spreadsheet";

            mWindow.dataGrid.PreviewMouseUp += DataGrid_PreviewMouseUp_Delays;
        }

        /// <summary>
        /// Delegate after highlighting takes place on datagrid (call back specific to values).
        /// </summary>
        private void DataGrid_PreviewMouseUp_Values(object sender, MouseButtonEventArgs e)
        {
            List<DataGridCellInfo> cells = mWindow.dataGrid.SelectedCells.ToList();
            var itemSource = mWindow.dataGrid.ItemsSource as ObservableCollection<RowViewModel>;

            if (cells.Count < 1 || itemSource.Count < 1) return;

            lowRowValue = cells.Min(i => DataGridTools.GetIndexViewModel((RowViewModel)i.Item, itemSource));
            highRowValue = cells.Max(i => DataGridTools.GetIndexViewModel((RowViewModel)i.Item, itemSource));

            lowColValue = cells.Min(i => i.Column.DisplayIndex);
            highColValue = cells.Max(i => i.Column.DisplayIndex);
            
            if ((highRowValue - lowRowValue) < 2)
            {
                DefaultFieldsToGray();

                mWindow.dataGrid.PreviewMouseUp -= DataGrid_PreviewMouseUp_Values;

                lowColValue = -1;
                lowRowValue = -1;
                highColValue = -1;
                highRowValue = -1;

                MessageBox.Show("Please select multiple rows (at least 3).");

                return;
            }

            mWindow.dataGrid.PreviewMouseUp -= DataGrid_PreviewMouseUp_Values;

            ValuesBrush = Brushes.LightGreen;
            Values = DataGridTools.GetColumnName(lowColValue) + lowRowValue.ToString() + ":" + DataGridTools.GetColumnName(highColValue) + highRowValue.ToString();
        }

        /// <summary>
        /// Call window reference (shameful deviation from MVVM) for Unveil's range PickRange function.
        /// Successful (or failing) selections result in a range string in respective text fields for later parsing.
        /// </summary>
        private void GetValuesRange()
        {
            DefaultFieldsToGray();

            ValuesBrush = Brushes.Yellow;
            Values = "Select values on spreadsheet";

            mWindow.dataGrid.PreviewMouseUp += DataGrid_PreviewMouseUp_Values;
        }

        /// <summary>
        /// Walk through ranged values as needed, finding appropriate values
        /// </summary>
        /// <param name="startCol">
        /// First column index for delay
        /// </param>
        /// <param name="endCol">
        /// Row index for delays
        /// </param>
        /// <param name="startRow">
        /// Row index for values
        /// </param>
        /// <returns>
        /// List of all range/value pairs that correspond
        /// </returns>
        private List<double> GetRangedValues(int startCol, int endCol, int startRow)
        {
            List<double> mRange = new List<double>();

            //DataGridCell mCell;
            string mCell;
            double test;

            //DataGridRow row = DataGridTools.GetDataGridRow(mWindow.dataGrid, startRow);

            var itemSource = mWindow.dataGrid.ItemsSource as ObservableCollection<RowViewModel>;

            if (itemSource == null)
                return null;

            for (int i = startCol; i <= endCol; i++)
            {
                //mCell = DataGridTools.GetDataGridCell(mWindow.dataGrid, DataGridTools.GetDataGridRow(mWindow.dataGrid, startRow), i);
                mCell = itemSource[startRow].values[i];
                //mCell = DataGridTools.GetDataGridCell(mWindow.dataGrid, row, i);

//                if (!Double.TryParse((((TextBlock)mCell.Content)).Text.ToString(), out test))
                if (!Double.TryParse(mCell, out test))
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
        public string[,] ParseBulkRangeStrings(int lowRowValue, int highRowValue, int lowColValue, int highColValue)
        {
            string[,] mDouble = null;
            string mCell;

            int mRows = (highRowValue - lowRowValue) + 1;
            int mCols = (highColValue - lowColValue) + 1;

            mDouble = new string[mCols, mRows];

            var itemSource = mWindow.dataGrid.ItemsSource as ObservableCollection<RowViewModel>;

            if (itemSource == null)
                return null;

            try
            {
                for (int i = lowRowValue; i <= highRowValue; i++)
                {

                    for (int j = lowColValue; j <= highColValue; j++)
                    {
                        mCell = itemSource[i].values[j];
                        mDouble[j - lowColValue, i - lowRowValue] = mCell;
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
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public string ConvertBoolToString(bool value)
        {
            return value == true ? "1" : "0";
        }

        /// <summary>
        /// Command-call to calculate based on supplied ranges and reference values (max value).
        /// Will reference user-selected options (figures, outputs, etc.) throughout calls to R
        /// </summary>
        private void CalculateScores()
        {
            mWindow.dataGrid.CommitEdit();

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
            xRange = GetRangedValues(lowColDelay, highColDelay, lowRowDelay);

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

            for (int i = 0; i < wholeRange.GetLength(0); i++)
            {
                if (double.TryParse(wholeRange[i, 0], out holder))
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
            mWin.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            mWin.Height = 600;
            mWin.Width = 800;

            for (int i = 0; i < wholeRange.GetLength(1) + 5; i++)
            {
                mVM.RowViewModels.Add(new RowViewModel());
            }

            for (int mIndex = 0; mIndex < wholeRange.GetLength(1); mIndex++)
            {
                engine.Evaluate("rm(list = setdiff(ls(), lsf.str()))");

                yRange.Clear();
                xRangeShadow.Clear();

                for (int i = 0; i < wholeRange.GetLength(0); i++)
                {

                    if (double.TryParse(wholeRange[i, mIndex], out holder))
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
                    string evalStatement = string.Format("output <-BDS(datHack, Noise={0},Mazur={1},Exponential={2},Rachlin={3},GreenMyerson={4},BD={5})",
                        1,
                        ConvertBoolToString(HyperbolicModel),
                        ConvertBoolToString(ExponentialModel),
                        ConvertBoolToString(RachHyperboloidModel),
                        ConvertBoolToString(MyerHyperboloidModel),
                        ConvertBoolToString(QuasiHyperbolicModel));

                    engine.Evaluate(evalStatement);

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

                    if (mIndex == 0)
                    {
                        mVM.RowViewModels[0].values[1] = "Exponential - ln(k): ";
                        mVM.RowViewModels[0].values[2] = "Hyperbolic - ln(k): ";
                        mVM.RowViewModels[0].values[3] = "Quasi-Hyperbolic - beta: ";
                        mVM.RowViewModels[0].values[4] = "Quasi-Hyperbolic - delta: ";
                        mVM.RowViewModels[0].values[5] = "Myerson-Hyperboloid - ln(k): ";
                        mVM.RowViewModels[0].values[6] = "Myerson-Hyperboloid - s: ";
                        mVM.RowViewModels[0].values[7] = "Rachlin-Hyperboloid - ln(k): ";
                        mVM.RowViewModels[0].values[8] = "Rachlin-Hyperboloid - s): ";
                        mVM.RowViewModels[0].values[9] = "Model Competition (#1)";
                        mVM.RowViewModels[0].values[10] = "#2";
                        mVM.RowViewModels[0].values[11] = "#3";
                        mVM.RowViewModels[0].values[12] = "#4";
                        mVM.RowViewModels[0].values[13] = "#5";
                        mVM.RowViewModels[0].values[14] = "#6";
                        mVM.RowViewModels[0].values[15] = "Most competitive model: ";
                        mVM.RowViewModels[0].values[16] = "ED50 of Most Competitive Model - ln(x): ";
                    }

                    mVM.RowViewModels[mIndex + 1].values[0] = "Series #" + (int)(mIndex + 1);

                    double ed50Best = engine.Evaluate("as.numeric(output[[8]]['lnED50.mostprob'])").AsNumeric().First();

                    mVM.RowViewModels[mIndex + 1].values[1] = engine.Evaluate("as.numeric(output[[3]]['exp.lnk'])").AsVector().First().ToString();
                    mVM.RowViewModels[mIndex + 1].values[2] = engine.Evaluate("as.numeric(output[[2]]['Mazur.lnk'])").AsVector().First().ToString();
                    mVM.RowViewModels[mIndex + 1].values[3] = engine.Evaluate("as.numeric(output[[9]]['BD.beta'])").AsVector().First().ToString();
                    mVM.RowViewModels[mIndex + 1].values[4] = engine.Evaluate("as.numeric(output[[9]]['BD.delta'])").AsVector().First().ToString();
                    mVM.RowViewModels[mIndex + 1].values[5] = engine.Evaluate("as.numeric(output[[4]]['MG.lnk'])").AsVector().First().ToString();
                    mVM.RowViewModels[mIndex + 1].values[6] = engine.Evaluate("as.numeric(output[[4]]['MG.s'])").AsVector().First().ToString();
                    mVM.RowViewModels[mIndex + 1].values[7] = engine.Evaluate("as.numeric(output[[5]]['Rachlin.lnk'])").AsVector().First().ToString();
                    mVM.RowViewModels[mIndex + 1].values[8] = engine.Evaluate("as.numeric(output[[5]]['Rachlin.s'])").AsVector().First().ToString();

                    int row = 9;
                    foreach (KeyValuePair<string, double> pair in items)
                    {
                        mVM.RowViewModels[mIndex + 1].values[row] = pair.Key + " - (" + pair.Value.ToString("0.000") + ")";
                        row++;
                    }

                    mVM.RowViewModels[mIndex + 1].values[15] = items.First().Key.ToString();
                    mVM.RowViewModels[mIndex + 1].values[16] = ed50Best.ToString();
                }
                catch (ParseException pe)
                {
                    Logging.SubmitError("ViewModelBatchDiscounting", pe.ToString());
                    mWindow.OutputEvents(pe.ToString());
                }

                mWindow.OutputEvents("Computation #" + ((int)mIndex + (int)1) + " of " + wholeRange.GetLength(1) + " Completed!");

            }

            mWindow.OutputEvents("Final Calculations Completed!");
            mWin.Show();
        }

    }
}
