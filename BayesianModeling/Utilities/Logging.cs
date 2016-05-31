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
