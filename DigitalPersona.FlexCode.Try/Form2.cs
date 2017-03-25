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
    public partial class Form2 : Form
    {
        FlexCodeSDK.FinFPReg fPReg = new FlexCodeSDK.FinFPReg();
        string Template;
        public Form2()
        {
            InitializeComponent();

            fPReg.FPSamplesNeeded += FPReg_FPSamplesNeeded;
            fPReg.FPRegistrationTemplate += FPReg_FPRegistrationTemplate;
            fPReg.FPRegistrationStatus += FPReg_FPRegistrationStatus;
            fPReg.FPRegistrationImage += FPReg_FPRegistrationImage;

            this.Load += delegate 
            {
                fPReg.AddDeviceInfo("G900E031040", "5AA182FC8737A2C", "TPJA8DF7205C2F23EDD3EE2B");
                fPReg.PictureSamplePath = Application.StartupPath + "//temp.bmp";
                //fPReg.PictureSampleHeight=Microsoft.VisualBasic.Compatibility.VB6.PixelTwipsY(...)
                //fPReg.PictureSampleHeight = pictureBox1.Height;
                //fPReg.PictureSampleWidth = pictureBox1.Width;

            };
        }

        private void FPReg_FPRegistrationImage()
        {
            FileStream fs = new FileStream(Application.StartupPath + "//temp.bmp", FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            byte[] fBytes = new byte[fs.Length];
            fs.Read(fBytes, 0, (int)fs.Length);
            fs.Close();
            MemoryStream ms = new MemoryStream(fBytes);
            pictureBox1.Image = Image.FromStream(ms);
        }

        private void FPReg_FPRegistrationStatus(FlexCodeSDK.RegistrationStatus Status)
        {
            if (Status == FlexCodeSDK.RegistrationStatus.r_OK)
            {
                Clipboard.SetText(Template);
                MessageBox.Show("Enrollment Successful");
            }
        }

        private void FPReg_FPRegistrationTemplate(string FPTemplate)
        {
            Template = FPTemplate;
        }

        private void FPReg_FPSamplesNeeded(short Samples)
        {
            label2.Text = Samples.ToString();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            fPReg.FPRegistrationStart("63...");
        }
    }
}
