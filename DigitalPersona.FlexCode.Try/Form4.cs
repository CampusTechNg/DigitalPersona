using MySql.Data.MySqlClient;
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
    public partial class Form4 : Form
    {
        string myConnectionString = "server=localhost;database=idp_test;uid=root;pwd=root;";

        public Form4()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            MySqlConnection conn = new MySqlConnection(myConnectionString);
            try
            {
                conn.Open();
                MessageBox.Show("Connection Open ! ");
                conn.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Can not open connection ! ");
            }
        }
    }
}
