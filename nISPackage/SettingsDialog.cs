using System;
using System.Diagnostics;
using System.IO;
using WixSharp;
using WixSharp.UI.Forms;
using System.Windows.Forms;

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
            if (txtInstanceName.Text.Equals("")
              || txtInstanceName.Text.Equals("")
              || txtInstanceName.Text.Equals("")
              || txtInstanceName.Text.Equals("")
              || txtInstanceName.Text.Equals("")
              )
            {
                MessageBox.Show("Please enter all details");
            }
            else {
                MsiRuntime.Session["INSTANCENAME"] = txtInstanceName.Text;
                MsiRuntime.Session["DBNAME"] = txtDBName.Text;
                MsiRuntime.Session["PASSWORD"] = txtPassword.Text;
                MsiRuntime.Session["USERNAME"] = txtUserName.Text;
                MsiRuntime.Session["APIPORT"] = txtAPIPortNo.Text;
                MsiRuntime.Session["APPPORT"] = txtAPPPortNo.Text;

                Shell.GoNext();
            }
           
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