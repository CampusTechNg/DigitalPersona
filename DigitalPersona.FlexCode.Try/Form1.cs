using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DigitalPersona.FlexCode.Try
{
    public partial class Form1 : Form
    {
        Form2 form2;
        Form3 form3;
        public Form1()
        {
            InitializeComponent();

            form2 = new Try.Form2();
            form3 = new Form3();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            form2.ShowDialog();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            form3.ShowDialog();
        }
    }
}
