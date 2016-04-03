using BayesianModeling.Interfaces;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Documents;
using unvell.ReoGrid.IO;

namespace BayesianModeling
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, SpreadsheetInterface
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Gain focus on startup.
        /// </summary>
        public void GainFocus()
        {
            spreadSheetView.Focus();
        }

        /// <summary>
        /// Clear current sheet.
        /// </summary>
        public bool NewFile()
        {
            spreadSheetView.Reset();
            Title = "Bayesian Model Selection - " + "New File";
            return false;
        }

        /// <summary>
        /// Open file from disk.  Method has includes an open file dialog (win32).
        /// </summary>
        public string[] OpenFile()
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();

            openFileDialog1.Filter = "xlsx Files|*.xlsx";
            openFileDialog1.Title = "Select an Excel File";

            if (openFileDialog1.ShowDialog() == true)
            {
                try
                {
                    using (Stream myStream = openFileDialog1.OpenFile())
                    {
                        spreadSheetView.Load(myStream, FileFormat.Excel2007);
                        Title = "Small n Stats - " + openFileDialog1.SafeFileName;
                    }

                    return new string[] { openFileDialog1.SafeFileName, Path.GetDirectoryName(openFileDialog1.FileName) };
                }
                catch (Exception e)
                {
                    MessageBox.Show("We weren't able to save.  Is the target file open or in use?");
                    Console.WriteLine(e.ToString());
                }
            }

            return null;
        }

        /// <summary>
        /// Save file to disk.  Method has no dialog (CTRL+S option).
        /// </summary>
        /// <param name="path">
        /// String referencing file path, for use in saving file
        /// </param>
        /// <param name="title">
        /// String referencing file name, for use in saving file
        /// </param>
        public void SaveFile(string path, string title)
        {
            try
            {
                using (Stream myStream = new FileStream(Path.Combine(path, title), FileMode.Create))
                {
                    var workbook = spreadSheetView;
                    workbook.Save(myStream, FileFormat.Excel2007);
                    Title = "Small n Stats - " + title;
                }
            }
            catch (Exception e)
            {
                MessageBox.Show("We weren't able to save.  Is the target file open or in use?");
                Console.WriteLine(e.ToString());
            }
        }

        /// <summary>
        /// Save file to disk.  Method includes a save file dialog (win32).
        /// </summary>
        /// <param name="title">
        /// String referencing file name, for use in saving file
        /// </param>
        public string SaveFileWithDialog(string title)
        {
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.FileName = title;
            saveFileDialog1.Filter = "Excel file (*.xlsx)|*.xlsx|All files (*.*)|*.*";

            if (saveFileDialog1.ShowDialog() == true)
            {
                try
                {
                    using (Stream myStream = saveFileDialog1.OpenFile())
                    {
                        var workbook = spreadSheetView;
                        workbook.Save(myStream, FileFormat.Excel2007);
                        title = saveFileDialog1.SafeFileName;
                        Title = "Small n Stats - " + saveFileDialog1.SafeFileName;
                    }

                    return saveFileDialog1.SafeFileName;
                }
                catch (Exception e)
                {
                    MessageBox.Show("We weren't able to save.  Is the target file open or in use?");
                    Console.WriteLine(e.ToString());
                    return null;
                }


            }
            else
            {
                return null;
            }

        }

        /// <summary>
        /// Save file to disk.  Method for saving file specifically for different name.
        /// </summary>
        /// <param name="title">
        /// String referencing file name, for use in saving file
        /// </param>
        public string SaveFileAs(string title)
        {
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();

            saveFileDialog1.FileName = title;
            saveFileDialog1.Filter = "Excel file (*.xlsx)|*.xlsx|All files (*.*)|*.*";

            if (saveFileDialog1.ShowDialog() == true)
            {
                try
                {
                    using (Stream myStream = saveFileDialog1.OpenFile())
                    {
                        var workbook = spreadSheetView;
                        workbook.Save(myStream, FileFormat.Excel2007);
                        title = saveFileDialog1.SafeFileName;
                        Title = "Small n Stats - " + saveFileDialog1.SafeFileName;
                    }

                    return title;
                } 
                catch (Exception e)
                {
                    MessageBox.Show("We weren't able to save.  Is the target file open or in use?");
                    Console.WriteLine(e.ToString());
                    return null;
                }

            }
            else
            {
                return null;
            }

        }

        /// <summary>
        /// Method for closing application from menu.
        /// </summary>
        public void ShutDown()
        {
            Close();
        }

        /// <summary>
        /// Update title to reflect current file in use.
        /// </summary>
        /// <param name="title">
        /// String referencing file name, for use in title
        /// </param>
        public void UpdateTitle(string title)
        {
            Title = "Small n Stats - " + title;
        }

        /// <summary>
        /// A method for submitting a string-encoded range and returning the value of the cells selected.
        /// </summary>
        /// <param name="range">
        /// List of double values returned for use as delay or value points in simulation
        /// </param>
        public List<double> ParseRange(string range)
        {
            List<double> mReturned = new List<double>();

            try
            {
                var rangeReturned = spreadSheetView.CurrentWorksheet.Ranges[range];
                spreadSheetView.CurrentWorksheet.IterateCells(rangeReturned, (row, col, cell) =>
                {
                    double num;
                    if (double.TryParse(cell.Data.ToString(), out num))
                    {
                        mReturned.Add(num);
                    }
                    return true;
                });
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }

            return mReturned;
        }

        /// <summary>
        /// Code-behind to save the text output from logs to a .txt file, selected by user.
        /// </summary>
        private void saveLogs_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog sd = new SaveFileDialog();
            sd.FileName = "Logs";
            sd.Filter = "Text file (*.txt)|*.txt|All files (*.*)|*.*";

            if (sd.ShowDialog() == true)
            {
                using (StreamWriter sw = new StreamWriter(sd.FileName))
                {
                    TextRange textRange = new TextRange(outputWindow2.Document.ContentStart, outputWindow2.Document.ContentEnd);
                    sw.Write(textRange.Text);
                }
            }
        }

        /// <summary>
        /// Code-behind call to flow document, clearing the current content.
        /// </summary>
        private void clearLogs_Click(object sender, RoutedEventArgs e)
        {
            outputWindow2.Document.Blocks.Clear();
        }
    }
}
