using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DigitalPersona.Demo
{
    public partial class RegistrationForm : UserControl
    {
        HintedTextBox txtFirstName;
        public RegistrationForm(AppWindow owner)
        {
            InitializeComponent();

            this.Dock = DockStyle.Fill;
            this.BackColor = Color.White;

            txtFirstName = new HintedTextBox("first name")
            {
                BorderStyle = System.Windows.Forms.BorderStyle.None,
                Font = new System.Drawing.Font(this.Font.FontFamily, 11.0F),
                Width = 400
            };
            txtFirstName.Location = new Point(20, 20);
            UnderlineFor lblLine1 = new UnderlineFor(txtFirstName, owner.DarkColor, SystemColors.GrayText)
            {
                BackColor = owner.DarkColor,
            };
            lblLine1.Location = new Point(txtFirstName.Location.X, txtFirstName.Bottom);
            this.Controls.Add(txtFirstName);
            this.Controls.Add(lblLine1);
        }
    }
}
