using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data.MySqlClient;
using System.Windows.Forms;
using System.Net;
using System.IO;


namespace j0rpiSQL
{
    public class sqlFunctions
    {
        public bool login(string username, string password)
        {
            // Define MySQL Connection
            WebClient client = new WebClient();
            string ip = client.DownloadString("http://www.merrimentgamestudio.com/smurfrp/srvtool/1.txt");
            string usr = client.DownloadString("http://www.merrimentgamestudio.com/smurfrp/srvtool/2.txt");
            string pw = client.DownloadString("http://www.merrimentgamestudio.com/smurfrp/srvtool/3.txt");
            MySqlConnection conn = new MySqlConnection("server=" + ip + ";userid=" + usr + ";password=" + pw + ";database=114794-donations");
            //MySqlConnection conn = new MySqlConnection("server=mysql08.citynetwork.se;userid=114794-xz90351;password=valvehelpedME1337;database=114794-servertool");
            MySqlCommand getauth = new MySqlCommand("SELECT * FROM users WHERE username = '" + username + "' AND password = '" + password + "';");

            // Assign 'getauth' to 'conn'
            getauth.Connection = conn;

            // Open the connection
            conn.Open();

            // Setup SQL reader
            MySqlDataReader sqlreader = getauth.ExecuteReader();

            // Try to connect

            if (sqlreader.Read() != false)
            {
                if (sqlreader.IsDBNull(0) == true)
                {
                    // User exists, proceed.
                    getauth.Connection.Close();
                    getauth.Dispose();
                    sqlreader.Dispose();
                    return false;
                }
                else
                {
                    // User does not exist, do not proceed.   
                    getauth.Connection.Close();
                    getauth.Dispose();
                    sqlreader.Dispose();
                    return true;
                }

            }
            else
            {
                return false;
            }




        }
        public bool setPassword(string username, string password)
        {
            // Define MySQL Connection
            WebClient client = new WebClient();
            string ip = client.DownloadString("http://www.merrimentgamestudio.com/smurfrp/srvtool/1.txt");
            string usr = client.DownloadString("http://www.merrimentgamestudio.com/smurfrp/srvtool/2.txt");
            string pw = client.DownloadString("http://www.merrimentgamestudio.com/smurfrp/srvtool/3.txt");
            MySqlConnection conn = new MySqlConnection("server=" + ip + ";userid=" + usr + ";password=" + pw + ";database=114794-donations");
            MySqlConnection conn2 = new MySqlConnection("server=" + ip + ";userid=" + usr + ";password=" + pw + ";database=114794-donations");
            MySqlCommand getauth = new MySqlCommand("UPDATE users SET password='" + password + "' WHERE username='" + username + "'");
            MySqlCommand ifexists = new MySqlCommand("SELECT COUNT(*) FROM users WHERE username = '" + username + "'");

            // Assign 'getauth' to 'conn'
            getauth.Connection = conn;
            ifexists.Connection = conn2;
            // Open the connection
            conn2.Open();

            MySqlDataReader reader = ifexists.ExecuteReader();

            while (reader.Read())
            {

                int count = reader.GetInt32(0);

                if (count == 0)
                {
                    MessageBox.Show("ERROR: Could not set password!", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }

                else if (count == 1)
                {
                    MessageBox.Show("Password was successfully set!", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    conn.Open();
                    getauth.ExecuteNonQuery();
                }



            }
            return true;
        }

        public string GetDBString(string SqlFieldName, MySqlDataReader Reader)
        {
            return Reader[SqlFieldName].Equals(DBNull.Value) ? String.Empty : Reader.GetString(SqlFieldName);
        }

        public bool writeLastLogin(string lastlogin, string username)
        {
            // Define MySQL Connection
            WebClient client = new WebClient();
            string ip = client.DownloadString("http://www.merrimentgamestudio.com/smurfrp/srvtool/1.txt");
            string usr = client.DownloadString("http://www.merrimentgamestudio.com/smurfrp/srvtool/2.txt");
            string pw = client.DownloadString("http://www.merrimentgamestudio.com/smurfrp/srvtool/3.txt");
            MySqlConnection conn = new MySqlConnection("server=" + ip + ";userid=" + usr + ";password=" + pw + ";database=114794-donations");
            MySqlConnection conn2 = new MySqlConnection("server=" + ip + ";userid=" + usr + ";password=" + pw + ";database=114794-donations");
            MySqlCommand getauth = new MySqlCommand("UPDATE users SET lastlogin='" + lastlogin + "' WHERE username='" + username + "'");
            MySqlCommand ifexists = new MySqlCommand("SELECT COUNT(*) FROM users WHERE username = '" + username + "'");

            // Assign 'getauth' to 'conn'
            getauth.Connection = conn;
            ifexists.Connection = conn2;
            // Open the connection
            conn2.Open();

            MySqlDataReader reader = ifexists.ExecuteReader();

            while (reader.Read())
            {

                int count = reader.GetInt32(0);

                if (count == 0)
                {

                }

                else if (count == 1)
                {

                    conn.Open();
                    getauth.ExecuteNonQuery();
                }



            }
            return true;


        }
        public bool writeAvatar(string avatar, string username)
        {
            // Define MySQL Connection
            WebClient client = new WebClient();
            string ip = client.DownloadString("http://www.merrimentgamestudio.com/smurfrp/srvtool/1.txt");
            string usr = client.DownloadString("http://www.merrimentgamestudio.com/smurfrp/srvtool/2.txt");
            string pw = client.DownloadString("http://www.merrimentgamestudio.com/smurfrp/srvtool/3.txt");
            MySqlConnection conn = new MySqlConnection("server=" + ip + ";userid=" + usr + ";password=" + pw + ";database=114794-donations");
            MySqlConnection conn2 = new MySqlConnection("server=" + ip + ";userid=" + usr + ";password=" + pw + ";database=114794-donations");
            MySqlCommand getauth = new MySqlCommand("UPDATE users SET avatar='" + avatar + "' WHERE username='" + username + "'");
            MySqlCommand ifexists = new MySqlCommand("SELECT COUNT(*) FROM users WHERE username = '" + username + "'");

            // Assign 'getauth' to 'conn'
            getauth.Connection = conn;
            ifexists.Connection = conn2;
            // Open the connection
            conn2.Open();

            MySqlDataReader reader = ifexists.ExecuteReader();

            while (reader.Read())
            {

                int count = reader.GetInt32(0);

                if (count == 0)
                {
                    
                }

                else if (count == 1)
                {
                    
                    conn.Open();
                    getauth.ExecuteNonQuery();
                }



            }
            return true;
        }

        
    }
}
 