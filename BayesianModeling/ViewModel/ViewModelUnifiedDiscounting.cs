//----------------------------------------------------------------------------------------------
// <copyright file="ViewModelUnifiedDiscounting.cs" 
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
// </summary>
//----------------------------------------------------------------------------------------------

using BayesianModeling.Dialogs;
using BayesianModeling.Mathematics;
using BayesianModeling.Utilities;
using BayesianModeling.View;
using RDotNet;
using SharpVectors.Converters;
using SharpVectors.Renderers.Wpf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace BayesianModeling.ViewModel
{
    class ViewModelUnifiedDiscounting : ViewModelBase
    {
        #region Declarations

        public MainWindow mWindow { get; set; }
        public UnifiedDiscountingWindow windowRef { get; set; }

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

        #region DataModes

        private bool rowModeRadio = false;
        public bool RowModeRadio
        {
            get { return rowModeRadio; }
            set
            {
                rowModeRadio = value;
                OnPropertyChanged("RowModeRadio");
                UpdateSelectionMode();
            }
        }

        private bool columnModeRadio = false;
        public bool ColumnModeRadio
        {
            get { return columnModeRadio; }
            set
            {
                columnModeRadio = value;
                OnPropertyChanged("ColumnModeRadio");
                UpdateSelectionMode();
            }
        }

        private bool singleModeRadio = false;
        public bool SingleModeRadio
        {
            get { return singleModeRadio; }
            set
            {
                singleModeRadio = value;
                OnPropertyChanged("SingleModeRadio");
                UpdateButtons();
            }
        }

        private bool batchModeRadio = false;
        public bool BatchModeRadio
        {
            get { return batchModeRadio; }
            set
            {
                batchModeRadio = value;
                OnPropertyChanged("BatchModeRadio");
                UpdateButtons();
            }
        }

        #endregion

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

        private bool boundRachHyperboloidModel = false;
        public bool BoundRachHyperboloidModel
        {
            get { return boundRachHyperboloidModel; }
            set
            {
                var confirmWithUser = new YesNoDialog();
                confirmWithUser.QuestionText = "Note: constraining the s parameter assists in retaining the conceptual framework for interpreting k, but " +
                    "numerically interferes with approximate Bayesian model selection and analytic solutions for ED50.  Please interpret with caution.";
                confirmWithUser.ShowDialog();

                boundRachHyperboloidModel = confirmWithUser.ReturnedAnswer;
                OnPropertyChanged("BoundRachHyperboloidModel");
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

        private List<string> filesList = new List<string>();

        private string mPrecision = "0.00000000";

        /* UI Logic */

        bool failed;
        string path1 = null, path2 = null;

        private bool outputFigures = false;
        public bool OutputFigures
        {
            get { return outputFigures; }
            set
            {
                outputFigures = value;
                OnPropertyChanged("OutputFigures");
            }
        }

        private bool outputBIC = false;
        public bool OutputBIC
        {
            get { return outputBIC; }
            set
            {
                outputBIC = value;
                OnPropertyChanged("OutputBIC");
            }
        }

        private bool outputAIC = false;
        public bool OutputAIC
        {
            get { return outputAIC; }
            set
            {
                outputAIC = value;
                Console.WriteLine("Aic output: " + value);
                OnPropertyChanged("OutputAIC");
            }
        }

        private bool outputProb = false;
        public bool OutputProb
        {
            get { return outputProb; }
            set
            {
                outputProb = value;
                OnPropertyChanged("OutputProb");
            }
        }

        private bool outputRanks = false;
        public bool OutputRanks
        {
            get { return outputRanks; }
            set
            {
                outputRanks = value;
                OnPropertyChanged("OutputRanks");
            }
        }

        private bool possibleFigures = false;
        public bool PossibleFigures
        {
            get { return possibleFigures; }
            set
            {
                possibleFigures = value;
                OnPropertyChanged("PossibleFigures");
            }
        }

        /// <summary>
        /// Public constructor
        /// </summary>
        public ViewModelUnifiedDiscounting()
        {
            NoiseModel = HyperbolicModel = ExponentialModel = QuasiHyperbolicModel = MyerHyperboloidModel = RachHyperboloidModel = true;

            ViewLoadedCommand = new RelayCommand(param => ViewLoaded(), param => true);
            ViewClosingCommand = new RelayCommand(param => ViewClosed(), param => true);
            AdvancedSettings = new RelayCommand(param => UpdateSettings(), param => true);

            NoiseModelCommand = new RelayCommand(param => UpdateNoise(), param => true);
            ExponentialModelCommand = new RelayCommand(param => UpdateExponential(), param => true);
            HyperbolicModelCommand = new RelayCommand(param => UpdateHyperbolic(), param => true);
            QuasiHyperbolicModelCommand = new RelayCommand(param => UpdateQuasiHyperbolic(), param => true);
            MyersonHyperboloidModelCommand = new RelayCommand(param => UpdateMyersonHyperboloid(), param => true);
            RachlinHyperboloidModelCommand = new RelayCommand(param => UpdateRachlinHyperboloid(), param => true);
            
            GetDelaysRangeCommand = new RelayCommand(param => GetDelaysRange(), param => true);            
            DelayRangeCommand = new RelayCommand(param => GetCustomDelays(), param => true);

            GetValuesRangeCommand = new RelayCommand(param => GetValuesRange(), param => true);
            CalculateScoresCommand = new RelayCommand(param => PreScoring(), param => true);
            ValueRangeCommand = new RelayCommand(param => GetCustomValues(), param => true);
        }

        #endregion

        #region ManualEntryWindows

        /// <summary>
        /// Query user for a range
        /// </summary>
        private void GetCustomDelays()
        {
            var mWin = new RangePrompt();
            mWin.Topmost = true;
            mWin.Owner = windowRef;
            mWin.ResponseText = Delays;
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
                    if (RowModeRadio)
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
                    else if (columnModeRadio)
                    {
                        if ((DataGridTools.GetColumnIndex(secondChars) - DataGridTools.GetColumnIndex(firstChars)) == 0)
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
                            MessageBox.Show("Please ensure that only a single column is selected");
                        }
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
            mWin.ResponseText = Values;
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
                    if (SingleModeRadio)
                    {
                        if (RowModeRadio)
                        {
                            if ((sNum - fNum) == 0)
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
                        else if (ColumnModeRadio)
                        {
                            if ((DataGridTools.GetColumnIndex(secondChars) - DataGridTools.GetColumnIndex(firstChars)) == 0)
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
                                MessageBox.Show("Please ensure that only a single column is selected");
                            }
                        }
                    }
                    else if (BatchModeRadio)
                    {
                        if (RowModeRadio)
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
                                MessageBox.Show("Please ensure that more than two rows are selected");
                            }
                        }
                        else if (ColumnModeRadio)
                        {
                            if ((highRowValue - lowRowValue) < 2 || (highColValue - lowColValue) < 1)
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
                                MessageBox.Show("Please ensure that more than two columns are selected");
                            }
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Parse error!");
                }
            }
        }

        #endregion

        #region CheckBoxes

        /// <summary>
        /// Calls to update after mode change
        /// </summary>
        private void UpdateStyles()
        {
            OutputFigures = false;
            lowRowValue = highRowValue = lowColValue = highColValue = -1;
            ValuesBrush = Brushes.LightGray;
            Values = "";

            mWindow.dataGrid.PreviewMouseUp -= DataGrid_PreviewMouseUp_Values;

            //if (ModeSelection == "Batched")
            if (BatchModeRadio)
            {
                PossibleFigures = true;
            }
            //else if (ModeSelection == "Single")
            else if (SingleModeRadio)
            {
                PossibleFigures = false;
            }
        }

        /// <summary>
        /// Calls to update after mode change
        /// </summary>
        private void UpdateButtons()
        {
            OutputFigures = false;
            lowRowValue = highRowValue = lowColValue = highColValue = -1;
            ValuesBrush = Brushes.LightGray;
            Values = "";

            mWindow.dataGrid.PreviewMouseUp -= DataGrid_PreviewMouseUp_Values;

            if (SingleModeRadio)
            {
                PossibleFigures = true;
            }
            else if (BatchModeRadio)
            {
                PossibleFigures = false;
            }
        }

        /// <summary>
        /// Calls to update after mode change
        /// </summary>
        private void UpdateSelectionMode()
        {
            lowRowDelay = highRowDelay = lowColDelay = highColDelay = -1;
            lowRowValue = highRowValue = lowColValue = highColValue = -1;

            Delays = "";
            Values = "";

            DefaultFieldsToGray();
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

        #endregion

        #region Events

        /// <summary>
        /// Command-based update of UI logic during close.
        /// Will retain window position in Settings.settings
        /// </summary>
        private void ViewClosed()
        {
            Properties.Settings.Default.Save();

            if (filesList.Count > 0)
            {
                foreach (string str in filesList)
                {
                    if (File.Exists(str))
                    {
                        File.Delete(str);
                    }
                }
            }
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

            //ModeSelection = "Batched";
            BatchModeRadio = true;
            RowModeRadio = true;
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

            mWindow.dataGrid.IsReadOnly = false;

            if (RowModeRadio)
            {
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
            }
            else if (ColumnModeRadio)
            {
                if ((highColDelay - lowColDelay) > 0)
                {
                    DefaultFieldsToGray();

                    mWindow.dataGrid.PreviewMouseUp -= DataGrid_PreviewMouseUp_Delays;

                    lowColDelay = -1;
                    lowRowDelay = -1;
                    highColDelay = -1;
                    highRowDelay = -1;
                    MessageBox.Show("Please select a single vertical column.  You can have many rows, but just one column of pricing values.");

                    return;
                }
            }

            mWindow.dataGrid.PreviewMouseUp -= DataGrid_PreviewMouseUp_Delays;

            DelaysBrush = Brushes.LightBlue;
            Delays = DataGridTools.GetColumnName(lowColDelay) + lowRowDelay.ToString() + ":" + DataGridTools.GetColumnName(highColDelay) + highRowDelay.ToString();
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

            mWindow.dataGrid.IsReadOnly = false;

            if (SingleModeRadio)
            {
                if (RowModeRadio)
                {
                    if ((highRowValue - lowRowValue) > 0)
                    {
                        DefaultFieldsToGray();

                        mWindow.dataGrid.PreviewMouseUp -= DataGrid_PreviewMouseUp_Values;

                        lowColValue = -1;
                        lowRowValue = -1;
                        highColValue = -1;
                        highRowValue = -1;
                        MessageBox.Show("Please select a single horizontal row (increasing, from left to right).  You can have many columns, but just one row.");

                        return;
                    }
                }
                else if (ColumnModeRadio)
                {
                    if ((highRowValue - lowRowValue) < 2 || (highColValue - lowColValue) > 0)
                    {
                        DefaultFieldsToGray();

                        mWindow.dataGrid.PreviewMouseUp -= DataGrid_PreviewMouseUp_Values;

                        lowColValue = -1;
                        lowRowValue = -1;
                        highColValue = -1;
                        highRowValue = -1;
                        MessageBox.Show("Please select a matrix of indifference values, with one column of values with at least three individual points of data (i.e., 3x3).");

                        return;
                    }
                }
            }
            else if (BatchModeRadio)
            {
                if (RowModeRadio)
                {
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
                }
                else if (ColumnModeRadio)
                {
                    if ((highRowValue - lowRowValue) < 2 || (highColValue - lowColValue) < 1)
                    {
                        DefaultFieldsToGray();

                        mWindow.dataGrid.PreviewMouseUp -= DataGrid_PreviewMouseUp_Values;

                        lowColValue = -1;
                        lowRowValue = -1;
                        highColValue = -1;
                        highRowValue = -1;
                        MessageBox.Show("Please select a matrix of indifference values, with more than one column of values with at least three individual points of data (i.e., 3x3).");

                        return;
                    }
                }
            }

            mWindow.dataGrid.PreviewMouseUp -= DataGrid_PreviewMouseUp_Values;

            ValuesBrush = Brushes.LightGreen;
            Values = DataGridTools.GetColumnName(lowColValue) + lowRowValue.ToString() + ":" + DataGridTools.GetColumnName(highColValue) + highRowValue.ToString();
        }

        #endregion

        #region DataParsing

        /// <summary>
        /// Call window reference (shameful deviation from MVVM) for PickRange function.
        /// Successful (or failing) selections result in a range string in respective text fields for later parsing.
        /// </summary>
        private void GetDelaysRange()
        {
            mWindow.dataGrid.CommitEdit();

            DefaultFieldsToGray();

            DelaysBrush = Brushes.Yellow;
            Delays = "Select delays on spreadsheet";

            mWindow.dataGrid.PreviewMouseUp += DataGrid_PreviewMouseUp_Delays;
            mWindow.dataGrid.IsReadOnly = true;
        }
        
        /// <summary>
        /// Call window reference (shameful deviation from MVVM) for PickRange function.
        /// Successful (or failing) selections result in a range string in respective text fields for later parsing.
        /// </summary>
        private void GetValuesRange()
        {
            mWindow.dataGrid.CommitEdit();

            DefaultFieldsToGray();

            ValuesBrush = Brushes.Yellow;
            Values = "Select values on spreadsheet";

            mWindow.dataGrid.PreviewMouseUp += DataGrid_PreviewMouseUp_Values;
            mWindow.dataGrid.IsReadOnly = true;
        }

        /// <summary>
        /// Converts bool to string
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public string ConvertBoolToString(bool value)
        {
            return value == true ? "1" : "0";
        }

        #endregion

        #region Calculations

        /// <summary>
        /// Routing command for mode-specific scoring
        /// </summary>
        /// <returns></returns>
        private void PreScoring()
        {
            if (BatchModeRadio)
            {
                CalculateBatchScores();
            }
            else if (SingleModeRadio)
            {
                CalculateScores();
            }
        }

        /// <summary>
        /// Grabs row from the current grid (results only)
        /// </summary>
        /// <param name="grid">Datagrid from results</param>
        /// <param name="index">Index from results</param>
        /// <returns></returns>
        public DataGridRow GetGridRow(CustomDataGrid grid, int index)
        {
            DataGridRow row = (DataGridRow) grid.ItemContainerGenerator.ContainerFromIndex(index);
            if (row == null)
            {
                grid.UpdateLayout();
                grid.ScrollIntoView(grid.Items[index]);
                row = (DataGridRow) grid.ItemContainerGenerator.ContainerFromIndex(index);
            }
            return row;
        }

        /// <summary>
        /// Command-call to calculate based on supplied ranges and reference values (max value).
        /// Will reference user-selected options (figures, outputs, etc.) throughout calls to R
        /// </summary>
        private void CalculateScores()
        {
            mWindow.dataGrid.CommitEdit();

            if (failed) return;

            if (Delays == Values)
            {
                mWindow.OutputEvents("Error while validating the ranges.  Did you select the same for each?");
                MessageBox.Show("Please review the ranges.  These cannot be the same.");
                return;
            }
            
            List<double>[] array = null;

            if (RowModeRadio)
            {
                array = DataGridTools.GetRangedValuesHorizontal(lowColDelay, highColDelay, lowRowDelay, lowColValue, highColValue, lowRowValue, mWindow.dataGrid.ItemsSource);
            }
            else if (ColumnModeRadio)
            {
                array = DataGridTools.GetRangedValuesVertical(lowRowDelay, highRowDelay, lowColDelay, lowRowValue, highRowValue, lowColValue, mWindow.dataGrid.ItemsSource);
            }

            if (array == null)
            {
                return;
            }

            List<double> xRange = array[0];
            List<double> yRange = array[1];

            if (xRange == null || yRange == null)
            {
                return;
            }

            mWindow.OutputEvents("---------------------------------------------------");
            mWindow.OutputEvents("Checking user-supplied ranges and reference points.");

            if (!double.TryParse(DelayedValue, out MaxValueA) || MaxValueA == 0)
            {
                mWindow.OutputEvents("Error while validating the Delayed Amount.  Is this a non-zero number?");
                MessageBox.Show("Please review the Delayed Amount number.  This must be a non-zero number.");
                return;
            }

            if (xRange.Count != yRange.Count)
            {
                mWindow.OutputEvents("Error while validating current ranges, Delay/Value ranges must be EQUAL in length for comparison.");
                mWindow.OutputEvents("Counts for Delays/Values were " + xRange.Count + " and " + yRange.Count + " respectively.");
                MessageBox.Show("Error while validating current ranges, Delay/Value ranges must be EQUAL in length for comparison.");
                return;
            }

            if ((yRange[0] / MaxValueA) <= 0.1)
            {
                MessageBox.Show("There's a chance your max value is off (the initial value is <10% of the max already).  If this is expected, please disregard.");
                mWindow.OutputEvents("Initial indifference point was <10% of A.  This is irregular, please inspect.  If this is accurate, disregard.");
            }

            if (yRange[0] > MaxValueA)
            {
                MessageBox.Show("There's a chance your max value is off (the initial value is greater than the max).  This shouldn't be possible.");
                mWindow.OutputEvents("Initial value is greater than A.  This shouldn't be possible.  Halting Computation.");
                return;
            }

            mWindow.OutputEvents("Inputs passed verification.");
            mWindow.OutputEvents("Figure output: " + outputFigures);
            mWindow.OutputEvents("Workbook output: true");
            mWindow.OutputEvents("Beginning Computation...");

            windowRef.calculateButton.IsEnabled = false;

            try
            {
                engine.Evaluate("rm(list = setdiff(ls(), lsf.str()))");

                List<double>[] arrayNew = null;

                if (RowModeRadio)
                {
                    arrayNew = DataGridTools.GetRangedValuesHorizontal(lowColDelay, highColDelay, lowRowDelay, lowColValue, highColValue, lowRowValue, mWindow.dataGrid.ItemsSource);
                }
                else if (ColumnModeRadio)
                {
                    arrayNew = DataGridTools.GetRangedValuesVertical(lowRowDelay, highRowDelay, lowColDelay, lowRowValue, highRowValue, lowColValue, mWindow.dataGrid.ItemsSource);
                }
                
                List<double> xRangeNew = arrayNew[0];
                List<double> yRangeNew = arrayNew[1];

                NumericVector delayValues = engine.CreateNumericVector(xRangeNew.ToArray());
                engine.SetSymbol("mDelays", delayValues);

                List<double> yRangeMod = new List<double>(yRangeNew);

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

                engine.Evaluate(DiscountingModelSelection.GetFranckFunction());

                engine.Evaluate("datHack<-data.frame(X = mDelays, Y = mIndiffs, ses=mSes)");

                string rachlinBoundCheck = (ConvertBoolToString(RachHyperboloidModel) == "1" && BoundRachHyperboloidModel) ? "2" : ConvertBoolToString(RachHyperboloidModel);

                engine.Evaluate("datHack<-data.frame(X = mDelays, Y = mIndiffs, ses=mSes)");
                string evalStatement = string.Format("output <-BDS(datHack, Noise={0},Mazur={1},Exponential={2},Rachlin={3},GreenMyerson={4},BD={5})",
                    1,
                    ConvertBoolToString(HyperbolicModel),
                    ConvertBoolToString(ExponentialModel),
                    rachlinBoundCheck,
                    ConvertBoolToString(MyerHyperboloidModel),
                    ConvertBoolToString(QuasiHyperbolicModel));

                engine.Evaluate(evalStatement);

                engine.Evaluate("ainslieK <- as.numeric(output[[2]]['Mazur.lnk'])");
                engine.Evaluate("samuelsonK <- as.numeric(output[[3]]['exp.lnk'])");
                engine.Evaluate("beta <- as.numeric(output[[9]]['BD.beta'])");
                engine.Evaluate("delta <- as.numeric(output[[9]]['BD.delta'])");
                engine.Evaluate("myersonK <- as.numeric(output[[4]]['MG.lnk'])");
                engine.Evaluate("myersonS <- as.numeric(output[[4]]['MG.s'])");
                engine.Evaluate("rachlinK <- as.numeric(output[[5]]['Rachlin.lnk'])");
                engine.Evaluate("rachlinS <- as.numeric(output[[5]]['Rachlin.s'])");

                NumericVector aSymbol = engine.Evaluate(MaxValueA.ToString()).AsNumeric();
                engine.SetSymbol("A", aSymbol);

                engine.SetSymbol("mDelays", delayValues);

                indiffValues = engine.CreateNumericVector(yRangeNew);
                engine.SetSymbol("mIndiff", indiffValues);

                engine.Evaluate("endDelay <- max(mDelays)");
            }
            catch (ParseException pe)
            {
                mWindow.OutputEvents(pe.ToString());
            }

            mWindow.OutputEvents("Computation successful!");
            mWindow.OutputEvents(" ");

            double noiseProb = double.NaN;
            double hyperProb = double.NaN;
            double exponProb = double.NaN;
            double quasiProb = double.NaN;
            double myerProb = double.NaN;
            double rachProb = double.NaN;

            if (!BoundRachHyperboloidModel)
            {
                try
                {
                    double.TryParse(engine.Evaluate("as.numeric(output[[1]]['noise.prob'])").AsVector().First().ToString(), out noiseProb);
                    double.TryParse(engine.Evaluate("as.numeric(output[[2]]['Mazur.prob'])").AsVector().First().ToString(), out hyperProb);
                    double.TryParse(engine.Evaluate("as.numeric(output[[3]]['exp.prob'])").AsVector().First().ToString(), out exponProb);
                    double.TryParse(engine.Evaluate("as.numeric(output[[9]]['BD.prob'])").AsVector().First().ToString(), out quasiProb);
                    double.TryParse(engine.Evaluate("as.numeric(output[[4]]['MG.prob'])").AsVector().First().ToString(), out myerProb);
                    double.TryParse(engine.Evaluate("as.numeric(output[[5]]['Rachlin.prob'])").AsVector().First().ToString(), out rachProb);

                    //double noiseProb = double.Parse(engine.Evaluate("as.numeric(output[[1]]['noise.prob'])").AsVector().First().ToString(), System.Globalization.NumberStyles.Float);
                    //double hyperProb = double.Parse(engine.Evaluate("as.numeric(output[[2]]['Mazur.prob'])").AsVector().First().ToString(), System.Globalization.NumberStyles.Float);
                    //double exponProb = double.Parse(engine.Evaluate("as.numeric(output[[3]]['exp.prob'])").AsVector().First().ToString(), System.Globalization.NumberStyles.Float);
                    //double quasiProb = double.Parse(engine.Evaluate("as.numeric(output[[9]]['BD.prob'])").AsVector().First().ToString(), System.Globalization.NumberStyles.Float);
                    //double myerProb = double.Parse(engine.Evaluate("as.numeric(output[[4]]['MG.prob'])").AsVector().First().ToString(), System.Globalization.NumberStyles.Float);
                    //double rachProb = double.Parse(engine.Evaluate("as.numeric(output[[5]]['Rachlin.prob'])").AsVector().First().ToString(), System.Globalization.NumberStyles.Float);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
            }

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

            mWindow.OutputEvents("Outputting to workbook Started... Please wait... ");

            var mWin = new ResultsWindow();
            var mVM = new ResultsViewModel();
            mWin.DataContext = mVM;
            mWin.Owner = windowRef;
            mWin.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            mWin.Height = 500;
            mWin.Width = 800;

            for (int i = 0; i < 25; i++)
            {
                mVM.RowViewModels.Add(new RowViewModel());
            }

            mVM.RowViewModels[0].values[0] = "Results of Discounting Model Selector";
            mVM.RowViewModels[1].values[0] = "Single Series Analysis";
            mVM.RowViewModels[0].values[1] = "Exponential - k: ";

            double modExp = double.NaN;
            if (double.TryParse(engine.Evaluate("as.numeric(output[[3]]['exp.lnk'])").AsVector().First().ToString(), out modExp))
            {
                mVM.RowViewModels[1].values[1] = Math.Exp(modExp).ToString(mPrecision);
            }
            else
            {
                mVM.RowViewModels[1].values[1] = modExp.ToString();
            }

            mVM.RowViewModels[0].values[2] = "Hyperbolic - k: ";

            double modHyp = double.NaN;
            if (double.TryParse(engine.Evaluate("as.numeric(output[[2]]['Mazur.lnk'])").AsVector().First().ToString(), out modHyp))
            {
                mVM.RowViewModels[1].values[2] = Math.Exp(modHyp).ToString(mPrecision);
            }
            else
            {
                mVM.RowViewModels[1].values[2] = modHyp.ToString();
            }

            mVM.RowViewModels[0].values[3] = "Quasi-Hyperbolic - beta: ";

            double qhB = double.NaN;
            if (double.TryParse(engine.Evaluate("as.numeric(output[[9]]['BD.beta'])").AsVector().First().ToString(), out qhB))
            {
                mVM.RowViewModels[1].values[3] = qhB.ToString(mPrecision);
            }
            else
            {
                mVM.RowViewModels[1].values[3] = qhB.ToString();
            }

            //mVM.RowViewModels[1].values[3] = engine.Evaluate("as.numeric(output[[9]]['BD.beta'])").AsVector().First().ToString();

            mVM.RowViewModels[0].values[4] = "Quasi-Hyperbolic - delta: ";
            
            double qhD = double.NaN;
            if (double.TryParse(engine.Evaluate("as.numeric(output[[9]]['BD.delta'])").AsVector().First().ToString(), out qhD))
            {
                mVM.RowViewModels[1].values[4] = qhD.ToString(mPrecision);
            }
            else
            {
                mVM.RowViewModels[1].values[4] = qhD.ToString();
            }

            //mVM.RowViewModels[1].values[4] = engine.Evaluate("as.numeric(output[[9]]['BD.delta'])").AsVector().First().ToString();

            mVM.RowViewModels[0].values[5] = "Myerson-Hyperboloid - k: ";

            double modMG = double.NaN;
            if (double.TryParse(engine.Evaluate("as.numeric(output[[4]]['MG.lnk'])").AsVector().First().ToString(), out modMG))
            {
                mVM.RowViewModels[1].values[5] = Math.Exp(modMG).ToString(mPrecision);
            }
            else
            {
                mVM.RowViewModels[1].values[5] = modMG.ToString();
            }

            mVM.RowViewModels[0].values[6] = "Myerson-Hyperboloid - s: ";

            double mS = double.NaN;
            if (double.TryParse(engine.Evaluate("as.numeric(output[[4]]['MG.s'])").AsVector().First().ToString(), out mS))
            {
                mVM.RowViewModels[1].values[6] = mS.ToString(mPrecision);
            }
            else
            {
                mVM.RowViewModels[1].values[6] = mS.ToString();
            }

            //mVM.RowViewModels[1].values[6] = engine.Evaluate("as.numeric(output[[4]]['MG.s'])").AsVector().First().ToString();
            
            mVM.RowViewModels[0].values[7] = "Rachlin-Hyperboloid - k: ";

            double modR = double.NaN;
            if (double.TryParse(engine.Evaluate("as.numeric(output[[5]]['Rachlin.lnk'])").AsVector().First().ToString(), out modR))
            {
                mVM.RowViewModels[1].values[7] = Math.Exp(modR).ToString(mPrecision);
            }
            else
            {
                mVM.RowViewModels[1].values[7] = modR.ToString();
            }

            mVM.RowViewModels[0].values[8] = "Rachlin-Hyperboloid - s: ";

            double rS = double.NaN;
            if (double.TryParse(engine.Evaluate("as.numeric(output[[5]]['Rachlin.s'])").AsVector().First().ToString(), out rS))
            {
                mVM.RowViewModels[1].values[8] = rS.ToString(mPrecision);
            }
            else
            {
                mVM.RowViewModels[1].values[8] = rS.ToString();
            }

            //mVM.RowViewModels[1].values[8] = engine.Evaluate("as.numeric(output[[5]]['Rachlin.s'])").AsVector().First().ToString();

            int col = 9;

            if (!BoundRachHyperboloidModel)
            {
                mVM.RowViewModels[0].values[col] = "Most competitive model: ";
                mVM.RowViewModels[1].values[col] = items.First().Key.ToString();
                col++;

                double ed50Best = engine.Evaluate("as.numeric(output[[8]]['lnED50.mostprob'])").AsNumeric().First();

                mVM.RowViewModels[0].values[col] = "ED50 of Most Competitive Model - ln(x): ";
                mVM.RowViewModels[1].values[col] = ed50Best.ToString(mPrecision);
                col++;
            }

            if (OutputProb && !BoundRachHyperboloidModel)
            {

                mVM.RowViewModels[0].values[col] = "Noise Model Probs";
                mVM.RowViewModels[1].values[col] = noiseProb.ToString(mPrecision);
                col++;

                mVM.RowViewModels[0].values[col] = "Exponential Model Probs";
                mVM.RowViewModels[1].values[col] = exponProb.ToString(mPrecision);
                col++;

                mVM.RowViewModels[0].values[col] = "Hyperbolic Model Probs";
                mVM.RowViewModels[1].values[col] = hyperProb.ToString(mPrecision);
                col++;

                mVM.RowViewModels[0].values[col] = "Quasi Hyperbolic Model Probs";
                mVM.RowViewModels[1].values[col] = quasiProb.ToString(mPrecision);
                col++;

                mVM.RowViewModels[0].values[col] = "Hyperboloid (Myerson) Model Probs";
                mVM.RowViewModels[1].values[col] = myerProb.ToString(mPrecision);
                col++;

                mVM.RowViewModels[0].values[col] = "Hyperboloid (Rachlin) Model Probs";
                mVM.RowViewModels[1].values[col] = rachProb.ToString(mPrecision);
                col++;
            }

            if (OutputBIC)
            {
                mVM.RowViewModels[0].values[col] = "Noise Model BIC";
                mVM.RowViewModels[1].values[col] = engine.Evaluate("as.numeric(output[[1]]['noise.BIC'])").AsVector().AsNumeric().First().ToString(mPrecision);
                col++;

                mVM.RowViewModels[0].values[col] = "Exponential Model BIC";
                mVM.RowViewModels[1].values[col] = engine.Evaluate("as.numeric(output[[3]]['exp.BIC'])").AsVector().AsNumeric().First().ToString(mPrecision);
                col++;

                mVM.RowViewModels[0].values[col] = "Hyperbolic Model BIC";
                mVM.RowViewModels[1].values[col] = engine.Evaluate("as.numeric(output[[2]]['Mazur.BIC'])").AsVector().AsNumeric().First().ToString(mPrecision);
                col++;

                mVM.RowViewModels[0].values[col] = "Quasi Hyperbolic Model BIC";
                mVM.RowViewModels[1].values[col] = engine.Evaluate("as.numeric(output[[9]]['BD.BIC'])").AsVector().AsNumeric().First().ToString(mPrecision);
                col++;

                mVM.RowViewModels[0].values[col] = "Hyperboloid (Myerson) Model BIC";
                mVM.RowViewModels[1].values[col] = engine.Evaluate("as.numeric(output[[4]]['MG.BIC'])").AsVector().AsNumeric().First().ToString(mPrecision);
                col++;

                mVM.RowViewModels[0].values[col] = "Hyperboloid (Rachlin) Model BIC";
                mVM.RowViewModels[1].values[col] = engine.Evaluate("as.numeric(output[[5]]['Rachlin.BIC'])").AsVector().AsNumeric().First().ToString(mPrecision);
                col++;
            }

            if (OutputAIC)
            {
                mVM.RowViewModels[0].values[col] = "Noise Model AIC";
                mVM.RowViewModels[1].values[col] = engine.Evaluate("as.numeric(output[[1]]['noise.AIC'])").AsVector().AsNumeric().First().ToString(mPrecision);
                col++;

                mVM.RowViewModels[0].values[col] = "Exponential Model AIC";
                mVM.RowViewModels[1].values[col] = engine.Evaluate("as.numeric(output[[3]]['exp.AIC'])").AsVector().AsNumeric().First().ToString(mPrecision);
                col++;

                mVM.RowViewModels[0].values[col] = "Hyperbolic Model AIC";
                mVM.RowViewModels[1].values[col] = engine.Evaluate("as.numeric(output[[2]]['Mazur.AIC'])").AsVector().AsNumeric().First().ToString(mPrecision);
                col++;

                mVM.RowViewModels[0].values[col] = "Quasi Hyperbolic Model AIC";
                mVM.RowViewModels[1].values[col] = engine.Evaluate("as.numeric(output[[9]]['BD.AIC'])").AsVector().AsNumeric().First().ToString(mPrecision);
                col++;

                mVM.RowViewModels[0].values[col] = "Hyperboloid (Myerson) Model AIC";
                mVM.RowViewModels[1].values[col] = engine.Evaluate("as.numeric(output[[4]]['MG.AIC'])").AsVector().AsNumeric().First().ToString(mPrecision);
                col++;

                mVM.RowViewModels[0].values[col] = "Hyperboloid (Rachlin) Model AIC";
                mVM.RowViewModels[1].values[col] = engine.Evaluate("as.numeric(output[[5]]['Rachlin.AIC'])").AsVector().AsNumeric().First().ToString(mPrecision);
                col++;
            }

            if (OutputRanks && !BoundRachHyperboloidModel)
            {
                int rank = 1;

                foreach (KeyValuePair<string, double> pair in items)
                {
                    mVM.RowViewModels[0].values[col] = pair.Key + " Probability (Ranked #" + rank + ")";
                    mVM.RowViewModels[1].values[col] = pair.Value.ToString(mPrecision);
                    rank++;
                    col++;
                }
            }

            mVM.RowViewModels[3].values[0] = "Delayed Value";
            mVM.RowViewModels[3].values[1] = DelayedValue;

            mVM.RowViewModels[4].values[0] = "Delays";
            mVM.RowViewModels[5].values[0] = "Values";

            for (int i = 0; i < xRange.Count; i++)
            {
                mVM.RowViewModels[4].values[1 + i] = xRange[i].ToString();
                mVM.RowViewModels[5].values[1 + i] = yRange[i].ToString();
            }

            mWin.Show();

            for (int i = 0; i < mWin.dataGrid.Items.Count; i++)
            {
                col = 11;

                if (BoundRachHyperboloidModel)
                {
                    col = 9;
                }

                var row = GetGridRow(mWin.dataGrid, i);

                if (row == null) continue;

                if (mVM.RowViewModels[i].values[9] == "Noise Model")
                {
                    row.Background = Brushes.Yellow;
                }

                bool grayOut = true;

                if (OutputProb && !BoundRachHyperboloidModel)
                {
                    if (grayOut)
                    {
                        for (int j = 0; j < 6; j++)
                        {
                            var mCell = mWin.dataGrid.Columns[col].GetCellContent(row).Parent as DataGridCell;
                            mCell.Background = Brushes.LightGray;
                            col++;
                        }
                    }
                    else
                    {
                        col += 6;
                    }

                    grayOut = !grayOut;
                }

                if (OutputBIC)
                {
                    if (grayOut)
                    {
                        for (int j = 0; j < 6; j++)
                        {
                            var mCell = mWin.dataGrid.Columns[col].GetCellContent(row).Parent as DataGridCell;
                            mCell.Background = Brushes.LightGray;
                            col++;
                        }
                    }
                    else
                    {
                        col += 6;
                    }

                    grayOut = !grayOut;
                }

                if (OutputRanks && !BoundRachHyperboloidModel)
                {
                    if (grayOut)
                    {
                        for (int j = 0; j < 6; j++)
                        {
                            var mCell = mWin.dataGrid.Columns[col].GetCellContent(row).Parent as DataGridCell;
                            mCell.Background = Brushes.LightGray;
                            col++;
                        }
                    }
                    else
                    {
                        col += 6;
                    }

                    grayOut = !grayOut;
                }
            }

            mWindow.OutputEvents("Output Completed!");

            if (outputFigures)
            {
                mWindow.OutputEvents("Charting Started... Please wait... ");

                try
                {
                    engine.Evaluate("library(ggplot2)");
                    engine.Evaluate("library(reshape2)");
                    engine.Evaluate("library(gridExtra)");

                    engine.Evaluate(DiscountingModelSelection.GetLogChartFunction());

                    WpfDrawingSettings settings = new WpfDrawingSettings();
                    settings.IncludeRuntime = true;
                    settings.TextAsGeometry = false;

                    string output = engine.Evaluate("barString").AsVector().First().ToString();

                    byte[] bytes = Convert.FromBase64String(output);

                    path1 = Path.GetTempFileName();

                    if (File.Exists(path1))
                    {
                        File.Delete(path1);
                    }

                    File.WriteAllBytes(path1, bytes);

                    FileSvgReader converter1 = new FileSvgReader(settings);
                    DrawingGroup drawing1 = converter1.Read(path1);

                    if (drawing1 != null)
                    {
                        var iWindow1 = new ImageWindow();
                        iWindow1.filePath = path1;
                        iWindow1.Owner = windowRef;
                        iWindow1.imageHolder.Source = new DrawingImage(drawing1);
                        iWindow1.Show();
                    }

                    string output2 = engine.Evaluate("lineString").AsVector().First().ToString();

                    byte[] bytes2 = Convert.FromBase64String(output2);
                    path2 = Path.GetTempFileName();

                    if (File.Exists(path2))
                    {
                        File.Delete(path2);
                    }

                    File.WriteAllBytes(path2, bytes2);

                    FileSvgReader converter2 = new FileSvgReader(settings);
                    DrawingGroup drawing2 = converter2.Read(path2);

                    if (drawing2 != null)
                    {
                        var iWindow2 = new ImageWindow();
                        iWindow2.filePath = path2;
                        iWindow2.Owner = windowRef;
                        iWindow2.imageHolder.Source = new DrawingImage(drawing2);
                        iWindow2.Show();
                    }

                    mWindow.OutputEvents("Charting Completed!");

                    filesList.Add(path1);
                    filesList.Add(path2);

                }
                catch (Exception e)
                {
                    //Logging.SubmitError("ViewModelDiscounting", e.ToString());
                    mWindow.OutputEvents(e.ToString());
                }

                mWindow.OutputEvents("Please remember to cite the packages used in this process!");
                mWindow.OutputEvents("Citation:: Gilroy, S. P., Franck, C. T. & Hantula, D. A. (2016). The Discounting Model Selector: Statistical discounting software.");
                mWindow.OutputEvents("Citation:: Franck, C. T., Koffarnus, M. N., House, L. L. & Bickel, W. K. (2015). Accurate characterization of delay discounting: a multiple model approach using approximate Bayesian model selection and a unified discounting measure. Journal of the Experimental Analysis of Behavior, 103(1), 218-233.");
                mWindow.OutputEvents("Citation:: " + string.Join("", engine.Evaluate("citation()$textVersion").AsCharacter().ToArray()));
                mWindow.OutputEvents("Citation:: " + string.Join("", engine.Evaluate("citation('ggplot2')$textVersion").AsCharacter().ToArray()));
                mWindow.OutputEvents("Citation:: " + string.Join("", engine.Evaluate("citation('gridExtra')$textVersion").AsCharacter().ToArray()));
                mWindow.OutputEvents("Citation:: " + string.Join("", engine.Evaluate("citation('base64enc')$textVersion").AsCharacter().ToArray()));
                mWindow.OutputEvents("Citation:: " + string.Join("", engine.Evaluate("citation('reshape2')$textVersion").AsCharacter().ToArray()));
                mWindow.OutputEvents("Citation:: " + string.Join("", engine.Evaluate("citation('scales')$textVersion").AsCharacter().ToArray()));
            }

            if (windowRef != null)
            {
                windowRef.calculateButton.IsEnabled = true;
            }
        }

        /// <summary>
        /// Command-call to calculate based on supplied ranges and reference values (max value).
        /// Will reference user-selected options (figures, outputs, etc.) throughout calls to R
        /// </summary>
        private void CalculateBatchScores()
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

            if (Delays == Values)
            {
                mWindow.OutputEvents("Error while validating the ranges.  Did you select the same for each?");
                MessageBox.Show("Please review the ranges.  These cannot be the same.");
                return;
            }

            List<double> xRange = null;
            string[,] wholeRange = null;

            if (RowModeRadio)
            {
                xRange = DataGridTools.GetRangedValuesVM(lowColDelay, highColDelay, lowRowDelay, mWindow.dataGrid.ItemsSource);
                wholeRange = DataGridTools.ParseBulkRangeStringsVM(lowRowValue, highRowValue, lowColValue, highColValue, mWindow.dataGrid.ItemsSource);
            }
            else if (ColumnModeRadio)
            {
                xRange = DataGridTools.GetRangedValuesVerticalVM(lowRowDelay, highRowDelay, lowColDelay, mWindow.dataGrid.ItemsSource);
                wholeRange = DataGridTools.ParseBulkRangeStringsVerticalVM(lowRowValue, highRowValue, lowColValue, highColValue, mWindow.dataGrid.ItemsSource);
            }

            if (xRange == null)
            {
                mWindow.OutputEvents("Error while validating the Delays.  There cannot be any blank, null or non-numeric fields.");
                MessageBox.Show("Please review the the Delays column.  There cannot be any blank, null or non-numeric fields.");
                return;
            }

            if (wholeRange == null)
            {
                mWindow.OutputEvents("There were items that failed validation in the Indifference Point values.  Are any fields blank or not numeric?");
                MessageBox.Show("There were items that failed validation in the Indifference Point values.");
                return;
            }

            List<double> yRange = new List<double>();

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

            windowRef.calculateButton.IsEnabled = false;

            var mWin = new ResultsWindow();
            var mVM = new ResultsViewModel();
            mWin.DataContext = mVM;
            mWin.Owner = windowRef;
            mWin.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            mWin.Height = 600;
            mWin.Width = 800;

            for (int i = 0; i < wholeRange.GetLength(1) + 22; i++)
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
                        engine.Evaluate(DiscountingModelSelection.GetFranckFunction());
                    }

                    string rachlinBoundCheck = (ConvertBoolToString(RachHyperboloidModel) == "1" && BoundRachHyperboloidModel) ? "2" : ConvertBoolToString(RachHyperboloidModel);

                    engine.Evaluate("datHack<-data.frame(X = mDelays, Y = mIndiffs, ses=mSes)");
                    string evalStatement = string.Format("output <-BDS(datHack, Noise={0},Mazur={1},Exponential={2},Rachlin={3},GreenMyerson={4},BD={5})",
                        1,
                        ConvertBoolToString(HyperbolicModel),
                        ConvertBoolToString(ExponentialModel),
                        rachlinBoundCheck,
                        ConvertBoolToString(MyerHyperboloidModel),
                        ConvertBoolToString(QuasiHyperbolicModel));

                    try
                    {
                        engine.Evaluate(evalStatement);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.ToString());
                    }


                    double noiseProb = double.NaN;
                    double hyperProb = double.NaN;
                    double exponProb = double.NaN;
                    double quasiProb = double.NaN;
                    double myerProb = double.NaN;
                    double rachProb = double.NaN;

                    if (!BoundRachHyperboloidModel)
                    {
                        try
                        {
                            double.TryParse(engine.Evaluate("as.numeric(output[[1]]['noise.prob'])").AsVector().First().ToString(), out noiseProb);
                            double.TryParse(engine.Evaluate("as.numeric(output[[2]]['Mazur.prob'])").AsVector().First().ToString(), out hyperProb);
                            double.TryParse(engine.Evaluate("as.numeric(output[[3]]['exp.prob'])").AsVector().First().ToString(), out exponProb);
                            double.TryParse(engine.Evaluate("as.numeric(output[[9]]['BD.prob'])").AsVector().First().ToString(), out quasiProb);
                            double.TryParse(engine.Evaluate("as.numeric(output[[4]]['MG.prob'])").AsVector().First().ToString(), out myerProb);
                            double.TryParse(engine.Evaluate("as.numeric(output[[5]]['Rachlin.prob'])").AsVector().First().ToString(), out rachProb);

                            //double noiseProb = double.Parse(engine.Evaluate("as.numeric(output[[1]]['noise.prob'])").AsVector().First().ToString(), System.Globalization.NumberStyles.Float);
                            //double hyperProb = double.Parse(engine.Evaluate("as.numeric(output[[2]]['Mazur.prob'])").AsVector().First().ToString(), System.Globalization.NumberStyles.Float);
                            //double exponProb = double.Parse(engine.Evaluate("as.numeric(output[[3]]['exp.prob'])").AsVector().First().ToString(), System.Globalization.NumberStyles.Float);
                            //double quasiProb = double.Parse(engine.Evaluate("as.numeric(output[[9]]['BD.prob'])").AsVector().First().ToString(), System.Globalization.NumberStyles.Float);
                            //double myerProb = double.Parse(engine.Evaluate("as.numeric(output[[4]]['MG.prob'])").AsVector().First().ToString(), System.Globalization.NumberStyles.Float);
                            //double rachProb = double.Parse(engine.Evaluate("as.numeric(output[[5]]['Rachlin.prob'])").AsVector().First().ToString(), System.Globalization.NumberStyles.Float);
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e.ToString());
                        }
                    }

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
                        mVM.RowViewModels[0].values[1] = "Exponential - k: ";
                        mVM.RowViewModels[0].values[2] = "Hyperbolic - k: ";
                        mVM.RowViewModels[0].values[3] = "Quasi-Hyperbolic - beta: ";
                        mVM.RowViewModels[0].values[4] = "Quasi-Hyperbolic - delta: ";
                        mVM.RowViewModels[0].values[5] = "Myerson-Hyperboloid - k: ";
                        mVM.RowViewModels[0].values[6] = "Myerson-Hyperboloid - s: ";
                        mVM.RowViewModels[0].values[7] = "Rachlin-Hyperboloid - k: ";
                        mVM.RowViewModels[0].values[8] = "Rachlin-Hyperboloid - s: ";

                        int colTitle = 9;

                        if (!BoundRachHyperboloidModel)
                        {
                            mVM.RowViewModels[0].values[colTitle] = "Most competitive model: ";
                            colTitle++;
                            mVM.RowViewModels[0].values[colTitle] = "ED50 of Most Competitive Model - ln(x): ";
                            colTitle++;
                        }

                        if (OutputProb && !BoundRachHyperboloidModel)
                        {
                            mVM.RowViewModels[0].values[colTitle] = "Noise Model Probs";
                            colTitle++;
                            mVM.RowViewModels[0].values[colTitle] = "Exponential Model Probs";
                            colTitle++;
                            mVM.RowViewModels[0].values[colTitle] = "Hyperbolic Model Probs";
                            colTitle++;
                            mVM.RowViewModels[0].values[colTitle] = "Quasi Hyperbolic Model Probs";
                            colTitle++;
                            mVM.RowViewModels[0].values[colTitle] = "Hyperboloid (Myerson) Model Probs";
                            colTitle++;
                            mVM.RowViewModels[0].values[colTitle] = "Hyperboloid (Rachlin) Model Probs";
                            colTitle++;
                        }

                        if (OutputBIC)
                        {
                            mVM.RowViewModels[0].values[colTitle] = "Noise Model BIC";
                            colTitle++;
                            mVM.RowViewModels[0].values[colTitle] = "Exponential Model BIC";
                            colTitle++;
                            mVM.RowViewModels[0].values[colTitle] = "Hyperbolic Model BIC";
                            colTitle++;
                            mVM.RowViewModels[0].values[colTitle] = "Quasi Hyperbolic Model BIC";
                            colTitle++;
                            mVM.RowViewModels[0].values[colTitle] = "Hyperboloid (Myerson) Model BIC";
                            colTitle++;
                            mVM.RowViewModels[0].values[colTitle] = "Hyperboloid (Rachlin) Model BIC";
                            colTitle++;
                        }

                        if (OutputAIC)
                        {
                            mVM.RowViewModels[0].values[colTitle] = "Noise Model AIC";
                            colTitle++;
                            mVM.RowViewModels[0].values[colTitle] = "Exponential Model AIC";
                            colTitle++;
                            mVM.RowViewModels[0].values[colTitle] = "Hyperbolic Model AIC";
                            colTitle++;
                            mVM.RowViewModels[0].values[colTitle] = "Quasi Hyperbolic Model AIC";
                            colTitle++;
                            mVM.RowViewModels[0].values[colTitle] = "Hyperboloid (Myerson) Model AIC";
                            colTitle++;
                            mVM.RowViewModels[0].values[colTitle] = "Hyperboloid (Rachlin) Model AIC";
                            colTitle++;
                        }

                        if (OutputRanks && !BoundRachHyperboloidModel)
                        {
                            mVM.RowViewModels[0].values[colTitle] = "Ranked #1";
                            colTitle++;
                            mVM.RowViewModels[0].values[colTitle] = "Ranked #2";
                            colTitle++;
                            mVM.RowViewModels[0].values[colTitle] = "Ranked #3";
                            colTitle++;
                            mVM.RowViewModels[0].values[colTitle] = "Ranked #4";
                            colTitle++;
                            mVM.RowViewModels[0].values[colTitle] = "Ranked #5";
                            colTitle++;
                            mVM.RowViewModels[0].values[colTitle] = "Ranked #6";
                            colTitle++;
                        }
                    }

                    mVM.RowViewModels[mIndex + 1].values[0] = "Series #" + (int)(mIndex + 1);

                    double ed50Best = engine.Evaluate("as.numeric(output[[8]]['lnED50.mostprob'])").AsNumeric().First();

                    double modExp = double.NaN;
                    if (double.TryParse(engine.Evaluate("as.numeric(output[[3]]['exp.lnk'])").AsVector().First().ToString(), out modExp))
                    {
                        mVM.RowViewModels[mIndex + 1].values[1] = Math.Exp(modExp).ToString(mPrecision);
                    }
                    else
                    {
                        mVM.RowViewModels[mIndex + 1].values[1] = modExp.ToString();
                    }

                    double modHyp = double.NaN;
                    if (double.TryParse(engine.Evaluate("as.numeric(output[[2]]['Mazur.lnk'])").AsVector().First().ToString(), out modHyp))
                    {
                        mVM.RowViewModels[mIndex + 1].values[2] = Math.Exp(modHyp).ToString(mPrecision);
                    }
                    else
                    {
                        mVM.RowViewModels[mIndex + 1].values[2] = modHyp.ToString();
                    }

                    double modMG = double.NaN;
                    if (double.TryParse(engine.Evaluate("as.numeric(output[[4]]['MG.lnk'])").AsVector().First().ToString(), out modMG))
                    {
                        mVM.RowViewModels[mIndex + 1].values[5] = Math.Exp(modMG).ToString(mPrecision);
                    }
                    else
                    {
                        mVM.RowViewModels[mIndex + 1].values[5] = modMG.ToString();
                    }

                    double modR = double.NaN;
                    if (double.TryParse(engine.Evaluate("as.numeric(output[[5]]['Rachlin.lnk'])").AsVector().First().ToString(), out modR))
                    {
                        mVM.RowViewModels[mIndex + 1].values[7] = Math.Exp(modR).ToString(mPrecision);
                    }
                    else
                    {
                        mVM.RowViewModels[mIndex + 1].values[7] = modR.ToString();
                    }

                    //mVM.RowViewModels[mIndex + 1].values[1] = engine.Evaluate("as.numeric(output[[3]]['exp.lnk'])").AsVector().First().ToString();
                    //mVM.RowViewModels[mIndex + 1].values[2] = engine.Evaluate("as.numeric(output[[2]]['Mazur.lnk'])").AsVector().First().ToString();
                    mVM.RowViewModels[mIndex + 1].values[3] = engine.Evaluate("as.numeric(output[[9]]['BD.beta'])").AsVector().AsNumeric().First().ToString(mPrecision);
                    mVM.RowViewModels[mIndex + 1].values[4] = engine.Evaluate("as.numeric(output[[9]]['BD.delta'])").AsVector().AsNumeric().First().ToString(mPrecision);
                    //mVM.RowViewModels[mIndex + 1].values[5] = engine.Evaluate("as.numeric(output[[4]]['MG.lnk'])").AsVector().First().ToString();
                    mVM.RowViewModels[mIndex + 1].values[6] = engine.Evaluate("as.numeric(output[[4]]['MG.s'])").AsVector().AsNumeric().First().ToString(mPrecision);
                    //mVM.RowViewModels[mIndex + 1].values[7] = engine.Evaluate("as.numeric(output[[5]]['Rachlin.lnk'])").AsVector().First().ToString();
                    mVM.RowViewModels[mIndex + 1].values[8] = engine.Evaluate("as.numeric(output[[5]]['Rachlin.s'])").AsVector().AsNumeric().First().ToString(mPrecision);

                    int col = 9;

                    if (!BoundRachHyperboloidModel)
                    {
                        mVM.RowViewModels[mIndex + 1].values[col] = items.First().Key.ToString();
                        col++;
                        mVM.RowViewModels[mIndex + 1].values[col] = ed50Best.ToString(mPrecision);
                        col++;
                    }


                    if (OutputProb && !BoundRachHyperboloidModel)
                    {
                        mVM.RowViewModels[mIndex + 1].values[col] = noiseProb.ToString(mPrecision);
                        col++;

                        mVM.RowViewModels[mIndex + 1].values[col] = exponProb.ToString(mPrecision);
                        col++;

                        mVM.RowViewModels[mIndex + 1].values[col] = hyperProb.ToString(mPrecision);
                        col++;

                        mVM.RowViewModels[mIndex + 1].values[col] = quasiProb.ToString(mPrecision);
                        col++;

                        mVM.RowViewModels[mIndex + 1].values[col] = myerProb.ToString(mPrecision);
                        col++;

                        mVM.RowViewModels[mIndex + 1].values[col] = rachProb.ToString(mPrecision);
                        col++;
                    }

                    if (OutputBIC)
                    {
                        mVM.RowViewModels[mIndex + 1].values[col] = engine.Evaluate("as.numeric(output[[1]]['noise.BIC'])").AsVector().AsNumeric().First().ToString(mPrecision);
                        col++;

                        mVM.RowViewModels[mIndex + 1].values[col] = engine.Evaluate("as.numeric(output[[3]]['exp.BIC'])").AsVector().AsNumeric().First().ToString(mPrecision);
                        col++;

                        mVM.RowViewModels[mIndex + 1].values[col] = engine.Evaluate("as.numeric(output[[2]]['Mazur.BIC'])").AsVector().AsNumeric().First().ToString(mPrecision);
                        col++;

                        mVM.RowViewModels[mIndex + 1].values[col] = engine.Evaluate("as.numeric(output[[9]]['BD.BIC'])").AsVector().AsNumeric().First().ToString(mPrecision);
                        col++;

                        mVM.RowViewModels[mIndex + 1].values[col] = engine.Evaluate("as.numeric(output[[4]]['MG.BIC'])").AsVector().AsNumeric().First().ToString(mPrecision);
                        col++;

                        mVM.RowViewModels[mIndex + 1].values[col] = engine.Evaluate("as.numeric(output[[5]]['Rachlin.BIC'])").AsVector().AsNumeric().First().ToString(mPrecision);
                        col++;
                    }

                    if (OutputAIC)
                    {
                        mVM.RowViewModels[mIndex + 1].values[col] = engine.Evaluate("as.numeric(output[[1]]['noise.AIC'])").AsVector().AsNumeric().First().ToString(mPrecision);
                        col++;

                        mVM.RowViewModels[mIndex + 1].values[col] = engine.Evaluate("as.numeric(output[[3]]['exp.AIC'])").AsVector().AsNumeric().First().ToString(mPrecision);
                        col++;

                        mVM.RowViewModels[mIndex + 1].values[col] = engine.Evaluate("as.numeric(output[[2]]['Mazur.AIC'])").AsVector().AsNumeric().First().ToString(mPrecision);
                        col++;

                        mVM.RowViewModels[mIndex + 1].values[col] = engine.Evaluate("as.numeric(output[[9]]['BD.AIC'])").AsVector().AsNumeric().First().ToString(mPrecision);
                        col++;

                        mVM.RowViewModels[mIndex + 1].values[col] = engine.Evaluate("as.numeric(output[[4]]['MG.AIC'])").AsVector().AsNumeric().First().ToString(mPrecision);
                        col++;

                        mVM.RowViewModels[mIndex + 1].values[col] = engine.Evaluate("as.numeric(output[[5]]['Rachlin.AIC'])").AsVector().AsNumeric().First().ToString(mPrecision);
                        col++;
                    }

                    if (OutputRanks && !BoundRachHyperboloidModel)
                    {
                        int rank = 1;

                        foreach (KeyValuePair<string, double> pair in items)
                        {
                            mVM.RowViewModels[mIndex + 1].values[col] = pair.Key + " Probability (" + pair.Value.ToString(mPrecision) + ")";
                            rank++;
                            col++;
                        }
                    }
                }
                catch (ParseException pe)
                {
                    //Logging.SubmitError("ViewModelBatchDiscounting", pe.ToString());
                    mWindow.OutputEvents(pe.ToString());
                }

                mWindow.OutputEvents("Computation #" + ((int)mIndex + (int)1) + " of " + wholeRange.GetLength(1) + " Completed!");

            }

            mWindow.OutputEvents("Final Calculations Completed!");
            mWindow.OutputEvents("Please remember to cite the packages used in this process!");
            mWindow.OutputEvents("Citation:: Gilroy, S. P., Franck, C. T. & Hantula, D. A. (2016). The Discounting Model Selector: Statistical discounting software.");
            mWindow.OutputEvents("Citation:: Franck, C. T., Koffarnus, M. N., House, L. L. & Bickel, W. K. (2015). Accurate characterization of delay discounting: a multiple model approach using approximate Bayesian model selection and a unified discounting measure. Journal of the Experimental Analysis of Behavior, 103(1), 218-233.");
            mWindow.OutputEvents("Citation:: " + string.Join("", engine.Evaluate("citation()$textVersion").AsCharacter().ToArray()));
            mWindow.OutputEvents("Citation:: " + string.Join("", engine.Evaluate("citation('ggplot2')$textVersion").AsCharacter().ToArray()));
            mWindow.OutputEvents("Citation:: " + string.Join("", engine.Evaluate("citation('gridExtra')$textVersion").AsCharacter().ToArray()));
            mWindow.OutputEvents("Citation:: " + string.Join("", engine.Evaluate("citation('base64enc')$textVersion").AsCharacter().ToArray()));
            mWindow.OutputEvents("Citation:: " + string.Join("", engine.Evaluate("citation('reshape2')$textVersion").AsCharacter().ToArray()));
            mWindow.OutputEvents("Citation:: " + string.Join("", engine.Evaluate("citation('scales')$textVersion").AsCharacter().ToArray()));

            mWin.Show();

            for (int i = 0; i < mWin.dataGrid.Items.Count; i++)
            {
                int col = 11;

                if (BoundRachHyperboloidModel)
                {
                    col = 9;
                }

                var row = GetGridRow(mWin.dataGrid, i);

                if (row == null) continue;

                bool grayOut = true;

                if (OutputProb && !BoundRachHyperboloidModel)
                {
                    if (grayOut)
                    {
                        for (int j = 0; j < 6; j++)
                        {
                            var mCell = mWin.dataGrid.Columns[col].GetCellContent(row).Parent as DataGridCell;
                            mCell.Background = Brushes.LightGray;
                            col++;
                        }
                    }
                    else
                    {
                        col += 6;
                    }

                    grayOut = !grayOut;
                }

                if (OutputBIC)
                {
                    if (grayOut)
                    {
                        for (int j = 0; j < 6; j++)
                        {
                            var mCell = mWin.dataGrid.Columns[col].GetCellContent(row).Parent as DataGridCell;
                            mCell.Background = Brushes.LightGray;
                            col++;
                        }
                    }
                    else
                    {
                        col += 6;
                    }

                    grayOut = !grayOut;
                }

                if (OutputRanks && !BoundRachHyperboloidModel)
                {
                    if (grayOut)
                    {
                        for (int j = 0; j < 6; j++)
                        {
                            var mCell = mWin.dataGrid.Columns[col].GetCellContent(row).Parent as DataGridCell;
                            mCell.Background = Brushes.LightGray;
                            col++;
                        }
                    }
                    else
                    {
                        col += 6;
                    }

                    grayOut = !grayOut;
                }

                /*
                 * Highlight yellow if Noise model won
                 */
                if (mVM.RowViewModels[i].values[9] == "Noise Model" && !BoundRachHyperboloidModel)
                {
                    row.Background = Brushes.Yellow;
                }

                /*
                 * Highlight dark orange of Rachlin won, with bounding
                 */
                if (mVM.RowViewModels[i].values[9] == "Hyperboloid (Rachlin) Model" && BoundRachHyperboloidModel)
                {
                    row.Background = Brushes.DarkOrange;
                }

                if (windowRef != null)
                {
                    windowRef.calculateButton.IsEnabled = true;
                }
            }
        }

        #endregion
    }
}
