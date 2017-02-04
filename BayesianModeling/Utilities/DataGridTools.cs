//----------------------------------------------------------------------------------------------
// <copyright file="DataGridTools.cs" 
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
// </summary>
//----------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;

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
        /// Walk through ranged values as needed, finding necessary pairs
        /// </summary>
        /// <param name="startColDelay">
        /// First column index for delay
        /// </param>
        /// <param name="endColDelay">
        /// Final column index for delay
        /// </param>
        /// <param name="rowDelay">
        /// Row index for delays
        /// </param>
        /// <param name="startColValue">
        /// First column index for values
        /// </param>
        /// <param name="endColValue">
        /// Final column index for values
        /// </param>
        /// <param name="rowValue">
        /// Row index for values
        /// </param>
        /// <returns>
        /// List of all range/value pairs that correspond
        /// </returns>
        public static List<double>[] GetRangedValuesHorizontal(int startColDelay, int endColDelay, int rowDelay, int startColValue, int endColValue, int rowValue)
        {
            List<double>[] array = new List<double>[2];
            array[0] = new List<double>();
            array[1] = new List<double>();

            string mCellDelay, mCellValue;

            double testDelay = -1,
                   testValue = -1;

            int i = startColDelay,
                j = startColValue;

            var itemSource = App.Workbook.CurrentWorksheet;

            for (; i <= endColDelay && j <= endColValue;)
            {
                mCellDelay = itemSource.CreateAndGetCell(rowDelay, i).Data.ToString();
                mCellValue = itemSource.CreateAndGetCell(rowValue, j).Data.ToString();

                //mCellDelay = itemSource[rowDelay].values[i];
                //mCellValue = itemSource[rowValue].values[j];

                if (Double.TryParse(mCellDelay, out testDelay) && Double.TryParse(mCellValue, out testValue))
                {
                    array[0].Add(testDelay);
                    array[1].Add(testValue);
                }

                i++;
                j++;
            }

            return array;
        }

        /// <summary>
        /// Walk through ranged values as needed, finding necessary pairs
        /// </summary>
        /// <param name="startColDelay">
        /// First column index for delay
        /// </param>
        /// <param name="endColDelay">
        /// Final column index for delay
        /// </param>
        /// <param name="rowDelay">
        /// Row index for delays
        /// </param>
        /// <param name="startColValue">
        /// First column index for values
        /// </param>
        /// <param name="endColValue">
        /// Final column index for values
        /// </param>
        /// <param name="rowValue">
        /// Row index for values
        /// </param>
        /// <returns>
        /// List of all range/value pairs that correspond
        /// </returns>
        public static List<double>[] GetRangedValuesVertical(int startRowDelay, int endRowDelay, int colDelay, int startRowValue, int endRowValue, int colValue)
        {
            List<double>[] array = new List<double>[2];
            array[0] = new List<double>();
            array[1] = new List<double>();

            string mCellDelay, mCellValue;

            double testDelay = -1,
                   testValue = -1;

            int i = startRowDelay,
                j = startRowValue;

            var itemSource = App.Workbook.CurrentWorksheet;

            for (; i <= endRowDelay && j <= endRowValue;)
            {
                mCellDelay = itemSource.CreateAndGetCell(i, colDelay).Data.ToString();
                mCellValue = itemSource.CreateAndGetCell(j, colValue).Data.ToString();

                if (Double.TryParse(mCellDelay, out testDelay) && Double.TryParse(mCellValue, out testValue))
                {
                    array[0].Add(testDelay);
                    array[1].Add(testValue);
                }

                i++;
                j++;
            }

            return array;
        }

        /// <summary>
        /// Function for parsing values of individual cells by referencing view model
        /// </summary>
        public static List<double> GetRangedValuesVM(int startCol, int endCol, int startRow)
        {
            if (startCol == -1 || startRow == -1) return null;

            var itemSource = App.Workbook.CurrentWorksheet;

            List<double> mRange = new List<double>();

            double test;

            for (int i = startCol; i <= endCol; i++)
            {
                string mRowItem = itemSource.CreateAndGetCell(startRow, i).Data.ToString();

                //string mRowItem = itemSource[startRow, i].ToString();

                if (!Double.TryParse(mRowItem, out test))
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
        /// Function for parsing values of individual cells by referencing view model
        /// </summary>
        public static List<double> GetRangedValuesVerticalVM(int startRow, int endRow, int col)
        {
            List<double> mRange = new List<double>();

            if (startRow == -1 && endRow == -1)
            {
                return null;
            }

            var itemSource = App.Workbook.CurrentWorksheet;

            double test;

            for (int i = startRow; i <= endRow; i++)
            {
                string mRowItemCell = itemSource.CreateAndGetCell(i, col).Data.ToString();
                //string mRowItemCell = itemSource[i].values[col];

                if (!Double.TryParse(mRowItemCell, out test))
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
        public static string[,] ParseBulkRangeStringsVM(int lowRowValue, int highRowValue, int lowColValue, int highColValue)
        {
            string[,] mDouble = null;

            var itemSource = App.Workbook.CurrentWorksheet;

            double tempHolder;
            List<double> tempHolderList = new List<double>();

            int mRows = (highRowValue - lowRowValue) + 1;
            int mCols = (highColValue - lowColValue) + 1;

            mDouble = new string[mCols, mRows];

            try
            {
                for (int i = lowRowValue; i <= highRowValue; i++)
                {
                    for (int j = lowColValue; j <= highColValue; j++)
                    {
                        string mRowItem = itemSource.CreateAndGetCell(i, j).Data.ToString();
                        //string mRowItem = itemSource[i].values[j];
                        mDouble[j - lowColValue, i - lowRowValue] = mRowItem;

                        if (double.TryParse(mRowItem, out tempHolder))
                        {
                            tempHolderList.Add(tempHolder);
                        }
                    }
                }
            }
            catch
            {
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
        public static string[,] ParseBulkRangeStringsVerticalVM(int lowRowValue, int highRowValue, int lowColValue, int highColValue)
        {
            string[,] mDouble = null;

            var itemSource = App.Workbook.CurrentWorksheet;

            double tempHolder;
            List<double> tempHolderList = new List<double>();

            int mRows = (highRowValue - lowRowValue) + 1;
            int mCols = (highColValue - lowColValue) + 1;

            mDouble = new string[mRows, mCols];

            try
            {

                for (int i = lowRowValue; i <= highRowValue; i++)
                {
                    for (int j = lowColValue; j <= highColValue; j++)
                    {
                        //string mRowItem = itemSource[i].values[j];
                        string mRowItem = itemSource.CreateAndGetCell(i, j).Data.ToString();
                        mDouble[i - lowRowValue, j - lowColValue] = mRowItem;

                        if (double.TryParse(mRowItem, out tempHolder))
                        {
                            tempHolderList.Add(tempHolder);
                        }
                    }
                }
            }
            catch
            {
                return null;
            }

            return mDouble;
        }

    }
}
