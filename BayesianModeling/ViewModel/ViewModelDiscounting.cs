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
    /// MVVM-ish Interaction logic for DiscountingWindow.xaml.  
    /// Separation of R-based calculations from View (as much as possible)
    /// </summary>
    class ViewModelDiscounting : ViewModelBase
    {
        public MainWindow mWindow { get; set; }
        public DiscountingWindow windowRef { get; set; }

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
            mInterface.SendMessageToOutput("---------------------------------------------------");

            failed = false;

            try
            {
                REngine.SetEnvironmentVariables();
                engine = REngine.GetInstance();
                engine.Initialize();
                engine.AutoPrint = false;

                if (!engine.Evaluate("require('ggplot2')").AsVector().First().ToString().Equals("True") ||
                    !engine.Evaluate("require('reshape2')").AsVector().First().ToString().Equals("True") ||
                    !engine.Evaluate("require('gridExtra')").AsVector().First().ToString().Equals("True"))
                {
                    failed = true;
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
            }
            catch (Exception e)
            {
                failed = true;
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
        /// Call window reference (shameful deviation from MVVM) for Unveil's range PickRange function.
        /// Successful (or failing) selections result in a range string in respective text fields for later parsing.
        /// </summary>
        private void GetDelaysRange()
        {
            DefaultFieldsToGray();

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
                if (range.Rows > 1 && range.Cols > 1)
                {
                    DefaultFieldsToGray();
                    MessageBox.Show("Please select single row or single column selections");
                    return true;
                }

                ValuesBrush = Brushes.LightGreen;
                Values = range.ToString();

                mWindow.spreadSheetView.CurrentWorksheet.SetRangeStyles(range, new WorksheetRangeStyle
                {
                    Flag = PlainStyleFlag.BackColor,
                    BackColor = System.Windows.Media.Colors.LightGreen,
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
            List<double> yRange = mWindow.ParseRange(Values);

            mInterface.SendMessageToOutput("---------------------------------------------------");
            mInterface.SendMessageToOutput("Checking user-supplied ranges and reference points.");

            if (!double.TryParse(MaxValue, out maxValueA) || (xRange.Count != yRange.Count))
            {
                mInterface.SendMessageToOutput("Error while validating current ranges, Delay/Value ranges must be EQUAL in length for comparison.");
                mInterface.SendMessageToOutput("Counts for Delays/Values were " + xRange.Count + " and " + yRange.Count + " respectively.");
                mInterface.SendMessageToOutput("Reference for the maximum value was: " + maxValueA);
                MessageBox.Show("Please review the Delay/Value ranges and maximum value reference point.");
                return;
            }

            if ((yRange[0] / maxValueA) <= 0.1)
            {
                MessageBox.Show("There's a chance your max value is off (the initial value is <10% of the max already).  If this is expected, please disregard.");
                mInterface.SendMessageToOutput("Initial indifference point was <10% of A.  This is irregular, please inspect.  If this is accurate, disregard.");
            }

            if (yRange[0] >= maxValueA)
            {
                MessageBox.Show("There's a chance your max value is off (the initial value is greater than the max).  This shouldn't be possible.");
                mInterface.SendMessageToOutput("Initial value is greater than A.  This shouldn't be possible.  Halting simulation.");
                return;
            }

            mInterface.SendMessageToOutput("Inputs passed verification.");
            mInterface.SendMessageToOutput("Figure output: " + outputFigures);
            mInterface.SendMessageToOutput("Workbook output: " + outputWorkbook);
            mInterface.SendMessageToOutput("Beginning Bayesian Simulation...");
               
            try
            {
                NumericVector delayValues = engine.CreateNumericVector(mWindow.ParseRange(Delays).ToArray());
                engine.SetSymbol("mDelays", delayValues);

                List<double> yRangeMod = new List<double>(mWindow.ParseRange(Values));

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

                indiffValues = engine.CreateNumericVector(mWindow.ParseRange(Values));
                engine.SetSymbol("mIndiff", indiffValues);

                engine.Evaluate("endDelay <- max(mDelays)*10");
            }
            catch (ParseException pe)
            {
                Console.WriteLine(pe.ToString());
            }

            mInterface.SendMessageToOutput("Simulation successful!");
            mInterface.SendMessageToOutput(" ");

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

            mInterface.SendMessageToOutput("Results of Model competition (output Highest to Lowest):");
            mInterface.SendMessageToOutput(" ");

            foreach (KeyValuePair<string, double> pair in items)
            {
                  mInterface.SendMessageToOutput(pair.Key + ":  " + pair.Value + " ");
            }

            mInterface.SendMessageToOutput(" ");
            mInterface.SendMessageToOutput("Simulation Completed");
            mInterface.SendMessageToOutput("---------------------------------------------------");

            if (outputFigures)
            {
                mInterface.SendMessageToOutput("Charting Started... Please wait... ");
                engine.Evaluate(BayesianModelSelection.GetLogChartFunction());
                mInterface.SendMessageToOutput("Charting Completed!");
            }

            if (outputWorkbook)
            {
                mInterface.SendMessageToOutput("Outputting to workbook Started... Please wait... ");
                var sheet = mWindow.spreadSheetView.CreateWorksheet();
                sheet.ColumnHeaders["A"].IsAutoWidth = true;
                sheet.ColumnHeaders["B"].IsAutoWidth = true;

                sheet["A22"] = "Model Competition (Best to Worst)";

                string left = string.Empty;
                string right = string.Empty;
                int row = 23;

                foreach (KeyValuePair<string, double> pair in items)
                {
                    left = "A" + row;
                    right = "B" + row;
                    sheet[left] = pair.Key;
                    sheet[right] = pair.Value;
                    row++;
                }

                sheet["A2"] = "Delays: ";
                sheet["A3"] = "Values: ";

                for (int i=0; i< xRange.Count; i++)
                {
                    sheet[1 , 1 + i] = xRange[i].ToString();
                    sheet[2 , 1 + i] = yRange[i].ToString();
                }

                sheet["A6"] = "Results of Fittings:";

                sheet["A8"] = "Exponential - ln(k): ";
                sheet["B8"] = engine.Evaluate("as.numeric(output[[3]]['exp.lnk'])").AsVector().First().ToString();
                sheet["A10"] = "Hyperbolic - ln(k): ";
                sheet["B10"] = engine.Evaluate("as.numeric(output[[2]]['Mazur.lnk'])").AsVector().First().ToString();
                sheet["A12"] = "Quasi-Hyperbolic (beta): ";
                sheet["B12"] = engine.Evaluate("as.numeric(output[[9]]['BD.beta'])").AsVector().First().ToString();
                sheet["A13"] = "Quasi-Hyperbolic (delta): ";
                sheet["B13"] = engine.Evaluate("as.numeric(output[[9]]['BD.delta'])").AsVector().First().ToString();
                sheet["A15"] = "Myerson-Hyperboloid - ln(k): ";
                sheet["B15"] = engine.Evaluate("as.numeric(output[[4]]['MG.lnk'])").AsVector().First().ToString();
                sheet["A16"] = "Myerson-Hyperboloid (s): ";
                sheet["B16"] = engine.Evaluate("as.numeric(output[[4]]['MG.s'])").AsVector().First().ToString();
                sheet["A18"] = "Rachlin-Hyperboloid (k): ";
                sheet["B18"] = engine.Evaluate("as.numeric(output[[5]]['Rachlin.lnk'])").AsVector().First().ToString();
                sheet["A19"] = "Rachlin-Hyperboloid (s): ";
                sheet["B19"] = engine.Evaluate("as.numeric(output[[5]]['Rachlin.s'])").AsVector().First().ToString();

                sheet["A30"] = "Most competitive model: ";
                sheet["B30"] = items.First().Key.ToString();

                double ed50Best = engine.Evaluate("as.numeric(output[[8]]['lnED50.mostprob'])").AsNumeric().First();

                sheet["A31"] = "ED50 of Most Competitive Model - ln(x): ";
                sheet["B31"] = ed50Best.ToString();


                mWindow.spreadSheetView.AddWorksheet(sheet);
                mWindow.spreadSheetView.CurrentWorksheet = sheet;

                var action = new SetColumnsWidthAction(0, 1, 200);
                mWindow.spreadSheetView.DoAction(action);
                action = new SetColumnsWidthAction(1, 1, 200);
                mWindow.spreadSheetView.DoAction(action);

                mInterface.SendMessageToOutput("Output Completed!");
            }
        }
    }
}
