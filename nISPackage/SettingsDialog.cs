using System;
using System.Diagnostics;
using System.IO;
using WixSharp;
using WixSharp.UI.Forms;

namespace WixSharpSetup
{
    public partial class SettingsDialog : ManagedForm, IManagedDialog
    {
        public SettingsDialog()
        {
            //NOTE: If this assembly is compiled for v4.0.30319 runtime, it may not be compatible with the MSI hosted CLR.
            //The incompatibility is particularly possible for the Embedded UI scenarios. 
            //The safest way to avoid the problem is to compile the assembly for v3.5 Target Framework.WixSharp Setup
            InitializeComponent();
        }

        void dialog_Load(object sender, EventArgs e)
        {
            banner.Image = Runtime.Session.GetResourceBitmap("WixUI_Bmp_Banner");

        }

        void back_Click(object sender, EventArgs e)
        {
            Shell.GoPrev();
        }

        void next_Click(object sender, EventArgs e)
        {
            Runtime.Session["INSTANCENAME"] = txtInstanceName.Text;
            Runtime.Session["DATABASE"] = txtDBName.Text;
            Runtime.Session["PASSWORD"] = txtPassword.Text;
            Runtime.Session["USERNAME"] = txtUserName.Text;
            Runtime.Session["APIPORT"] = txtAPIPort.Text;
            Runtime.Session["APPPORT"] = txtAPPPort.Text;
            // ..\API\bin\app.publish\Web.config
            //..\APP\\wwwroot\env-specific.json
            //script.sql

            //#region Update sql connection string;
            //string dbConnectionString = "Data Source={{InstanceName}};Initial Catalog={{DataBaseName}};User ID={{UserName}};Password={{Password}}";

            //dbConnectionString = dbConnectionString.Replace("{{InstanceName}}", txtInstanceName.Text);
            //dbConnectionString = dbConnectionString.Replace("{{DataBaseName}}", txtDBName.Text);
            //dbConnectionString = dbConnectionString.Replace("{{UserName}}", txtUserName.Text);
            //dbConnectionString = dbConnectionString.Replace("{{Password}}", txtPassword.Text);

            //StreamReader reader = new StreamReader(@"script.sql");
            //string readedData = reader.ReadToEnd();
            //reader.Close();
            ////modify what you want
            //readedData.Replace("{{DataBaseConnectionString}}", dbConnectionString);

            ////Write new file or append on existing file
            //StreamWriter writer = new StreamWriter(@"script.sql", false);
            //writer.Write(readedData);
            //writer.Close();
            //#endregion

            //#region Update API Webconfig File
            //reader = new StreamReader(@"..\API\bin\app.publish\Web.config");
            //readedData = reader.ReadToEnd();
            //reader.Close();
            ////modify what you want
            //readedData.Replace("{{DataBaseConnectionString}}", dbConnectionString);
            //readedData.Replace("{{APPPORTNO}}", txtAPPPort.Text);
            ////Write new file or append on existing file
            //writer = new StreamWriter(@"..\API\bin\app.publish\Web.config", false);
            //writer.Write(readedData);
            //writer.Close();

            //#endregion

            //#region Update APP Env Setting JSON File

            //reader = new StreamReader(@"..\APP\\wwwroot\env-specific.json");
            //readedData = reader.ReadToEnd();
            //reader.Close();

            ////modify what you want
            //readedData.Replace("{{APIPORTNO}}", txtAPIPort.Text);

            ////Write new file or append on existing file
            //writer = new StreamWriter(@"..\APP\\wwwroot\env-specific.json", false);
            //writer.Write(readedData);
            //writer.Close();

            //#endregion
            Shell.GoNext();
        }

        void cancel_Click(object sender, EventArgs e)
        {
            Shell.Cancel();
        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }
    }
}