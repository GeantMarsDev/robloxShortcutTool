using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RobloxShortcuts
{
    public partial class AuthForm : Form
    {

        private MainForm originalWindow;

        public AuthForm(MainForm main)
        {

            originalWindow = main;

            InitializeComponent();
            
        }

        private string GetAuthCookie()
        {
            return WebHelper.GetGlobalCookie("https://www.roblox.com/", ".ROBLOSECURITY");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string authCookie = textBox1.Text;
            if (authCookie == "")
            {
                MessageBox.Show("Please paste your cookie to confirm");
            }
            else
            {
                string exePath = System.Reflection.Assembly.GetExecutingAssembly().Location + "\\..";
                StreamWriter writer = new StreamWriter(exePath + "\\loginCookie.dat");
                writer.Write(authCookie);
                writer.Close();
                MessageBox.Show("Cookie saved");
                Close();
            };
        }

        private void AuthForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            originalWindow.button1.Enabled = true;
        }
    }
}
