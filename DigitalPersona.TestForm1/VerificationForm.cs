using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Collections;

namespace DigitalPersona.TestForm1
{
    public partial class VerificationForm : UserControl
    {
        DPFP.Verification.Verification matcher;
        DPFP.Verification.Verification.Result matchResult;
        string serial = "1284476B-2B20-CE4C-947B-0F1CF99144F6";

        DPFP.Gui.Verification.VerificationControl verificationControl;

        PictureBox picBox;
        Label lblName, lblAge, lblGender, lblMaritalStatus, lblState, lblLGA;

        public VerificationForm(AppWindow owner)
        {
            InitializeComponent();

            this.Dock = DockStyle.Fill;
            this.BackColor = owner.LightColor;

            verificationControl = new DPFP.Gui.Verification.VerificationControl()
            {
                Size = new Size(100, 100)
            };
            verificationControl.ReaderSerialNumber = serial;
            verificationControl.OnComplete += VerificationControl_OnComplete;
            this.Controls.Add(verificationControl);

            picBox = new PictureBox()
            {
                Size = new Size(300, 300)
            };
            this.Controls.Add(picBox);

            lblName = new Label()
            {
                AutoSize = false,
                Font = new Font(this.Font.FontFamily, 16.0f, FontStyle.Regular),
                TextAlign = ContentAlignment.MiddleLeft,
                Width = 500
            };
            this.Controls.Add(lblName);

            lblAge = new Label()
            {
                AutoSize = lblName.AutoSize,
                Font = lblName.Font,
                TextAlign = lblName.TextAlign,
                Width = lblName.Width
            };
            this.Controls.Add(lblAge);

            lblGender = new Label()
            {
                AutoSize = lblName.AutoSize,
                Font = lblName.Font,
                TextAlign = lblName.TextAlign,
                Width = lblName.Width
            };
            this.Controls.Add(lblGender);

            lblMaritalStatus = new Label()
            {
                AutoSize = lblName.AutoSize,
                Font = lblName.Font,
                TextAlign = lblName.TextAlign,
                Width = lblName.Width
            };
            this.Controls.Add(lblMaritalStatus);

            lblState = new Label()
            {
                AutoSize = lblName.AutoSize,
                Font = lblName.Font,
                TextAlign = lblName.TextAlign,
                Width = lblName.Width
            };
            this.Controls.Add(lblState);

            lblLGA = new Label()
            {
                AutoSize = lblName.AutoSize,
                Font = lblName.Font,
                TextAlign = lblName.TextAlign,
                Width = lblName.Width
            };
            this.Controls.Add(lblLGA);

            this.SizeChanged += delegate
            {
                verificationControl.Location = new Point((this.Width - verificationControl.Width) / 2, 20);
                picBox.Location = new Point(verificationControl.Left - picBox.Width, verificationControl.Bottom + 20);
                lblName.Location = new Point(picBox.Right + 50, picBox.Top);
                lblAge.Location = new Point(lblName.Left, lblName.Bottom + 20);
                lblGender.Location = new Point(lblName.Left, lblAge.Bottom + 20);
                lblMaritalStatus.Location = new Point(lblName.Left, lblGender.Bottom + 20);
                lblState.Location = new Point(lblName.Left, lblMaritalStatus.Bottom + 20);
                lblLGA.Location = new Point(lblName.Left, lblState.Bottom + 20);
            };
        }

        private void VerificationControl_OnComplete(object Control, DPFP.FeatureSet FeatureSet, 
            ref DPFP.Gui.EventHandlerStatus EventHandlerStatus)
        {
            picBox.Image = null;
            lblName.ResetText();
            lblAge.ResetText();
            lblGender.ResetText();
            lblMaritalStatus.ResetText();
            lblState.ResetText();
            lblLGA.ResetText();

            IdpDb db = new IdpDb();
            bool breakOuter = false;
            foreach (Idp person in db.GetPersons())
            {
                int finger = 0;
                foreach(var template in person.FingerTemplates)
                {
                    finger++;
                    if (template != null)
                    {
                        matcher = new DPFP.Verification.Verification();
                        matchResult = new DPFP.Verification.Verification.Result();
                        matcher.Verify(FeatureSet, template, ref matchResult);
                        if (matchResult.Verified)
                        {
                            //MessageBox.Show(person.FirstName + ", finger: " + finger);

                            //pic
                            if (person.Photo != null)
                            {
                                picBox.Image = new Bitmap(person.Photo, picBox.Size);
                            }

                            string nm = (!string.IsNullOrEmpty(person.FirstName) ? person.FirstName + " " : "") +
                                (!string.IsNullOrEmpty(person.LastName) ? person.LastName + " " : "") +
                                (!string.IsNullOrEmpty(person.OtherNames) ? person.OtherNames + " " : "");
                            lblName.Text = nm;
                            if (person.DoB != null)
                            {
                                int age = DateTime.Now.Year - person.DoB.Year;
                                lblAge.Text = person.DoB.Day + "/" + person.DoB.Month + "/" + person.DoB.Year +
                                    " (" + age + " year(s) old)";
                            }
                            else
                            {
                                int age = DateTime.Now.Year - person.YoB;
                                lblAge.Text = "About " + age + " year(s) old";
                            }
                            lblGender.Text = person.Gender;
                            lblMaritalStatus.Text = person.MaritalStatus;
                            lblState.Text = person.State;
                            lblLGA.Text = person.LGA;
                            breakOuter = true;
                            break;
                        }
                    }
                }
                if (breakOuter) break;
            }
            if (!breakOuter)
            {
                MessageBox.Show("Could not identify user!");
            }
        }
    }
}
