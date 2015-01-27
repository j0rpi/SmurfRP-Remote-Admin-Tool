using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using System.Net;
using j0rpiGlobal;
using System.Threading;
using System.IO;

namespace SmurfRPBanList
{
    public partial class UpdateDL : Form
    {
        public UpdateDL()
        {
            InitializeComponent();
        }

        private void UpdateDL_Load(object sender, EventArgs e)
        {
            WebClient http = new WebClient();
            string path;
            path = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase);
 
             
            
            using (WebClient myWebClient = new WebClient())
            {
                Thread.Sleep(2000);
                myWebClient.DownloadFileCompleted += new AsyncCompletedEventHandler(Completed);
                myWebClient.DownloadProgressChanged += new DownloadProgressChangedEventHandler(ProgressChanged);
                string remoteUri = "http://merrimentgamestudio.com/smurfrp/SRPSTOOL/updates/v0.1d_beta/";
                string fileName = "update.exe", myStringWebResource = null;
                myStringWebResource = remoteUri + fileName;
                myWebClient.DownloadFileAsync(new Uri(myStringWebResource), fileName);
                
            }
            
        }

        private void ProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            progressBar1.Value = e.ProgressPercentage;
        }

        private void Completed(object sender, AsyncCompletedEventArgs e)
        {
            MessageBox.Show("Update has been downloaded. SRPSTOOL will now exit to update.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
            Thread.Sleep(2000);
            Process.Start("update.exe");
            Application.Exit();
        }
    }
}
