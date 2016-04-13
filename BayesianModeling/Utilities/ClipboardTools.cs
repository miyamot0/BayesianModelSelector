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

using System;
using System.Collections.Generic;
using System.Windows;

namespace BayesianModeling.Utilities
{
    class ClipboardTools
    {
        delegate string[] ParseFormat(string value);

        static char mComma = ',';
        static char mTab = '\t';

        private static string[] ParseCsvOrTextFormat(string value, bool isCommaDoc)
        {
            List<string> returnList = new List<string>();
            char separator;

            if (isCommaDoc)
            {
                separator = mComma;
            }
            else
            {
                separator = mTab;
            }

            int begin = 0, end = 0;

            for (int i = 0; i < value.Length; i++)
            {
                char ch = value[i];

                /* If delimit, begin a new item for list */
                if (ch == separator)
                {
                    returnList.Add(value.Substring(begin, end - begin));

                    begin = end + 1;
                    end = begin;
                }
                else if (isCommaDoc && ch == '\"')
                {
                    /* Loop to next relevant item */
                    i++;
                    if (i >= value.Length)
                    {
                        return new string[] { "Error with parsing" };
                    }

                    char tempCh = value[i];

                    while (tempCh != '\"' && i < value.Length)
                    {
                        i++;
                    }

                    end = i;
                }
                else if (i + 1 == value.Length)
                {
                    returnList.Add(value.Substring(begin));

                    break;
                }
                else
                {
                    end++;
                }
            }

            return returnList.ToArray();
        }

        public static List<string[]> ReadAndParseClipboardData()
        {
            List<string[]> clipboardData = new List<string[]>();
            object clipboardRawData = null;
            IDataObject clipboadDataObj = System.Windows.Clipboard.GetDataObject();

            if ((clipboardRawData = clipboadDataObj.GetData(DataFormats.CommaSeparatedValue)) != null)
            {
                string[] rows = (clipboardRawData as string).Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);

                if (rows != null && rows.Length > 0)
                {
                    foreach (string row in rows)
                    {
                        clipboardData.Add(ParseCsvOrTextFormat(row, true));
                    }
                }
            }
            else if ((clipboardRawData = clipboadDataObj.GetData(DataFormats.Text)) != null)
            {
                string[] rows = (clipboardRawData as string).Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);

                if (rows != null && rows.Length > 0)
                {
                    foreach (string row in rows)
                    {
                        clipboardData.Add(ParseCsvOrTextFormat(row, false));
                    }
                }
            }

            return clipboardData;
        }
    }
}
