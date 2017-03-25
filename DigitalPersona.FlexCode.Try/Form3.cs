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

namespace DigitalPersona.FlexCode.Try
{
    public partial class Form3 : Form
    {
        FlexCodeSDK.FinFPVer fpVer = new FlexCodeSDK.FinFPVer();

        public Form3()
        {
            InitializeComponent();

            fpVer.AddDeviceInfo("G900E031040", "5AA182FC8737A2C", "TPJA8DF7205C2F23EDD3EE2B");

            fpVer.PictureSamplePath = Application.StartupPath + "//temp.bmp";
            //fpVer.PictureSampleHeight=Microsoft.VisualBasic.Compatibility.VB6.PixelTwipsY(...)
            //fpVer.PictureSampleHeight = pictureBox1.Height;
            //fpVer.PictureSampleWidth = pictureBox1.Width;

            string Template = Clipboard.GetText();
            fpVer.FPLoad("Your ID", FlexCodeSDK.FingerNumber.Fn_LeftPinkie, Template, "63...");
            fpVer.FPVerificationStatus += FpVer_FPVerificationStatus;
            fpVer.FPVerificationImage += FpVer_FPVerificationImage;
        }

        private void FpVer_FPVerificationImage()
        {
            FileStream fs = new FileStream(Application.StartupPath + "//temp.bmp", FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            byte[] fBytes = new byte[fs.Length];
            fs.Read(fBytes, 0, (int)fs.Length);
            fs.Close();
            MemoryStream ms = new MemoryStream(fBytes);
            pictureBox1.Image = Image.FromStream(ms);
        }

        private void FpVer_FPVerificationStatus(FlexCodeSDK.VerificationStatus Status)
        {
            if (Status== FlexCodeSDK.VerificationStatus.v_OK)
            {
                MessageBox.Show("got your match");
            }
            else if (Status== FlexCodeSDK.VerificationStatus.v_NotMatch)
            {
                MessageBox.Show("no match");
            }
            button1.Enabled = true;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            fpVer.FPVerificationStart();
            button1.Enabled = false;
        }
    }
}
