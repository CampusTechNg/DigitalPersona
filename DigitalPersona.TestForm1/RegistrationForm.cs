using AForge.Video.DirectShow;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DigitalPersona.TestForm1
{
    public partial class RegistrationForm : UserControl
    {
        string serial = "1284476B-2B20-CE4C-947B-0F1CF99144F6";
        DPFP.Template[] capturedTemplates = new DPFP.Template[10];
        Dictionary<string, string[]> statesAndLGAs;

        HintedTextBox txtFirstname, txtLastName, txtOtherNames;
        RadioButton btnDob, btnYoB;
        DateTimePicker datePicker;
        NumericUpDown txtAge;
        ComboBox genderList, maritalStatusList, statesList, lgaList;
        CameraControl camControl;
        DPFP.Gui.Enrollment.EnrollmentControl enrollmentControl;
        Button btnSave;

        public RegistrationForm(AppWindow owner)
        {
            InitializeComponent();

            PopulateStatesAndLGAs();

            this.Dock = DockStyle.Fill;
            this.BackColor = owner.LightColor;

            txtFirstname = new HintedTextBox("first name")
            {
                BorderStyle = System.Windows.Forms.BorderStyle.None,
                Font = new System.Drawing.Font(this.Font.FontFamily, 16.0F),
                Width = 300
            };
            txtFirstname.Location = new Point(100, 20);
            UnderlineFor lblLine1 = new UnderlineFor(txtFirstname, owner.DarkColor, SystemColors.GrayText)
            {
                BackColor = owner.DarkColor,
            };
            lblLine1.Location = new Point(txtFirstname.Location.X, txtFirstname.Bottom);
            this.Controls.Add(txtFirstname);
            this.Controls.Add(lblLine1);

            txtLastName = new HintedTextBox("last name")
            {
                BorderStyle = txtFirstname.BorderStyle,
                Font = txtFirstname.Font,
                Width = txtFirstname.Width
            };
            txtLastName.Location = new Point(txtFirstname.Left, txtFirstname.Bottom + 20);
            UnderlineFor lblLine2 = new UnderlineFor(txtLastName, owner.DarkColor, SystemColors.GrayText)
            {
                BackColor = SystemColors.GrayText,
            };
            lblLine2.Location = new Point(txtLastName.Location.X, txtLastName.Bottom);
            this.Controls.Add(txtLastName);
            this.Controls.Add(lblLine2);

            txtOtherNames = new HintedTextBox("other names")
            {
                BorderStyle = txtFirstname.BorderStyle,
                Font = txtFirstname.Font,
                Width = txtFirstname.Width
            };
            txtOtherNames.Location = new Point(txtLastName.Left, txtLastName.Bottom + 20);
            UnderlineFor lblLine3 = new UnderlineFor(txtOtherNames, owner.DarkColor, SystemColors.GrayText)
            {
                BackColor = SystemColors.GrayText,
            };
            lblLine3.Location = new Point(txtOtherNames.Location.X, txtOtherNames.Bottom);
            this.Controls.Add(txtOtherNames);
            this.Controls.Add(lblLine3);

            btnDob = new RadioButton()
            {
                Checked = true,
                Font = txtFirstname.Font,
                Height = 30,
                Location = new Point(txtOtherNames.Location.X, txtOtherNames.Bottom + 20),
                Text = "I know my date of birth",
                Width = txtFirstname.Width,
            };
            btnDob.CheckedChanged += delegate 
            {
                datePicker.Enabled = btnDob.Checked;
            };
            this.Controls.Add(btnDob);

            datePicker = new DateTimePicker()
            {
                Enabled = btnDob.Checked,
                Font = txtFirstname.Font,
                Location = new Point(btnDob.Left + 20, btnDob.Bottom + 5),
                Width = btnDob.Width - 20
            };
            this.Controls.Add(datePicker);

            btnYoB = new RadioButton()
            {
                Checked = false,
                Font = btnDob.Font,
                Height = btnDob.Height,
                Location = new Point(btnDob.Left, datePicker.Bottom + 20),
                Text = "I may know my age",
                Width = btnDob.Width
            };
            btnYoB.CheckedChanged += delegate
            {
                txtAge.Enabled = btnYoB.Checked;
            };
            this.Controls.Add(btnYoB);

            txtAge = new NumericUpDown()//HintedTextBox("estimated age")
            {
                BorderStyle = txtFirstname.BorderStyle,
                Enabled = btnYoB.Checked,
                Font = txtFirstname.Font,
                Maximum = 200,
                Minimum = 0,
                Width = datePicker.Width
            };
            txtAge.Location = new Point(btnYoB.Left + 20, btnYoB.Bottom + 5);
            //UnderlineFor lblLine4 = new UnderlineFor(txtAge, owner.DarkColor, SystemColors.GrayText)
            //{
            //    BackColor = SystemColors.GrayText,
            //};
            //lblLine4.Location = new Point(txtAge.Location.X, txtAge.Bottom);
            this.Controls.Add(txtAge);
            //this.Controls.Add(lblLine4);
            
            genderList = new ComboBox()
            {
                //DropDownStyle = ComboBoxStyle.DropDownList,
                Font = txtFirstname.Font,
                Location = new Point(txtFirstname.Left, txtAge.Bottom + 20),
                Text = "select gender",
                Width = txtFirstname.Width,
            };
            genderList.Items.Add("Male");
            genderList.Items.Add("Female");
            this.Controls.Add(genderList);

            maritalStatusList = new ComboBox()
            {
                //DropDownStyle = ComboBoxStyle.DropDownList,
                Font = txtFirstname.Font,
                Location = new Point(txtFirstname.Left, genderList.Bottom + 20),
                Text = "select marital status",
                Width = txtFirstname.Width,
            };
            maritalStatusList.Items.Add("Single");
            maritalStatusList.Items.Add("Married");
            maritalStatusList.Items.Add("Widowed");
            maritalStatusList.Items.Add("Separated");
            maritalStatusList.Items.Add("Others");
            this.Controls.Add(maritalStatusList);

            statesList = new ComboBox()
            {
                //DropDownStyle = ComboBoxStyle.DropDownList,
                Font = txtFirstname.Font,
                Location = new Point(txtFirstname.Left, maritalStatusList.Bottom + 20),
                Text = "select state",
                Width = txtFirstname.Width,
            };
            statesList.AutoCompleteCustomSource.AddRange(statesAndLGAs.Keys.ToArray());
            statesList.AutoCompleteMode = AutoCompleteMode.Suggest;
            statesList.AutoCompleteSource = AutoCompleteSource.CustomSource;
            statesList.Items.AddRange(statesAndLGAs.Keys.ToArray());
            statesList.SelectedIndexChanged += delegate 
            {
                lgaList.Items.Clear();
                if (statesList.SelectedItem != null)
                {
                    var key = statesList.SelectedItem.ToString();
                    if (statesAndLGAs.ContainsKey(key))
                    {
                        lgaList.Items.AddRange(statesAndLGAs[key]);
                    }
                }
            };
            this.Controls.Add(statesList);

            lgaList = new ComboBox()
            {
                //DropDownStyle = ComboBoxStyle.DropDownList,
                Font = txtFirstname.Font,
                Location = new Point(txtFirstname.Left, statesList.Bottom + 20),
                Text = "select LGA",
                Width = txtFirstname.Width,
            };
            this.Controls.Add(lgaList);

            camControl = new TestForm1.RegistrationForm.CameraControl(owner);
            camControl.Location = new Point(txtFirstname.Right + 50, txtFirstname.Top);
            camControl.Size = new Size(txtFirstname.Width, txtFirstname.Width);
            this.Controls.Add(camControl);

            RecreateEnrollmentControl();

            btnSave = new Button()
            {
                BackColor = owner.DarkColor,
                ForeColor = owner.LightColor,
                Font = new Font(this.Font.FontFamily, 16.0f, FontStyle.Bold),
                Size = new Size(100, 50),
                Text = "Save"
            };
            this.Controls.Add(btnSave);
            btnSave.Click += BtnSave_Click;

            this.SizeChanged += delegate 
            {
                btnSave.Location = new Point((this.Width - btnSave.Width) / 2, this.Height - (btnSave.Height + 2));
            };
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            Idp person = new Idp();
            person.Photo = camControl.CapturedBitmap;
            person.FirstName = txtFirstname.Text;
            person.LastName = txtLastName.Text;
            person.OtherNames = txtOtherNames.Text;
            person.FingerTemplates = capturedTemplates;
            if(btnDob.Checked)
                person.DoB = datePicker.Value;
            if (btnYoB.Checked)
            {
                int currentYear = DateTime.Now.Year;
                person.YoB = currentYear - (int)txtAge.Value;
            }
            if (genderList.SelectedItem != null)
                person.Gender = genderList.SelectedItem.ToString();
            if (maritalStatusList.SelectedItem != null)
                person.MaritalStatus = maritalStatusList.SelectedItem.ToString();
            if (statesList.SelectedItem != null)
                person.State = statesList.SelectedItem.ToString();
            if (lgaList.SelectedItem != null)
                person.LGA = lgaList.SelectedItem.ToString();

            //QRCodeGenerator qrGenerator = new QRCodeGenerator();
            //var content = "ID: " + "user1" + "\n" +
            //    "Name: " + person.FirstName + " " + person.LastName;
            //QRCodeGenerator.QRCode qrCode = qrGenerator.CreateQrCode(content, QRCodeGenerator.ECCLevel.M);
            //IdCardWindow idCardWin = new TestForm1.IdCardWindow(person, qrCode.GetGraphic(20));
            //idCardWin.ShowDialog();

            IdpDb db = new IdpDb();
            string id;
            if (db.SavePerson(person, out id))
            {
                person.ID = id;

                camControl.Reset();
                txtFirstname.ResetText();
                txtFirstname.ShowHint();
                txtLastName.ResetText();
                txtLastName.ShowHint();
                txtOtherNames.ResetText();
                txtOtherNames.ShowHint();
                datePicker.Value = DateTime.Now;
                txtAge.Value = txtAge.Minimum;
                genderList.SelectedItem = null;
                genderList.Text = "select gender";
                maritalStatusList.SelectedItem = null;
                maritalStatusList.Text = "select marital status";
                statesList.SelectedItem = null;
                statesList.Text = "select state";
                lgaList.SelectedItem = null;
                lgaList.Text = "select LGA";
                lgaList.Items.Clear();
                RecreateEnrollmentControl();

                capturedTemplates = null;
                capturedTemplates = new DPFP.Template[10];

                IdCardWindow idCardWin = new TestForm1.IdCardWindow(person);
                idCardWin.ShowDialog();
            }
            else
            {
                MessageBox.Show("Could not save data successfully!");
            }
        }

        //private void EnrollmentControl_OnSampleQuality(object Control, string ReaderSerialNumber, 
        //    int Finger, DPFP.Capture.CaptureFeedback CaptureFeedback)
        //{
        //    //
        //}

        //private void EnrollmentControl_OnComplete(object Control, string ReaderSerialNumber, int Finger)
        //{
        //    //
        //}

        private void EnrollmentControl_OnEnroll(object Control, int FingerMask, 
            DPFP.Template Template, ref DPFP.Gui.EventHandlerStatus EventHandlerStatus)
        {
            capturedTemplates[FingerMask - 1] = Template;
        }

        void PopulateStatesAndLGAs()
        {
            //stopped at: Boripe
            statesAndLGAs = new Dictionary<string, string[]>()
            {
                ["Abia"] = new string[] { "Aba North", "Aba South", "Arochukwu", "Bende" },
                ["Abuja"] = new string[] { "Abaji", "" },
                ["Adawama"] = new string[] { "A", "D" },
                ["Anambra"] = new string[] { "Aguata", "Anambra East", "Anambra West", "Anaocha", "Awka North", "Awka South",
                "Ayamelum"},
                ["Akwa Ibom"] = new string[] { "Abak", "" },
                ["Bauchi"] = new string[] { "Alkaleri", "Bauchi", "Bogoro" },
                ["Bayelsa"] = new string[] { "", "" },
                ["Benue"] = new string[] { "Agatu", "Apa", "Ado" },
                ["Borno"] = new string[] { "Abadam", "Askira/Uba", "Bama", "Bayo", "Biu" },
                ["Cross River"] = new string[] { "Abi", "Akamkpa", "Akpabuyo", "Bakassi", "Bekwarra", "Biase", "Boki" },
                ["Delta"] = new string[] { "Aniocha North", "Aniocha South", "Bomadi" },
                ["Ebonyi"] = new string[] { "Abakaliki", "Afikpo North", "Afikpo South (Edda)" },
                ["Enugu"] = new string[] { "Aninri", "Awgu" },
                ["Edo"] = new string[] { "Akoko-Edo", "" },
                ["Ekiti"] = new string[] { "Ado Ekiti", "D" },
                ["Gombe"] = new string[] { "Akko", "Balanga", "Billiri" },
                ["Imo"] = new string[] { "Aboh Mbaise", "Ahiazu Mbaise" },
                ["Jigawa"] = new string[] { "Auyo", "Babura", "Biriniwa", "Birnin Kudu" },
                ["Kaduna"] = new string[] { "Birnin Gwari", "D" },
                ["Kano"] = new string[] { "Ajingi", "Albasu", "Bagwai", "Bebeji", "Bichi" },
                ["Katsina"] = new string[] { "Bakori", "Batagarawa", "Batsari", "Baure", "Bindawa" },
                ["Kebbi"] = new string[] { "Aleiro", "Arewa Dandi", "Argungu", "Augie", "Bagudo", "Birnin Kebbi" },
                ["Kogi"] = new string[] { "Adavi", "Ajaokuta", "Ankpa", "Bassa" },
                ["Kwara"] = new string[] { "Asa", "Baruten" },
                ["Lagos"] = new string[] { "Agege", "Ajeromi-Ifelodun", "Alimosho", "Amuwo-Odofin", "Apapa", "Badagry" },
                ["Nasarawa"] = new string[] { "Akwanga", "Awe" },
                ["Niger"] = new string[] { "Agaie", "Agwara", "Bida", "Borgu" },
                ["Ogun"] = new string[] { "Abeokuta North", "Abeokuta South", "Ado-Odo/Ota", },
                ["Ondo"] = new string[] { "Akoko North-East", "Akoko North-West", "Akoko South-West", "Akoko South-East",
                "Akure North", "Akure South"},
                ["Osun"] = new string[] { "Atakunmosa East", "Atakunmosa West", "Aiyedaade", "Aiyedire", "Boluwaduro",
                "Boripe"},
                ["Oyo"] = new string[] { "Afijio", "Atiba", "Atisbo" },
                ["Plateau"] = new string[] { "Bokkos", "Barkin Ladi", "Bassa" },
                ["Rivers"] = new string[] { "Abua/Odual", "Ahoada East", "Ahoada West", "Akuku-Toru", "Andoni", "Asari-Toru",
                "Bonny"},
                ["Sokoto"] = new string[] { "Binji", "Bodinga" },
                ["Taraba"] = new string[] { "Ardo Kola", "Bali" },
                ["Yobe"] = new string[] { "Bade", "D" },
                ["Zamfara"] = new string[] { "Anka", "Bakura", "Birnin Magaji/Kiyaw" },
            };
        }

        void RecreateEnrollmentControl() //since I can't find a way to reset the control, I hv to do this
        {
            //first destroy it
            if (enrollmentControl != null)
            {
                this.Controls.Remove(enrollmentControl);
                enrollmentControl.OnEnroll -= EnrollmentControl_OnEnroll;
                enrollmentControl = null;
            }

            //now recreate it
            if (enrollmentControl == null)
            {
                enrollmentControl = new DPFP.Gui.Enrollment.EnrollmentControl();
                enrollmentControl.Location = new Point(camControl.Left, camControl.Bottom + 20);
                enrollmentControl.ReaderSerialNumber = serial;
                enrollmentControl.Size = new Size(txtFirstname.Width, txtFirstname.Width);
                enrollmentControl.OnEnroll += EnrollmentControl_OnEnroll;
                //enrollmentControl.OnComplete += EnrollmentControl_OnComplete;
                //enrollmentControl.OnSampleQuality += EnrollmentControl_OnSampleQuality;
                this.Controls.Add(enrollmentControl);
            }
        }

        private class CameraControl : Panel
        {
            Bitmap currentBitmap;
            bool capture = true;

            ComboBox camerasList;
            PictureBox picBox;
            FilterInfoCollection cameras;
            VideoCaptureDevice device;
            Button btnCaptureRestart;

            public CameraControl(AppWindow owner)
            {
                picBox = new PictureBox()
                {
                    BorderStyle = BorderStyle.Fixed3D,
                    Location = new Point(2, 2)
                };
                this.Controls.Add(picBox);

                camerasList = new ComboBox()
                {
                    Text = "Select camera",
                    //DropDownStyle = ComboBoxStyle.DropDownList,
                };
                camerasList.DropDown += delegate 
                {
                    cameras = new FilterInfoCollection(FilterCategory.VideoInputDevice);
                    camerasList.Items.Clear();
                    foreach (FilterInfo cam in cameras)
                    {
                        camerasList.Items.Add(cam.Name);
                    }
                };
                camerasList.SelectedIndexChanged += delegate 
                {
                    if (device != null && device.IsRunning) { device.Stop(); }

                    device = new VideoCaptureDevice(cameras[camerasList.SelectedIndex].MonikerString);
                    device.NewFrame += (sender, eventargs) =>
                    {
                        currentBitmap = (Bitmap)eventargs.Frame.Clone();
                        //picBox.Image = bitmap;
                        picBox.Image = new Bitmap(currentBitmap, picBox.Size);
                    };
                    device.Start();
                };
                this.Controls.Add(camerasList);

                btnCaptureRestart = new Button()
                {
                    BackColor = owner.DarkColor,
                    ForeColor = owner.LightColor,
                    Text = "Capture"
                };
                this.Controls.Add(btnCaptureRestart);
                btnCaptureRestart.Click += delegate
                {
                    if (capture)
                    {
                        if (device != null && device.IsRunning)
                        {
                            device.Stop();
                            CapturedBitmap = new Bitmap(currentBitmap, picBox.Size);
                            btnCaptureRestart.Text = "Again";
                            capture = !capture;
                        }
                    }
                    else
                    {
                        if (device != null && !device.IsRunning)
                        {
                            device.Start();
                            btnCaptureRestart.Text = "Capture";
                            capture = !capture;
                        }
                    }
                };

                this.SizeChanged += delegate 
                {
                    picBox.Width = this.Width - 4;
                    picBox.Height = this.Height - 30;
                    camerasList.Width = this.Width / 2;
                    camerasList.Location = new Point(2, picBox.Bottom + 2);
                    btnCaptureRestart.Width = this.Width / 4;
                    btnCaptureRestart.Location = new Point(this.Width - (btnCaptureRestart.Width + 2), camerasList.Top);
                };

                this.Disposed += delegate 
                {
                    if (device != null && device.IsRunning) { device.Stop(); }
                };
            }

            public void Reset()
            {
                picBox.Image = currentBitmap = null;
                //capture = true;
                //btnCaptureRestart.Text = "Capture";
            }

            public Bitmap CapturedBitmap { get; private set; }
        }
    }
}
