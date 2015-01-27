/*

     SmurfRP Server Tool
          by j0rpi

*/
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Net;
using MySql.Data.MySqlClient;
using SourceRconLib;
using j0rpiSQL;
using j0rpiApp;
using j0rpiGlobal;
using System.Text.RegularExpressions;
using System.IO;
using System.Diagnostics;

namespace SmurfRPBanList
{
    public partial class GUI : Form
    {

        public SourceRconLib.Rcon remote = new SourceRconLib.Rcon();
        
       
        public GUI()
        {
            InitializeComponent();
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            MessageBox.Show(null, "Please contact j0rpi to have your credentials reset. This cannot be done from within the program.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void linkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            MessageBox.Show(null, "Please contact j0rpi, or make a thread in the 'Admin HQ' forum in the 'SmurfRP Tool' section.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // Version String
            j0rpiGlobal.Global j0rpiStrings = new j0rpiGlobal.Global();
            label23.Text = j0rpiStrings.curversion();

            // Hide Ban Label
            label24.Visible = false;
            
            // The app is initializing, we'll set some shit.
            toolStripStatusLabel1.Text = "Not Logged In";
            toolStripStatusLabel2.Text = string.Empty;
            label4.Visible = false;

            // Move the panel which is shown when logged in.
            panel10.Location = new Point(260, 33);

            // Hide 'Logout' Panel
            panel10.Visible = false;

            // Set up handlers for rcon output
            remote.ServerOutput += new SourceRconLib.RconOutput(rcon_ServerOutput);

            // Setup IP:Port

            // UK Server
            var addresses = Dns.GetHostAddresses("uk.smurfrp.com");
            Debug.Assert(addresses.Length > 0);
            var endPoint = new IPEndPoint(addresses[0], 28715);

            // SWE Server
            //var addresses = Dns.GetHostAddresses("server.smurfrp.com");
            //Debug.Assert(addresses.Length > 0);
            //var endPoint = new IPEndPoint(addresses[0], 27015);
           

            // Now, let it connect
            remote.Connect(endPoint, "kanelbulle");

            // Prints output of status command
            remote.ServerCommand("status");

            TextBox.CheckForIllegalCrossThreadCalls = false;
            RichTextBox.CheckForIllegalCrossThreadCalls = false;

            // Gibberish
            textBox3.Text = ("SmurfRP Server Tool Beta\n");
            textBox3.AppendText("======================\n");
            textBox3.AppendText("\n");
            textBox3.AppendText("Useful commands:\n");
            textBox3.AppendText("\n");
            textBox3.AppendText("/clear        - Clears the console window\n");
            textBox3.AppendText("status        - Displays current players on the server\n");
            textBox3.AppendText("ulx help      - Displays help with all ULX commands (use wisely)\n");

            // Make 'Add Ban' Panel Invisible
            panel15.Visible = false;

            // Make 'Edit Ban' Panel Invisible
            panel17.Visible = false;

            // We won't be logged in at start. Let's hide acc settings panel.
            panel7.Visible = false;

            // And let's hide the ban buttons.
            button6.Enabled = false;
            button7.Enabled = false;
            button8.Enabled = false;

            // Ugly version check
            WebClient http = new WebClient();

            // Set 'version' string to the downloaded HTTP string
            string version = http.DownloadString("http://merrimentgamestudio.com/smurfrp/SRPSTOOL/latest.txt");

            // Now check for new updates
            if (version == "v0.1d beta")
            {
                DialogResult dialogResult = new DialogResult();
                dialogResult = MessageBox.Show("A new update is available!" + Environment.NewLine + Environment.NewLine + "Current Version: 0.1c beta" + Environment.NewLine + "New Version: v0.1d beta"  + Environment.NewLine + Environment.NewLine + "Would you like to update?", "Update Available", MessageBoxButtons.YesNo, MessageBoxIcon.Information);

                if (dialogResult == DialogResult.Yes)
                {
                    this.Hide();
                    UpdateDL updatedialog = new UpdateDL();
                    updatedialog.Show();
                    
                }
                else if (dialogResult == DialogResult.No)
                {
                    MessageBox.Show("You will be asked next time you start SRPSTOOL again to update. It's strongly suggested that you do so.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    string path;
                    path = System.IO.Path.GetDirectoryName(
                       System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase);
                    MessageBox.Show(path);
                }
            }
            else
            {
                // Do Nothing.. NOTHING.
            }
            

            // Check how many bans
            label24.Text = ("Total Bans: ") + listView1.Items.Count.ToString();

            // Re-Position 'Edit Ban' Panel
            panel17.Location = (new Point(306, 86));

            // Load last user, if user choosed to do so.
            if (File.Exists("lastuser.txt"))
            {
                using (StreamReader sr = File.OpenText("lastuser.txt"))
                {
                    string s = "";
                    while ((s = sr.ReadLine()) != null)
                    {
                        textBox1.Text = s;
                    }
                }
            }
            else
            {
                // Do Nothing.
            }

        }

        void rcon_ServerOutput(SourceRconLib.MessageCode code, string data)
        {
            switch (code)
            {
                case SourceRconLib.MessageCode.ConsoleOutput:
                    if (!string.IsNullOrEmpty(data))
                    {
                        if (data.Contains("\nL"))
                        {
                            textBox3.AppendText(Regex.Split(data, "\nL")[0]);
                        }
                        else
                        {
                            this.textBox3.AppendText(data);
                            this.textBox3.ScrollToCaret();
                            
                        }
                    }
                    break;
                case SourceRconLib.MessageCode.TooMuchData:
                    textBox3.AppendText("[ERROR] Attempted to send to much data!");
                    break;
                default:
                    // Eha yah sure?
                    break;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            sqlFunctions sql = new sqlFunctions();

                if (sql.login(textBox1.Text, textBox2.Text) == true)
                {
                    toolStripStatusLabel1.ForeColor = Color.DarkGreen;
                    toolStripStatusLabel1.Text = "Logged In As:";
                    toolStripStatusLabel2.Text = textBox1.Text;
                    sqlFunctions sql2 = new sqlFunctions();
                    WebClient client = new WebClient();
                    string ip = client.DownloadString("http://www.merrimentgamestudio.com/smurfrp/srvtool/1.txt");
                    string usr = client.DownloadString("http://www.merrimentgamestudio.com/smurfrp/srvtool/2.txt");
                    string pw = client.DownloadString("http://www.merrimentgamestudio.com/smurfrp/srvtool/3.txt");
                    string ip2 = client.DownloadString("http://www.merrimentgamestudio.com/smurfrp/srvtool/4.txt");
                    string usr2 = client.DownloadString("http://www.merrimentgamestudio.com/smurfrp/srvtool/5.txt");
                    string pw2 = client.DownloadString("http://www.merrimentgamestudio.com/smurfrp/srvtool/6.txt");
                    MySqlConnection conn2 = new MySqlConnection("server=" + ip + ";userid=" + usr + ";password=" + pw + ";database=114794-donations");
                    MySqlConnection conn4 = new MySqlConnection("server=" + ip + ";userid=" + usr + ";password=" + pw + ";database=114794-donations");
                    MySqlConnection conn3 = new MySqlConnection("server=" + ip2 + ";userid=" + usr2 + ";password=" + pw2 + ";database=114794-drpbans");
                    MySqlCommand getauth2 = new MySqlCommand("SELECT * FROM users WHERE username = '" + textBox1.Text + "';");
                    MySqlCommand getauth4 = new MySqlCommand("SELECT * FROM users");
                    MySqlCommand getauth3 = new MySqlCommand("SELECT * FROM u_globalbans");
                    getauth2.Parameters.AddWithValue("username", textBox1.Text);
                    getauth2.Connection = conn2;
                    getauth3.Connection = conn3;
                    getauth4.Connection = conn4;
                    conn2.Open();
                    conn3.Open();
                    conn4.Open();
                    MySqlDataReader Reader2 = getauth2.ExecuteReader();
                    MySqlDataReader Reader3 = getauth3.ExecuteReader();
                    MySqlDataReader Reader4 = getauth4.ExecuteReader();
                    // Set 'last login'
                    sql.writeLastLogin(DateTime.Now.ToString(), textBox1.Text);

                    // Lastly, set form title
                    this.Text = ("SmurfRP Server Tool :: " + textBox1.Text);

                    // Hide login panel
                    panel1.Visible = false;

                    // Show logout panel
                    panel10.Visible = true;

                    while (Reader2.Read())
                    {
                        
                        // Set some shit.
                        pictureBox2.ImageLocation = (sql.GetDBString("avatar", Reader2));
                        label7.Text = textBox1.Text;
                        label9.Text = (sql.GetDBString("registerdate", Reader2));
                        label10.Text = (sql.GetDBString("lastlogin", Reader2));
                        label15.Text = "You Last Logged In: " + (sql.GetDBString("lastlogin", Reader2));
                        textBox6.Text = (sql.GetDBString("avatar", Reader2));
                        label13.Text = "Logged in as " + textBox1.Text;
                        panel7.Visible = true;
                        
                        button6.Enabled = true;
                        button7.Enabled = true;
                        button8.Enabled = true;

                        // Show Ban Label
                        label24.Visible = true;

                        // Remember User?
                        if (checkBox1.Checked == true)
                        {
                            using (StreamWriter sw = File.CreateText("lastuser.txt"))
                            {
                                sw.WriteLine(textBox1.Text);
                            }
                        }
                        else
                        {
                            if (File.Exists("lastuser.txt"))
                            {
                                File.Delete("lastuser.txt");
                            }
                        }
                    }

                    while (Reader4.Read())
                    {

                            // Populate the new tab with even more shit!
                            ListViewItem userlist;
                            userlist = new ListViewItem(Reader4.GetString("username"));
                            userlist.SubItems.Add(Reader4.GetString("registerdate"));
                            userlist.SubItems.Add(sql.GetDBString("lastlogin", Reader4));
                            userlist.SubItems.Add(Reader4.GetString("isadmin"));
                            userlist.SubItems.Add(Reader4.GetString("canuseremote"));
                            userlist.SubItems.Add(Reader4.GetString("isbanned"));
                            listView2.Items.Add(userlist);
                    }

                   
                    while (Reader3.Read())
                    {
                        ListViewItem item;
                        item = new ListViewItem(Reader3.GetString("_SteamID"));
                        item.UseItemStyleForSubItems = false;
                        if (Reader3.IsDBNull(1) == true)
                        {
                            item.SubItems.Add("Unknown", Color.Maroon, Color.White, new Font(label1.Font.Name, 8.0F, FontStyle.Bold));
                        }
                        else
                        {
                            item.SubItems.Add(Convert.ToString(Reader3.GetValue(1)));
                        }
                        if (Reader3.GetString("_Reason") == "")
                        {
                            item.SubItems.Add("No Reason Given", Color.Gray, Color.White, new Font(label1.Font.Name, 8.0F, FontStyle.Italic));
                        }
                        else
                        {
                            item.SubItems.Add(Reader3.GetString("_Reason").Replace(@"\", ""));
                        }

                        if (Reader3.GetString("_Length") == "0")
                        {
                            item.SubItems.Add("Permanently", Color.Maroon, Color.White, new Font(label1.Font.Name, 8.0F, FontStyle.Bold));
                        }
                        else
                        {

                            TimeSpan t2 = TimeSpan.FromSeconds(Convert.ToDouble(Reader3.GetString("_Length")));


                            item.SubItems.Add(string.Format("{0:D2}h {1:D2}m {2:D2}s",
                                                            t2.Hours,
                                                            t2.Minutes,
                                                            t2.Seconds));

                        }

                       
                            TimeSpan t = TimeSpan.FromSeconds(Convert.ToDouble(Reader3.GetString("_Time")));


                            item.SubItems.Add(string.Format("{0:D2}h {1:D2}m {2:D2}s",
                                                            t.Hours,
                                                            t.Minutes,
                                                            t.Seconds));




                            item.SubItems.Add(Reader3.GetString("_ASteamName"));
                            item.SubItems.Add("lol");
                            listView1.Items.Add(item);
                        
                        
                    }
                    Reader2.Close();
                    conn2.Close();
                    Reader3.Close();
                    conn3.Close();
                    conn4.Close();
                    Reader4.Close();
                    label24.Text = ("Total Bans: ") + listView1.Items.Count.ToString();
                    

                }
                else
                {
                    label4.Text = "Authentication Failed. Check Username/Password.";
                    label4.Visible = true;
                }



        }

        private void button2_Click(object sender, EventArgs e)
        {
                    
            
            sqlFunctions sql = new sqlFunctions();

            WebClient client = new WebClient();
            string ip = client.DownloadString("http://www.merrimentgamestudio.com/smurfrp/srvtool/1.txt");
            string usr = client.DownloadString("http://www.merrimentgamestudio.com/smurfrp/srvtool/2.txt");
            string pw = client.DownloadString("http://www.merrimentgamestudio.com/smurfrp/srvtool/3.txt");
            MySqlConnection conn3 = new MySqlConnection("server=" + ip + ";userid=" + usr + ";password=" + pw + ";database=114794-donations");
            MySqlCommand getauth2 = new MySqlCommand("SELECT * FROM users WHERE username = '" + textBox1.Text + "';");
                    
                    getauth2.Parameters.AddWithValue("username", textBox1.Text);
                    getauth2.Connection = conn3;
                    conn3.Open();
                    MySqlDataReader Reader2 = getauth2.ExecuteReader();
                    Reader2.Read();
                    
            string canusercon = sql.GetDBString("canuseremote", Reader2);
            string isadmin = sql.GetDBString("isadmin", Reader2);
            // Check user rights - If user is just a standard user, we'll deny his rights.
            if (this.Text.Contains("Not Logged In")) 
            {
                MessageBox.Show("ERROR: You are not authenticated. Please login first.", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            // Add the ability to clear the whole textbox
            else if (textBox4.Text == "/clear")
            {
                textBox3.Clear();
                textBox4.Text = "";
            }
            // Disallow usage of rcon if user has the "canuseremote" flag set to false.
            else if (canusercon == "false")
            {
                MessageBox.Show("ERROR: You do not have sufficient rights to use the Remote Console.", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

            }
            else if (isadmin == "false")
            {
                MessageBox.Show("ERROR: Your user-group does not have sufficient rights to use the Remote Console.", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            else
            {
                // Send Command
                remote.ServerCommand(textBox4.Text);
            }


        }

        private void panel2_Paint(object sender, PaintEventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {
            sqlFunctions sql = new sqlFunctions();

            if (sql.setPassword(toolStripStatusLabel2.Text, textBox5.Text) == true)
            {

            }
            else
            {

            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            sqlFunctions sql = new sqlFunctions();
            if (sql.writeAvatar(textBox6.Text, textBox1.Text) == true)
            {
                pictureBox2.ImageLocation = textBox6.Text;
                MessageBox.Show("Avatar was successfully updated!", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("Failed to update avatar!", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            // Hide this panel.
            panel10.Visible = false;

            // Show login panel
            panel1.Visible = true;

            // Set everything back to normal
            toolStripStatusLabel1.Text = "Not Logged In";
            toolStripStatusLabel2.Text = "";
            this.Text = "SmurfRP Server Tool :: Not Logged In";

            // Account data shit
            pictureBox2.ImageLocation = "";
            textBox6.Text = "";
            label9.Text = "";
            label10.Text = "";
            label7.Text = "";

            // Hide acc settings panel
            panel7.Visible = false;

            // Clear SRPST User ListView/Ban ListView
            listView1.Items.Clear();
            listView2.Items.Clear();

            // Disable All Buttons
            button6.Enabled = false;
            button7.Enabled = false;
            button8.Enabled = false;

            //Hide 'Total Bans' Label
            label24.Visible = false;

            // Hide 'error login' label
            label4.Visible = false;

            // Set ToolStripLabel To Black Again
            toolStripStatusLabel1.ForeColor = Color.Black;
        }


        private void listView1_ColumnClick_1(object sender, ColumnClickEventArgs e)
        {
            listView1.Items.Clear();
            WebClient client = new WebClient();
            string ip2 = client.DownloadString("http://www.merrimentgamestudio.com/smurfrp/srvtool/4.txt");
            string usr2 = client.DownloadString("http://www.merrimentgamestudio.com/smurfrp/srvtool/5.txt");
            string pw2 = client.DownloadString("http://www.merrimentgamestudio.com/smurfrp/srvtool/6.txt");
            MySqlConnection conn3 = new MySqlConnection("server=" + ip2 + ";userid=" + usr2 + ";password=" + pw2 + ";database=114794-drpbans");
            MySqlCommand getauth3 = new MySqlCommand("SELECT * FROM u_globalbans ORDER BY _ASteamName");
            getauth3.Connection = conn3;
            conn3.Open();
            MySqlDataReader Reader3 = getauth3.ExecuteReader();

            while (Reader3.Read())
            {
                ListViewItem item;
                item = new ListViewItem(Reader3.GetString("_SteamID"));
                if (Reader3.IsDBNull(1) == true)
                {
                    item.SubItems.Add("Unknown");
                }
                else
                {
                    item.SubItems.Add(Convert.ToString(Reader3.GetValue(1)));
                }
                item.SubItems.Add(Reader3.GetString("_Reason"));

                DateTime dt1 = DateTime.ParseExact(Reader3.GetString("_Time"), "H:m:s", null);
                
         
                item.SubItems.Add(string.Format("{0:D2}d:{1:D2}h:{2:D2}m:{3:D2}s",
                                                dt1.Day,
                                                dt1.Hour,
                                                dt1.Minute,
                                                dt1.Second));
                item.SubItems.Add(Reader3.GetString("_ASteamName"));
                listView1.Items.Add(item);

            }
            Reader3.Close();
            conn3.Close();
        }

        private void listView1_MouseEnter_1(object sender, EventArgs e)
        {
            listView1.Focus();
        }

        private void SendCommand(string command)
        {
            textBox3.AppendText(string.Format("---->> {0}", command));
            remote.ServerCommand(command);
        }

        private void button6_Click(object sender, EventArgs e)
        {

            sqlFunctions sql = new sqlFunctions();

            WebClient client = new WebClient();
            string ip = client.DownloadString("http://www.merrimentgamestudio.com/smurfrp/srvtool/1.txt");
            string usr = client.DownloadString("http://www.merrimentgamestudio.com/smurfrp/srvtool/2.txt");
            string pw = client.DownloadString("http://www.merrimentgamestudio.com/smurfrp/srvtool/3.txt");
            MySqlConnection conn3 = new MySqlConnection("server=" + ip + ";userid=" + usr + ";password=" + pw + ";database=114794-donations");
            MySqlCommand getauth2 = new MySqlCommand("SELECT * FROM users WHERE username = '" + textBox1.Text + "';");

            getauth2.Parameters.AddWithValue("username", textBox1.Text);
            getauth2.Connection = conn3;
            conn3.Open();
            MySqlDataReader Reader2 = getauth2.ExecuteReader();
            Reader2.Read();
            string canusercon = sql.GetDBString("canuseremote", Reader2);
            string isadmin = sql.GetDBString("isadmin", Reader2);


            if (isadmin == "true")
            {

                DialogResult dialogResult = MessageBox.Show("Do you really want to remove this ban with SteamID: " + listView1.FocusedItem.SubItems[0].Text + "?", "Some Title", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
                if (dialogResult == DialogResult.Yes)
                {
                    remote.ServerCommand("ulx unban " + listView1.FocusedItem.SubItems[0].Text);
                    MessageBox.Show("Player with SteamID: " + listView1.FocusedItem.SubItems[0].Text + " was successfully un-banned.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    remote.ServerCommand("say [SmurfRP Server Tool] " + listView1.FocusedItem.SubItems[0].Text + " was un-banned by " + textBox1.Text);
                }
                else if (dialogResult == DialogResult.No)
                {
                    // We ain't doing shit.
                }
            }
            else
            {
                MessageBox.Show("ERROR: Your user-group does not have sufficient rights to use the Remote Console.", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            
            
        }

        private void button8_Click(object sender, EventArgs e)
        {
            // Hide Banlist, Show 'Add Ban' Panel
            listView1.Visible = false;
            panel15.Visible = true;
        }

        private void button9_Click(object sender, EventArgs e)
        {
            sqlFunctions sql = new sqlFunctions();

            WebClient client = new WebClient();
            string ip = client.DownloadString("http://www.merrimentgamestudio.com/smurfrp/srvtool/1.txt");
            string usr = client.DownloadString("http://www.merrimentgamestudio.com/smurfrp/srvtool/2.txt");
            string pw = client.DownloadString("http://www.merrimentgamestudio.com/smurfrp/srvtool/3.txt");
            MySqlConnection conn3 = new MySqlConnection("server=" + ip + ";userid=" + usr + ";password=" + pw + ";database=114794-donations");
            MySqlCommand getauth2 = new MySqlCommand("SELECT * FROM users WHERE username = '" + textBox1.Text + "';");

            getauth2.Parameters.AddWithValue("username", textBox1.Text);
            getauth2.Connection = conn3;
            conn3.Open();
            MySqlDataReader Reader2 = getauth2.ExecuteReader();
            Reader2.Read();
            string canusercon = sql.GetDBString("canuseremote", Reader2);
            string isadmin = sql.GetDBString("isadmin", Reader2);

            if (isadmin == "true")
            {
                remote.ServerCommand("ulx banid " + textBox7.Text + " " + textBox9.Text + " " + textBox8.Text);
           
                MessageBox.Show("Player with SteamID: " + textBox7.Text + " was successfully added to the ban list.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                panel15.Visible = false;
                listView1.Visible = true;
                remote.ServerCommand("say [SmurfRP Server Tool] " + textBox7.Text + " was added to the ban list by " + textBox1.Text);
            }
            else
            {
                MessageBox.Show("ERROR: Your user-group does not have sufficient rights to use the Remote Console.", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                panel15.Visible = false;
                listView1.Visible = true;
            }
        }

        private void button7_Click(object sender, EventArgs e)
        {

            listView1.Visible = false;
            panel17.Visible = true;
            textBox10.Text = listView1.FocusedItem.SubItems[4].Text;
            textBox11.Text = listView1.FocusedItem.SubItems[2].Text;
            textBox12.Text = listView1.FocusedItem.SubItems[0].Text;
        }

        private void button10_Click(object sender, EventArgs e)
        {
            label24.Text = listView1.Items.Count.ToString();
        }

        private void tabControl1_TabIndexChanged(object sender, EventArgs e)
        {
            label24.Text = ("Total Bans: ") + listView1.Items.Count.ToString();
        }

        private void panel10_Paint(object sender, PaintEventArgs e)
        {

        }

        private void panel17_Paint(object sender, PaintEventArgs e)
        {

        }

        private void button10_Click_1(object sender, EventArgs e)
        {
            sqlFunctions sql = new sqlFunctions();

            WebClient client = new WebClient();
            string ip = client.DownloadString("http://www.merrimentgamestudio.com/smurfrp/srvtool/1.txt");
            string usr = client.DownloadString("http://www.merrimentgamestudio.com/smurfrp/srvtool/2.txt");
            string pw = client.DownloadString("http://www.merrimentgamestudio.com/smurfrp/srvtool/3.txt");
            MySqlConnection conn3 = new MySqlConnection("server=" + ip + ";userid=" + usr + ";password=" + pw + ";database=114794-donations");
            MySqlCommand getauth2 = new MySqlCommand("SELECT * FROM users WHERE username = '" + textBox1.Text + "';");

            getauth2.Parameters.AddWithValue("username", textBox1.Text);
            getauth2.Connection = conn3;
            conn3.Open();
            MySqlDataReader Reader2 = getauth2.ExecuteReader();
            Reader2.Read();
            string canusercon = sql.GetDBString("canuseremote", Reader2);
            string isadmin = sql.GetDBString("isadmin", Reader2);

            if (textBox10.Text.Contains("h"))
            {
                MessageBox.Show("Ban length needs to be set in numbers!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            else if (textBox10.Text.Contains("m"))
            {
                MessageBox.Show("Ban length needs to be set in numbers!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            else if (textBox10.Text.Contains("s"))
            {
                MessageBox.Show("Ban length needs to be set in numbers!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }

            if (isadmin == "true")
            {
                remote.ServerCommand("ulx banid " + textBox7.Text + " " + textBox9.Text + " " + textBox8.Text);

                MessageBox.Show("Player with SteamID: " + textBox7.Text + " has his/her ban edited.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                // Hide Banlist, Show 'Add Ban' Panel
                listView1.Visible = true;
                panel17.Visible = false;
                remote.ServerCommand("say [SmurfRP Server Tool] Ban with SteamID: " + textBox7.Text + " was edited by " + textBox1.Text);
            }
            else
            {
                MessageBox.Show("ERROR: Your user-group does not have sufficient rights to use the Remote Console.", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                panel17.Visible = false;
                listView1.Visible = true;
            }
        }
 
    }
}
