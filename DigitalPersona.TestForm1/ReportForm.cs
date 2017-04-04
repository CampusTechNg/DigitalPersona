using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace DigitalPersona.TestForm1
{
    public partial class ReportForm : UserControl
    {
        public ReportForm(AppWindow owner)
        {
            InitializeComponent();

            this.Dock = DockStyle.Fill;
            this.BackColor = owner.LightColor;

            this.AutoScroll = true;

            this.Load += delegate 
            {
                IdpDb db = new IdpDb();
                var persons = db.GetPersons();

                ChartArea caGender = new ChartArea()
                {
                    Name = "caGender"
                };
                Legend leGender = new Legend()
                {
                    BackColor = Color.Green,
                    ForeColor = Color.Black,
                    Name = "leGender",
                    Title = "Gender"
                };
                Chart chGender = new Chart()
                {
                    BackColor = Color.LightYellow,
                    Name = "chGender",
                    Location = new Point(100, 100),
                    Size = new Size(400, 400),
                };
                Series serGender = new Series()
                {
                    Name = "serGender",
                    Color = Color.Green,
                    IsVisibleInLegend = true,
                    IsXValueIndexed = true,
                    ChartType = SeriesChartType.Pie
                };
                double females = persons.Count(p => p.Gender.ToLower() == "female");
                double males = persons.Count(p => p.Gender.ToLower() == "male");
                serGender.Points.Add(females);
                serGender.Points.Add(males);
                serGender.Points[0].AxisLabel = females + " (" + (females / (females + males)) * 100 + "%)";
                serGender.Points[0].LegendText = "Female";
                serGender.Points[1].AxisLabel = males + " (" + (males / (females + males)) * 100 + "%)";
                serGender.Points[1].LegendText = "Male";

                chGender.ChartAreas.Add(caGender);
                chGender.ChartAreas[0].BackColor = Color.Transparent;
                chGender.Legends.Add(leGender);
                chGender.Series.Clear();
                chGender.Palette = ChartColorPalette.Fire;
                chGender.Titles.Add("Gender Distribution");
                chGender.Series.Add(serGender);
                chGender.Invalidate();
                this.Controls.Add(chGender);

                ChartArea caMarital = new ChartArea()
                {
                    Name = "caMarital"
                };
                Legend leMarital = new Legend()
                {
                    BackColor = Color.Blue,
                    ForeColor = Color.White,
                    Name = "leMarital",
                    Title = "Marital Status"
                };
                Chart chMarital = new Chart()
                {
                    BackColor = Color.LightBlue,
                    Name = "chMarital",
                    Location = new Point(chGender.Right + 100, chGender.Top),
                    Size = chGender.Size,
                };
                Series serMarital = new Series()
                {
                    Name = "serMarital",
                    Color = Color.Green,
                    IsVisibleInLegend = true,
                    IsXValueIndexed = true,
                    ChartType = SeriesChartType.Pie
                };
                double single = persons.Count(p => p.MaritalStatus.ToLower() == "single");
                double married = persons.Count(p => p.MaritalStatus.ToLower() == "married");
                double widowed = persons.Count(p => p.MaritalStatus.ToLower() == "widowed");
                double separated = persons.Count(p => p.MaritalStatus.ToLower() == "separated");
                double others = persons.Count(p => p.MaritalStatus.ToLower() == "others");
                double totalMarital = single + married + widowed + separated + others;
                serMarital.Points.Add(single);
                serMarital.Points.Add(married);
                serMarital.Points.Add(widowed);
                serMarital.Points.Add(separated);
                serMarital.Points.Add(others);
                serMarital.Points[0].AxisLabel = single + " (" + (single / totalMarital) * 100 + "%)";
                serMarital.Points[0].LegendText = "Single";
                serMarital.Points[1].AxisLabel = married + " (" + (married / totalMarital) * 100 + "%)";
                serMarital.Points[1].LegendText = "Married";
                serMarital.Points[2].AxisLabel = widowed + " (" + (widowed / totalMarital) * 100 + "%)";
                serMarital.Points[2].LegendText = "Widowed";
                serMarital.Points[3].AxisLabel = separated + " (" + (separated / totalMarital) * 100 + "%)";
                serMarital.Points[3].LegendText = "Separated";
                serMarital.Points[4].AxisLabel = others + " (" + (others / totalMarital) * 100 + "%)";
                serMarital.Points[4].LegendText = "Others";

                chMarital.ChartAreas.Add(caMarital);
                chMarital.ChartAreas[0].BackColor = Color.Transparent;
                chMarital.Legends.Add(leMarital);
                chMarital.Series.Clear();
                //chMarital.Palette = ChartColorPalette.Berry;
                chMarital.Titles.Add("Marital Status Distribution");
                chMarital.Series.Add(serMarital);
                chMarital.Invalidate();
                this.Controls.Add(chMarital);

                ChartArea caAge = new ChartArea()
                {
                    Name = "caAge"
                };
                Legend legAge = new Legend()
                {
                    BackColor = Color.Green,
                    ForeColor = Color.Black,
                    Name = "legAge",
                    Title = "Age Group"
                };
                Chart chAge = new Chart()
                {
                    BackColor = Color.LightGreen,
                    Name = "chAge",
                    Location = new Point(chGender.Left, chGender.Bottom + 100),
                    Size = new Size(900, chGender.Height),
                };
                Series serAge = new Series()
                {
                    Name = "serAge",
                    Color = Color.Green,
                    IsVisibleInLegend = false,
                    IsXValueIndexed = true,
                    ChartType = SeriesChartType.Column
                };
                double zeroTo4 = persons.Count(p => age(p) >= 0 && age(p) < 5);
                double fiveTo12 = persons.Count(p => age(p) >= 5 && age(p) < 13);
                double thirteenTo19 = persons.Count(p => age(p) >= 13 && age(p) < 20);
                double twentyTo35 = persons.Count(p => age(p) >= 20 && age(p) < 36);
                double thirtysixTo70 = persons.Count(p => age(p) >= 36 && age(p) < 71);
                double above70 = persons.Count(p => age(p) >= 71);
                double totalAge = zeroTo4 + fiveTo12 + thirteenTo19 + twentyTo35 + thirtysixTo70 + above70;
                serAge.Points.Add(zeroTo4);
                serAge.Points.Add(fiveTo12);
                serAge.Points.Add(thirteenTo19);
                serAge.Points.Add(twentyTo35);
                serAge.Points.Add(thirtysixTo70);
                serAge.Points.Add(above70);
                serAge.Points[0].AxisLabel = "0-4";
                serAge.Points[0].LegendText = "0-4";
                serAge.Points[0].Label = zeroTo4 + " (" + (zeroTo4 / totalAge) * 100 + "%)";
                serAge.Points[1].AxisLabel = "5-12";
                serAge.Points[1].LegendText = "5-12";
                serAge.Points[1].Label = fiveTo12 + " (" + (fiveTo12 / totalAge) * 100 + "%)";
                serAge.Points[2].AxisLabel = "13-19";
                serAge.Points[2].LegendText = "13-19";
                serAge.Points[2].Label = thirteenTo19 + " (" + (thirteenTo19 / totalAge) * 100 + "%)";
                serAge.Points[3].AxisLabel = "20-35";
                serAge.Points[3].LegendText = "20-35";
                serAge.Points[3].Label = twentyTo35 + " (" + (twentyTo35 / totalAge) * 100 + "%)";
                serAge.Points[4].AxisLabel = "36-70";
                serAge.Points[4].LegendText = "36-70";
                serAge.Points[4].Label = thirtysixTo70 + " (" + (thirtysixTo70 / totalAge) * 100 + "%)";
                serAge.Points[5].AxisLabel = "70-~";
                serAge.Points[5].LegendText = "70-~";
                serAge.Points[5].Label = above70 + " (" + (above70 / totalAge) * 100 + "%)";

                chAge.ChartAreas.Add(caAge);
                chAge.ChartAreas[0].BackColor = Color.Transparent;
                chAge.ChartAreas[0].AxisX.MajorGrid.Enabled = false;
                chAge.ChartAreas[0].AxisY.MajorGrid.Enabled = true;
                chAge.Legends.Add(legAge);
                chAge.Series.Clear();
                chAge.Palette = ChartColorPalette.Excel;
                chAge.Titles.Add("Age Distribution");
                chAge.Series.Add(serAge);
                chAge.Invalidate();
                this.Controls.Add(chAge);
            };

            //ChartArea chartArea1 = 
            //    new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            //System.Windows.Forms.DataVisualization.Charting.Legend legend1 = 
            //    new System.Windows.Forms.DataVisualization.Charting.Legend();
            //System.Windows.Forms.DataVisualization.Charting.Chart chart1 =
            //    new System.Windows.Forms.DataVisualization.Charting.Chart();
            //chartArea1.Name = "ChartArea1";
            //chart1.ChartAreas.Add(chartArea1);
            //chart1.Dock = System.Windows.Forms.DockStyle.Fill;
            //legend1.Name = "Legend1";
            //chart1.Legends.Add(legend1);
            //chart1.Location = new System.Drawing.Point(0, 50);
            //chart1.Name = "chart1";
            //// this.chart1.Size = new System.Drawing.Size(284, 212);
            //chart1.TabIndex = 0;
            //chart1.Text = "chart1";
            ////this.Controls.Add(chart1);
            //chart1.Series.Clear();
            //var series1 = new System.Windows.Forms.DataVisualization.Charting.Series
            //{
            //    Name = "Series1",
            //    Color = System.Drawing.Color.Green,
            //    IsVisibleInLegend = false,
            //    IsXValueIndexed = true,
            //    ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line
            //};

            //chart1.Series.Add(series1);

            //for (int i = 0; i < 100; i++)
            //{
            //    series1.Points.AddXY(i, f(i));
            //}
            //chart1.Invalidate();
            //===========================
            ////chart1.Series.Add(new System.Windows.Forms.DataVisualization.Charting.Series("frank", 12));
            //var series = new System.Windows.Forms.DataVisualization.Charting.Series();
            //series.Name = "male";
            //chart1.Series.Add("Male");
            //chart1.Series.Add("Female");
            //Random rdn = new Random();
            //for (int i = 0; i < 50; i++)
            //{
            //    chart1.Series["Male"].Points.AddXY
            //                    (rdn.Next(0, 10), rdn.Next(0, 10));
            //    chart1.Series["Female"].Points.AddXY
            //                    (rdn.Next(0, 10), rdn.Next(0, 10));
            //}
            //chart1.Series["Male"].ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.FastLine;
            //chart1.Series["Male"].Color = Color.Red;

            //chart1.Series["Female"].ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.FastLine;
            //chart1.Series["Female"].Color = Color.Blue;
        }
        //private double f(int i)
        //{
        //    var f1 = 59894 - (8128 * i) + (262 * i * i) - (1.6 * i * i * i);
        //    return f1;
        //}

        private double age(Idp person)
        {
            if(person.YoB > 0)
            {
                return DateTime.Now.Year - person.YoB;
            }
            else
            {
                return DateTime.Now.Year - person.DoB.Year;
            }
        }
    }
}
