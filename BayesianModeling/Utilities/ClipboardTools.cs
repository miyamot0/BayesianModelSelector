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
using System.Linq;
using System.Windows;

namespace BayesianModeling.Utilities
{
    class ClipboardTools
    {
        delegate string[] ParseFormat(string value);

        public static List<string[]> ReadAndParseClipboardData()
        {
            List<string[]> clipboardData = new List<string[]>();
            object clipboardRawData = null;
            IDataObject clipboadDataObj = Clipboard.GetDataObject();

            string[] rows;

            if ((clipboardRawData = clipboadDataObj.GetData(DataFormats.CommaSeparatedValue)) != null)
            {
                rows = (clipboardRawData as string).Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);

                if (rows == null)
                    return clipboardData;

                rows.ToList().ForEach(x => clipboardData.Add(ParseCommaSeparatedFormat(x)));
            }
            else if ((clipboardRawData = clipboadDataObj.GetData(DataFormats.Text)) != null)
            {
                rows = (clipboardRawData as string).Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);

                if (rows == null)
                    return clipboardData;

                rows.ToList().ForEach(x => clipboardData.Add(ParseTabbedSeparatedFormat(x)));
            }

            return clipboardData;
        }

        private static string[] ParseCommaSeparatedFormat(string value)
        {
            List<string> returnList = new List<string>();
            char separator = ',';

            int begin = 0, 
                end = 0;

            for (int i = 0; i < value.Length; i++)
            {
                if (value[i] == separator)
                {
                    returnList.Add(value.Substring(begin, end - begin));

                    begin = end + 1;
                    end = begin;
                }
                else if (value[i] == '\"')
                { /*Possible escaped character*/
                    i++;
                    if (i >= value.Length)
                    { /*Possibly just at the end, in case break*/
                        break;
                    }

                    char charHolder = value[i];

                    while (charHolder != '\"' && i < value.Length)
                    { /*Continue reading until unescaped or over*/
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

        private static string[] ParseTabbedSeparatedFormat(string value)
        {
            List<string> returnList = new List<string>();
            char separator = '\t';

            int begin = 0, 
                end = 0;

            for (int i = 0; i < value.Length; i++)
            {
                if (value[i] == separator)
                {
                    returnList.Add(value.Substring(begin, end - begin));

                    begin = end + 1;
                    end = begin;
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
    }
}
