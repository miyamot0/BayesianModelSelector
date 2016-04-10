/*
 * Shawn Gilroy, 2016
 * Bayesian Model Selection Application
 * Based on conceptual work developed by Franck, C. T., Koffarnus, M. N., House, L. L. & Bickel, W. (2015)
 * 
 */

using BayesianModeling.Mathematics;
using BayesianModeling.Utilities;
using BayesianModeling.View;
using RDotNet;
using Small_N_Stats.Interface;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System;
using unvell.ReoGrid.Actions;
using unvell.ReoGrid;

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

        internal OutputWindowInterface mInterface;

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

        /* Math/Computation */

        REngine engine;

        /* Commands */

        public RelayCommand ViewLoadedCommand { get; set; }
        public RelayCommand ViewClosingCommand { get; set; }
        public RelayCommand GetDelaysRangeCommand { get; set; }
        public RelayCommand GetValuesRangeCommand { get; set; }
        public RelayCommand GetDelayedValuesRangeCommand { get; set; }
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
            GetDelayedValuesRangeCommand = new RelayCommand(param => GetDelayedValuesRange(), param => true);
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

            mInterface.SendMessageToOutput("---------------------------------------------------");
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
                mInterface.SendMessageToOutput(e.ToString());
                failed = true;
            }

            if (failed)
            {
                mInterface.SendMessageToOutput("R components modules were not found!");
                mInterface.SendMessageToOutput("Calculation cannot continue");
                mInterface.SendMessageToOutput("Connect to the internet and re-start the program");
                mInterface.SendMessageToOutput("");
                mInterface.SendMessageToOutput("");

                MessageBox.Show("Modules for R were not found.  Please connect to the internet and restart the program.");
            }
            else
            {
                mInterface.SendMessageToOutput("All R system components modules loaded.");
                mInterface.SendMessageToOutput("Loading Curve Fitting modules and R interface...");
                mInterface.SendMessageToOutput("");
                mInterface.SendMessageToOutput("");
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

        /// <summary>
        /// Call window reference (shameful deviation from MVVM) for Unveil's range PickRange function.
        /// Successful (or failing) selections result in a range string in respective text fields for later parsing.
        /// </summary>
        private void GetDelaysRange()
        {
            DefaultFieldsToGray();

            mWindow.spreadSheetView.CurrentWorksheet.SelectionMode = WorksheetSelectionMode.Range;

            if (Delays.Length > 0 && !Delays.ToLower().Contains("spreadsheet"))
            {
                /* Restore past ranges to white */
                mWindow.spreadSheetView.CurrentWorksheet.SetRangeStyles(mWindow.spreadSheetView.CurrentWorksheet.Ranges[Delays], new WorksheetRangeStyle
                {
                    Flag = PlainStyleFlag.BackColor,
                    BackColor = System.Windows.Media.Colors.Transparent,
                });
            }

            DelaysBrush = Brushes.Yellow;
            Delays = "Select delays on spreadsheet";

            mWindow.spreadSheetView.PickRange((inst, range) =>
            {

                if (range.Rows > 1 && range.Cols > 1)
                {
                    DefaultFieldsToGray();
                    MessageBox.Show("Please select single row or single column selections");
                    return true;
                }

                DelaysBrush = Brushes.LightBlue;
                Delays = range.ToString();

                mWindow.spreadSheetView.CurrentWorksheet.SetRangeStyles(range, new WorksheetRangeStyle
                {
                    Flag = PlainStyleFlag.BackColor,
                    BackColor = System.Windows.Media.Colors.LightBlue,
                });

                mWindow.spreadSheetView.CurrentWorksheet.SelectionMode = WorksheetSelectionMode.Range;

                return true;
            }, Cursors.Cross);
        }

        /// <summary>
        /// Call window reference (shameful deviation from MVVM) for Unveil's range PickRange function.
        /// Successful (or failing) selections result in a range string in respective text fields for later parsing.
        /// </summary>
        private void GetValuesRange()
        {
            DefaultFieldsToGray();

            mWindow.spreadSheetView.CurrentWorksheet.SelectionMode = WorksheetSelectionMode.Range;

            if (Values.Length > 0 && !Values.ToLower().Contains("spreadsheet"))
            {
                /* Restore past ranges to white */
                mWindow.spreadSheetView.CurrentWorksheet.SetRangeStyles(mWindow.spreadSheetView.CurrentWorksheet.Ranges[Values], new WorksheetRangeStyle
                {
                    Flag = PlainStyleFlag.BackColor,
                    BackColor = System.Windows.Media.Colors.Transparent,
                });
            }

            ValuesBrush = Brushes.Yellow;
            Values = "Select values on spreadsheet";

            mWindow.spreadSheetView.PickRange((inst, range) =>
            {
                ValuesBrush = Brushes.LightGreen;
                Values = range.ToString();

                mWindow.spreadSheetView.CurrentWorksheet.SetRangeStyles(range, new WorksheetRangeStyle
                {
                    Flag = PlainStyleFlag.BackColor,
                    BackColor = System.Windows.Media.Colors.LightGreen,
                });

                mWindow.spreadSheetView.CurrentWorksheet.SelectionMode = WorksheetSelectionMode.Range;

                return true;
            }, Cursors.Cross);
        }

        /// <summary>
        /// Call window reference (shameful deviation from MVVM) for Unveil's range PickRange function.
        /// Successful (or failing) selections result in a range string in respective text fields for later parsing.
        /// </summary>
        private void GetDelayedValuesRange()
        {
            DefaultFieldsToGray();

            if (DelayedValue.Length > 0 && !DelayedValue.ToLower().Contains("spreadsheet"))
            {
                /* Restore past ranges to white */
                mWindow.spreadSheetView.CurrentWorksheet.SetRangeStyles(mWindow.spreadSheetView.CurrentWorksheet.Ranges[DelayedValue], new WorksheetRangeStyle
                {
                    Flag = PlainStyleFlag.BackColor,
                    BackColor = System.Windows.Media.Colors.Transparent,
                });
            }

            DelayedValueBrush = Brushes.Yellow;
            DelayedValue = "Select values on spreadsheet";

            mWindow.spreadSheetView.PickRange((inst, range) =>
            {
                if (range.Rows > 1 && range.Cols > 1)
                {
                    DefaultFieldsToGray();
                    MessageBox.Show("Please select single row or single column selections");
                    return true;
                }

                DelayedValueBrush = Brushes.LightCoral;
                DelayedValue = range.ToString();

                mWindow.spreadSheetView.CurrentWorksheet.SetRangeStyles(range, new WorksheetRangeStyle
                {
                    Flag = PlainStyleFlag.BackColor,
                    BackColor = System.Windows.Media.Colors.LightCoral,
                });

                return true;
            }, Cursors.Cross);
        }

        /// <summary>
        /// Command-call to calculate based on supplied ranges and reference values (max value).
        /// Will reference user-selected options (figures, outputs, etc.) throughout calls to R
        /// </summary>
        private void CalculateScores()
        {
            if (failed) return;

            List<double> xRange = mWindow.ParseRange(Delays);
            double[,] wholeRange = mWindow.ParseBulkRange(Values);

            mInterface.SendMessageToOutput("---------------------------------------------------");
            mInterface.SendMessageToOutput("Checking user-supplied ranges and reference points.");

            List<double> yRange = new List<double>();

            if (wholeRange == null)
            {
                mInterface.SendMessageToOutput("There were items that failed validation in the Indifference Point values.  Are any fields blank or not numeric?");
                MessageBox.Show("There were items that failed validation in the Indifference Point values.");
                return;
            }

            for (int i = 0; i < wholeRange.GetLength(1); i++)
            {
                /* Loading Initial Range*/
                yRange.Add(wholeRange[0, i]);
            }

            if (!double.TryParse(DelayedValue, out MaxValueA) || MaxValueA == 0)
            {
                mInterface.SendMessageToOutput("Error while validating the Delayed Amount.  Is this a non-zero number?");
                MessageBox.Show("Please review the the Delayed Amount number.  This must be a non-zero number.");
                return;
            }

            if (xRange.Count != yRange.Count)
            {
                mInterface.SendMessageToOutput("Error while validating current ranges, Delay/Value ranges must be EQUAL in length for comparison.");
                mInterface.SendMessageToOutput("Counts for Delays/Values were " + xRange.Count + " and " + yRange.Count + " respectively.");
                MessageBox.Show("Error while validating current ranges, Delay/Value ranges must be EQUAL in length for comparison.");
                return;
            }

            if ((yRange[0] / MaxValueA) <= 0.1)
            {
                MessageBox.Show("There's a chance your max value is off (the initial value is <10% of the max already).  If this is expected, please disregard.");
                mInterface.SendMessageToOutput("Initial indifference point was <10% of A.  This is irregular, please inspect.  If this is accurate, disregard.");
            }

            if (yRange[0] > MaxValueA)
            {
                MessageBox.Show("Your Delayed Amount appears incorrect (the first Indifference Point is greater than the Delayed Amount).  This shouldn't be possible.");
                mInterface.SendMessageToOutput("Initial indifference point is greater than Delayed Amount.  This shouldn't be possible.  Halting Computation.");
                return;
            }

            mInterface.SendMessageToOutput("All inputs passed verification.");
            mInterface.SendMessageToOutput("---------------------------------------------------");
            mInterface.SendMessageToOutput("Beginning Batched Computations...");

            var sheet = mWindow.spreadSheetView.CreateWorksheet();

            for (int mIndex = 0; mIndex < wholeRange.GetLength(0); mIndex++)
            {
                engine.Evaluate("rm(list = setdiff(ls(), lsf.str()))");

                yRange.Clear();

                for (int i = 0; i < wholeRange.GetLength(1); i++)
                {
                    yRange.Add(wholeRange[mIndex, i]);
                }

                try
                {
                    NumericVector delayValues = engine.CreateNumericVector(xRange.ToArray());
                    engine.SetSymbol("mDelays", delayValues);

                    List<double> yRangeMod = new List<double>();

                    for (int i = 0; i < wholeRange.GetLength(1); i++)
                    {
                        yRangeMod.Add(wholeRange[mIndex, i]);
                    }

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

                    NumericVector aSymbol = engine.Evaluate(MaxValueA.ToString()).AsNumeric();
                    engine.SetSymbol("A", aSymbol);

                    engine.SetSymbol("mDelays", delayValues);

                    indiffValues = engine.CreateNumericVector(mWindow.ParseRange(Values));
                    engine.SetSymbol("mIndiff", indiffValues);

                    engine.Evaluate("endDelay <- max(mDelays)*10");
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

                sheet[0, mIndex+1] = "Series #" + (int)(mIndex + 1);

                if (mIndex == 0)
                {
                    sheet[1, 0] = "Results of Fittings:";
                    sheet[3, 0] = "Exponential - ln(k): ";
                    sheet[5, 0] = "Hyperbolic - ln(k): ";
                    sheet[7, 0] = "Quasi-Hyperbolic (beta): ";
                    sheet[8, 0] = "Quasi-Hyperbolic (delta): ";
                    sheet[10, 0] = "Myerson-Hyperboloid - ln(k): ";
                    sheet[11, 0] = "Myerson-Hyperboloid (s): ";
                    sheet[13, 0] = "Rachlin-Hyperboloid (k): ";
                    sheet[14, 0] = "Rachlin-Hyperboloid (s): ";

                    sheet[18, 0] = "Model Competition (#1)";
                    sheet[19, 0] = "#2";
                    sheet[20, 0] = "#3";
                    sheet[21, 0] = "#4";
                    sheet[22, 0] = "#5";
                    sheet[23, 0] = "#6";

                    sheet[25, 0] = "Most competitive model: ";
                    sheet[26, 0] = "ED50 of Most Competitive Model - ln(x): ";
                }

                int row = 18;
                foreach (KeyValuePair<string, double> pair in items)
                {
                    sheet[row, mIndex + 1] = pair.Key + " - (" + pair.Value.ToString("0.000") + ")";
                    row++;
                }

                sheet[3, mIndex + 1] = engine.Evaluate("as.numeric(output[[3]]['exp.lnk'])").AsVector().First().ToString();
                sheet[5, mIndex + 1] = engine.Evaluate("as.numeric(output[[2]]['Mazur.lnk'])").AsVector().First().ToString();
                sheet[7, mIndex + 1] = engine.Evaluate("as.numeric(output[[9]]['BD.beta'])").AsVector().First().ToString();
                sheet[8, mIndex + 1] = engine.Evaluate("as.numeric(output[[9]]['BD.delta'])").AsVector().First().ToString();
                sheet[10, mIndex + 1] = engine.Evaluate("as.numeric(output[[4]]['MG.lnk'])").AsVector().First().ToString();
                sheet[11, mIndex + 1] = engine.Evaluate("as.numeric(output[[4]]['MG.s'])").AsVector().First().ToString();
                sheet[13, mIndex + 1] = engine.Evaluate("as.numeric(output[[5]]['Rachlin.lnk'])").AsVector().First().ToString();
                sheet[14, mIndex + 1] = engine.Evaluate("as.numeric(output[[5]]['Rachlin.s'])").AsVector().First().ToString();

                sheet[25, mIndex + 1] = items.First().Key.ToString();

                double ed50Best = engine.Evaluate("as.numeric(output[[8]]['lnED50.mostprob'])").AsNumeric().First();

                sheet[26, mIndex + 1] = ed50Best.ToString();

                mInterface.SendMessageToOutput("Computation #" + ((int)mIndex + (int)1) + " of " + wholeRange.GetLength(0) + " Completed!");
            }

            mWindow.spreadSheetView.AddWorksheet(sheet);
            mWindow.spreadSheetView.CurrentWorksheet = sheet;

            var action = new SetColumnsWidthAction(0, wholeRange.GetLength(1), 250);
            mWindow.spreadSheetView.DoAction(action);

            mInterface.SendMessageToOutput("Final Calculations Completed!");

        }
    }
}
