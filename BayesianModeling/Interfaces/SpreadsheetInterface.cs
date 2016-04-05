/*
 * Shawn Gilroy, 2016
 * Bayesian Model Selection Application
 * Interface between main view and child views ( Spreadsheet data <> business layer)
 * 
 *  */
namespace BayesianModeling.Interfaces
{
    public interface SpreadsheetInterface
    {
        void UpdateTitle(string Title);

        void GainFocus();

        bool NewFile();

        void SaveFile(string path, string title);

        string SaveFileWithDialog(string title);

        string SaveFileAs(string title);

        string[] OpenFile();

        void ShutDown();
    }
}
