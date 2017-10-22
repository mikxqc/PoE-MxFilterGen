using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace PoE_MxFilterGen
{
    class web
    {
        public static void SaveString(string url, string path)
        {
            WebClient wb = new WebClient();
            wb.Encoding = Encoding.UTF8;
            try
            {
                Uri uri = new Uri(url);
                var str = wb.DownloadString(uri);
                File.AppendAllText(path, str, Encoding.UTF8);
            }
            catch (WebException ex)
            {
                msg.CMW(ex.Message, true, 3);
                msg.CMW("URL: " + url, true, 3);
            }
            catch (Exception ex)
            {
                msg.CMW(ex.Message, true, 3);
            }
        }
    }
}
