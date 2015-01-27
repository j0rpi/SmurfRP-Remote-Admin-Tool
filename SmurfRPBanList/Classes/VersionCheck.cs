using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;

namespace j0rpiApp
{
    class VersionCheck
    {
        public bool latestVersion(string version)
        {
            // Setup HTTP Client
            WebClient http = new WebClient();

            // Set 'version' string to the downloaded HTTP string
            version = http.DownloadString("http://merrimentgamestudio.com/smurfrp/SRPSTOOL/latest.txt");

            if (version == null)
            {
                return false;
            }
            else
            {
                return true;
            }

            
        }

        public string latestVersion2(string version)
        {
            // Setup HTTP Client
            WebClient http = new WebClient();

            // Set 'version' string to the downloaded HTTP string
            version = http.DownloadString("http://merrimentgamestudio.com/smurfrp/SRPSTOOL/latest.txt");

            return version;


        }

    }
}
