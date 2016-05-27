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

using BayesianModeling.ViewModel;
using System.Collections.ObjectModel;

namespace BayesianModeling.Utilities
{
    public class DataGridTools
    {
        static string letters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";

        /// <summary>
        /// Conversion of integer to alphabet-based index (similar to Spreadsheet column indexing workflow).
        /// <param name="index">
        /// Takes integer, converting to referenced letters string, concatenating as necessary
        /// </param>
        /// </summary>
        public static string GetColumnName(int index)
        {
            var value = "";

            if (index >= letters.Length)
            {
                // Add front-based character based on # of iterations past alphabet length
                value = value + letters[index / letters.Length - 1];
            }

            // Add trailing character based on modulus (% 26) index
            value = value + letters[index % letters.Length];

            return value;
        }

        /// <summary>
        /// Conversion of string to alphabet-referenced index (similar to Spreadsheet column indexing).
        /// <param name="location">
        /// Takes string, converting to integer referencing letters string
        /// </param>
        /// </summary>
        public static int GetColumnIndex(string location)
        {
            location = location.ToUpper();
            return ((location.Length - 1) * 26) + letters.IndexOf(location[location.Length - 1]);
        }

        /// <summary>
        /// Linq companion for referencing object's location in collection.
        /// </summary>
        /// <param name="model">
        /// Individual row model reference
        /// </param>
        /// <param name="coll">
        /// Collection overall
        /// </param>
        /// <returns>
        /// int-based index
        /// </returns>
        public static int GetIndexViewModel(RowViewModel model, ObservableCollection<RowViewModel> coll)
        {
            return coll.IndexOf(model);
        }
    }
}
