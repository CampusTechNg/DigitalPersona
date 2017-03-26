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
    public partial class LoginForm : UserControl
    {
        HintedTextBox txtUsername, txtPassword;
        Button btnLogin;

        public LoginForm(AppWindow owner)
        {
            InitializeComponent();

            this.Dock = DockStyle.Fill;
            this.BackColor = owner.LightColor;

            owner.LeftPanel.Enabled = false;

            txtUsername = new HintedTextBox("username")
            {
                BorderStyle = System.Windows.Forms.BorderStyle.None,
                Font = new System.Drawing.Font(this.Font.FontFamily, 11.0F),
                Width = 300
            };
            UnderlineFor lblLine1 = new UnderlineFor(txtUsername, owner.DarkColor, SystemColors.GrayText)
            {
                BackColor = owner.DarkColor,
            };
            this.Controls.Add(txtUsername);
            this.Controls.Add(lblLine1);

            txtPassword = new HintedTextBox("password")
            {
                BorderStyle = txtUsername.BorderStyle,
                Font = txtUsername.Font,
                //IsPasswordBox = true,
                Width = txtUsername.Width
            };
            UnderlineFor lblLine2 = new UnderlineFor(txtPassword, owner.DarkColor, SystemColors.GrayText)
            {
                BackColor = SystemColors.GrayText,
            };
            this.Controls.Add(txtPassword);
            this.Controls.Add(lblLine2);

            btnLogin = new Button()
            {
                BackColor = owner.DarkColor,
                ForeColor = owner.LightColor,
                FlatStyle = FlatStyle.Flat,
                Font = new Font(this.Font.FontFamily, 16.0f, FontStyle.Bold),
                Size = new Size(txtUsername.Width, 50),
                Text = "Login"
            };
            this.Controls.Add(btnLogin);
            btnLogin.Click += delegate
            {
                if (txtUsername.Text == "admin" && txtPassword.Text == "admin")
                {
                    owner.LeftPanel.Enabled = true;
                    owner.Body.Controls.Remove(this);
                }
                else
                {
                    MessageBox.Show("Invalid username and password");
                }
            };

            this.SizeChanged += delegate
            {
                txtPassword.Location = new Point((this.Width - txtPassword.Width) / 2, (this.Height - txtPassword.Height) / 2);
                lblLine2.Location = new Point(txtPassword.Location.X, txtPassword.Bottom);
                txtUsername.Location = new Point(txtPassword.Left, txtPassword.Top - (txtUsername.Height + 20));
                lblLine1.Location = new Point(txtUsername.Location.X, txtUsername.Bottom);
                btnLogin.Location = new Point(txtPassword.Location.X, txtPassword.Bottom + 20);
            };
        }
    }
}
