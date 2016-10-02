//----------------------------------------------------------------------------------------------
// <copyright file="Logging.cs" 
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

using System.Collections.Specialized;
using System.Net;

namespace BayesianModeling.Utilities
{
    class Logging
    {
        public static byte[] SubmitLogs(NameValueCollection pairs)
        {
            byte[] webResponse = null;

            try
            {
                using (WebClient client = new WebClient())
                {
                    webResponse = client.UploadValues("http://www.smallnstats.com/error_logs.php", pairs);
                }
            }
            catch
            {
                return null;
            }

            return webResponse;
        }

        public static void SubmitError(string sysError, string sysContent)
        {
            NameValueCollection coll = new NameValueCollection();
            coll.Add("tag", "smallntests");
            coll.Add("uid", Properties.Settings.Default.GUID);
            coll.Add("error", sysError);
            coll.Add("content", sysContent);

            var siteResponse = SubmitLogs(coll);
        }
    }
}
