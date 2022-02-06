using System;
using System.Linq;
using System.Windows.Forms;
using System.Net;
using System.IO;
using Newtonsoft.Json;
using IWshRuntimeLibrary;
using ImageMagick;
using System.Text.Json;
using System.Collections.Generic;

namespace RobloxShortcuts
{
    public partial class MainForm : Form
    {

        private string placeId = "606849621";
        private string currentName;
        private MagickImage currentImage;

        char[] allowed = new char[] {' ', '_', '.', ',', '!', '(', ')'};

        public MainForm()
        {
            InitializeComponent();
            FindGame();
        }

        private void FindGame()
        {
            //Grab place info using api
            WebRequest request0 = WebRequest.Create("https://api.roblox.com/marketplace/productinfo?assetId=" + placeId);
            WebResponse response0 = request0.GetResponse();
            string jsonString = new StreamReader(response0.GetResponseStream()).ReadToEnd();
            ProductInfo placeInfo = System.Text.Json.JsonSerializer.Deserialize<ProductInfo>(jsonString);

            //Set current name to place name
            currentName = placeInfo.Name;

            //Set current image to place icon
            try
            {
                string url = "https://thumbnails.roblox.com/v1/places/gameicons?placeIds={0}&size=512x512&format=Png&isCircular=false";
                WebRequest request1 = WebRequest.Create( String.Format(url, placeId) );
                WebResponse response1 = request1.GetResponse();
                Stream info = response1.GetResponseStream();
                StreamReader inforeader = new StreamReader(info);
                var contributorjsoninfo = inforeader.ReadToEnd();
                var contributorinfo = JsonConvert.DeserializeObject<Newtonsoft.Json.Linq.JObject>(contributorjsoninfo);
                Newtonsoft.Json.Linq.JToken data = contributorinfo.GetValue("data");
                Newtonsoft.Json.Linq.JToken data1 = data.First;
                Newtonsoft.Json.Linq.JToken imgprop = data1.Last;
                Newtonsoft.Json.Linq.JValue imgval = (Newtonsoft.Json.Linq.JValue)imgprop.Last;
                string imgurl = (string)imgval.Value;
                WebRequest imgrequest = WebRequest.Create(imgurl);
                WebResponse imgresponse = imgrequest.GetResponse();
                Stream img = imgresponse.GetResponseStream();

                currentImage = new MagickImage(img);
            }
            catch { }

            //Update display
            label2.Text = currentName;
            pictureBox1.Image = currentImage.ToBitmap();
        }

        private void SetControlEnabled(bool enabled)
        {
            // Enabled/disables all user input
            findGame.Enabled = enabled;
            makeShortcut.Enabled = enabled;
            textBox1.Enabled = enabled;
        }

        private void findGame_Click(object sender, EventArgs e)
        {
            SetControlEnabled(false);
            placeId = textBox1.Text;
            FindGame();
            SetControlEnabled(true);
        }

        private void makeShortcut_Click(object sender, EventArgs e)
        {
            SetControlEnabled(false);

            //Get all paths
            string exePath = System.Reflection.Assembly.GetExecutingAssembly().Location;
            string shortcutPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\" + RemoveInvalidChars(currentName) + ".lnk";
            string iconPath = Directory.GetCurrentDirectory() + "\\icons\\" + placeId + ".ico";

            //Generate ico file from game icon using ImageMagick
            MagickImage img = new MagickImage(currentImage);
            img.Resize(256, 256);
            img.Write(iconPath);

            //Build shortcut file
            WshShell shell = new WshShell();
            IWshShortcut shortcut = (IWshShortcut)shell.CreateShortcut(shortcutPath);
            shortcut.IconLocation = iconPath;
            shortcut.TargetPath = exePath;
            shortcut.Arguments = placeId; //This is key to functionality
            shortcut.Save();

            SetControlEnabled(true);
        }

        private string RemoveInvalidChars(string filename)
        {
            string safeString = "";
            foreach (char c in filename)
            {
                if (Char.IsLetterOrDigit(c) | allowed.Contains(c))
                {
                    safeString += c;
                }
            }
            if (safeString == "") {
                safeString = "My Shortcut";
            }
            return safeString;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            AuthForm form = new AuthForm(this);
            form.Show();
            button1.Enabled = false;
        }
    }
    
}
