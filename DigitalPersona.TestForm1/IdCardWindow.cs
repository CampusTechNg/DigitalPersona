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
    public partial class IdCardWindow : Form
    {
        PictureBox picBox;
        Button btnSave;
        SaveFileDialog saveDialog;

        public IdCardWindow(Idp person)
        {
            InitializeComponent();

            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MinimizeBox = this.MaximizeBox = false;
            this.Icon = Properties.Resources.Icon;
            this.Size = new Size(310, 520);
            this.StartPosition = FormStartPosition.CenterParent;

            QRCodeGenerator qrGenerator = new QRCodeGenerator();
            var content = "ID: " + person.ID + "\n" +
                "Name: " + person.FirstName + " " + person.LastName;
            QRCodeGenerator.QRCode qrCode = qrGenerator.CreateQrCode(content, QRCodeGenerator.ECCLevel.M);
            Image qrcodeImg = qrCode.GetGraphic(20);

            saveDialog = new SaveFileDialog()
            {
                Filter = "Bitmap|*.bmp|JPEG|*.jpg,*.jpeg|PNG|*.png|All|*.*"
            };

            picBox = new PictureBox()
            {
                Location = new Point(2, 2),
                Size = new Size(this.Width - 20, this.Height - 70)
            };
            this.Controls.Add(picBox);

            //draw card
            Size size = picBox.Size;
            Bitmap bmp = new Bitmap(qrcodeImg, size);
            Graphics g = Graphics.FromImage(bmp);
            //paint background
            g.Clear(Color.White);
            //paint border
            Rectangle outerRect = new Rectangle(new Point(1, 1), new Size(size.Width - 2, size.Height - 2));
            Rectangle innerRect = new Rectangle(new Point(3, 3), new Size(size.Width - 6, size.Height - 6));
            g.DrawRectangles(new Pen(Color.Black, 1f), new Rectangle[] { outerRect, innerRect });
            //paint photo
            Rectangle photoRect = new Rectangle(new Point(innerRect.X, innerRect.Y), 
                new Size(innerRect.Width, innerRect.Height / 2));
            g.DrawImage(person.Photo, photoRect);
            //paint below photo
            Rectangle belowRect = new Rectangle(new Point(photoRect.X, photoRect.Bottom), photoRect.Size);
            SolidBrush br = new SolidBrush(Color.FromArgb(02, 93, 171));
            g.FillRectangle(br, belowRect);
            //write info
            string info = person.FirstName + " " + person.LastName;
            int age = 0;
            if (person.YoB > 0)
            {
                age = DateTime.Now.Year - person.YoB;
                info += "\nAbout " + age + " year(s) old";
            }
            else
            {
                age = DateTime.Now.Year - person.DoB.Year;
                info += "\n" + age + " year(s) old";
            }
            info += "\n" + person.Gender + 
                (!string.IsNullOrEmpty(person.MaritalStatus)? ", " + person.MaritalStatus: "");
            g.DrawString(info, new Font(this.Font.FontFamily, 16.0f, FontStyle.Regular), Brushes.White, 
                new Point(belowRect.Left + 10, belowRect.Top + 10));
            //draw qr code
            Rectangle qrRect = new Rectangle(new Point(belowRect.Left + 10, (belowRect.Bottom - ((2 * belowRect.Height / 5) + 30))), 
                new Size((2 * belowRect.Width / 5), (2 * belowRect.Height / 5)));
            g.DrawImage(qrcodeImg, qrRect);
            //draw logo
            Rectangle logoRect = new Rectangle(new Point(qrRect.Left + 15, qrRect.Bottom + 2), new Size(26, 26));
            g.DrawImage(Properties.Resources.LogoWhite, logoRect);
            //write card footer
            g.DrawString("IDPs Relief Support Card", new Font(this.Font.FontFamily, 12.0f, FontStyle.Regular), Brushes.White,
                new Point(logoRect.Right + 10, logoRect.Top + 5));
            picBox.Image = bmp;

            btnSave = new Button()
            {
                Location = new Point(picBox.Left, picBox.Bottom + 2),
                Text = "Save"
            };
            btnSave.Click += delegate 
            {
                if (picBox.Image != null)
                {
                    if (saveDialog.ShowDialog() == DialogResult.OK)
                    {
                        picBox.Image.Save(saveDialog.FileName);
                    }
                }
            };
            this.Controls.Add(btnSave);
        }

        public PictureBox PictureBox { get { return picBox; } private set { picBox = value; } }
    }
}
