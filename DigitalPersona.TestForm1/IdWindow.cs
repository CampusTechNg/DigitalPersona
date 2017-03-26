using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DigitalPersona.TestForm1
{
    public partial class IdWindow : Form
    {
        PictureBox picBox;
        Button btnSave;
        SaveFileDialog saveDialog;

        public IdWindow(Image image)
        {
            InitializeComponent();

            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.Size = new Size(400, 480);

            saveDialog = new SaveFileDialog()
            {
                Filter = "Bitmap|*.bmp|JPEG|*.jpg,*.jpeg|PNG|*.png|All|*.*"
            };

            picBox = new PictureBox()
            {
                Location = new Point(2, 2),
                Size = new Size(this.Width - 4, this.Height - 80)
            };
            this.Controls.Add(picBox);
            picBox.Image = new Bitmap(image, picBox.Size);

            btnSave = new Button()
            {
                Location = new Point(picBox.Left, picBox.Bottom + 2),
                Text = "Save"
            };
            btnSave.Click += delegate 
            {
                if (image != null)
                {
                    if (saveDialog.ShowDialog() == DialogResult.OK)
                    {
                        image.Save(saveDialog.FileName);
                    }
                }
            };
            this.Controls.Add(btnSave);
        }

        public PictureBox PictureBox { get { return picBox; } private set { picBox = value; } }
    }
}
