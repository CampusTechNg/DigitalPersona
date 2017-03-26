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
    public partial class AppWindow : Form
    {
        LeftPanel pnlLeft;
        Container container;
        TitleBar titleBar;
        StatusBar statusBar;
        TopFrame topFrame;
        BottomFrame bottomFrame;
        LeftFrame leftFrame;
        RightFrame rightFrame;
        bool max;
        Point restoreLocation = new Point();
        Size restoreSize = new Size();

        public AppWindow()
        {
            InitializeComponent();

            restoreLocation = this.Location;
            restoreSize = this.Size = new Size(900, 600);

            this.BackColor = this.DarkColor;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = Properties.Resources.Icon;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Text = "IDPs Biometric Enumeration";
            //this.MinimumSize = this.Size;
            //RegisterToMoveWindow(this);
            maximizedChanged = new EventHandler(DefaultEventHandler);

            //container
            container = new Container(this);
            this.Controls.Add(container);

            //titleBar
            titleBar = new TitleBar(this);
            this.Controls.Add(titleBar);
            RegisterToMoveWindow(titleBar);
            this.TitleBar.RefreshTitleBar(true, true, false);

            //left panel
            pnlLeft = new LeftPanel(this);
            this.Controls.Add(pnlLeft);

            //statusBar
            statusBar = new StatusBar(this);
            //this.Controls.Add(statusBar);

            //topFrame
            topFrame = new TopFrame(this);
            this.Controls.Add(topFrame);
            //bottomFrame
            bottomFrame = new BottomFrame(this);
            this.Controls.Add(bottomFrame);
            //leftFrame
            leftFrame = new LeftFrame(this);
            this.Controls.Add(leftFrame);
            //rightFrame
            rightFrame = new RightFrame(this);
            this.Controls.Add(rightFrame);

            //login form
            Body.Controls.Add(new LoginForm(this));

            FitToScreen();
        }

        Point initialPoint, finalPoint;
        protected internal void RegisterToMoveWindow(Control control)
        {
            control.DoubleClick += delegate { this.Maximized = !this.Maximized; };
            control.MouseDown += delegate (object sender, MouseEventArgs e)
            {
                initialPoint = e.Location;
            };
            control.MouseMove += delegate (object sender, MouseEventArgs e)
            {
                if (!Maximized)
                {
                    if (e.Button == System.Windows.Forms.MouseButtons.Left)
                    {
                        finalPoint = e.Location;

                        int deltaX = (finalPoint.X - initialPoint.X);
                        int deltaY = (finalPoint.Y - initialPoint.Y);
                        this.Location = new Point(this.Location.X + deltaX, this.Location.Y + deltaY);
                    }
                }
            };
        }

        void FitToScreen()
        {
            restoreLocation = this.Location;
            restoreSize = this.Size;
            this.Location = new Point(0, 0);
            Rectangle rect = new Rectangle(int.MaxValue, int.MaxValue, int.MinValue, int.MinValue);
            rect = Rectangle.Union(rect, Screen.PrimaryScreen.WorkingArea);
            this.Size = new System.Drawing.Size(rect.Width, rect.Height);
            //topFrame.Visible = bottomFrame.Visible = leftFrame.Visible = rightFrame.Visible = false;
        }
        void UnfitFromScreen()
        {
            this.Location = restoreLocation;
            this.Size = restoreSize;
            //topFrame.Visible = bottomFrame.Visible = leftFrame.Visible = rightFrame.Visible = true;
        }

        public Container Body { get { return container; } }
        public Color DarkColor { get { return Color.FromArgb(02, 93, 171); } }//DodgerBlue, SkyBlue, MediumSlateBlue
        /// <summary>
        /// Gets or sets a value to determine if the window is fixed or can be resized
        /// </summary>
        public bool IsFixed { get; set; }
        public LeftPanel LeftPanel { get { return pnlLeft; } }
        public Color LightColor { get { return Color.White; } }
        /// <summary>
        /// Gets or sets a value to determine if the window is maximized
        /// </summary>
        public bool Maximized
        {
            get { return max; }
            set
            {
                if (!this.IsFixed && max != value)
                {
                    if (value)
                        FitToScreen();
                    else
                        UnfitFromScreen();

                    max = value;
                    maximizedChanged(this, new EventArgs());
                }
            }
        }
        public StatusBar StatusBar { get { return statusBar; } }
        public TitleBar TitleBar { get { return titleBar; } }

        void DefaultEventHandler(object sender, EventArgs e) { }
        event EventHandler maximizedChanged;
        public event EventHandler MaximizedChanged
        {
            add
            {
                if (maximizedChanged == null)
                    maximizedChanged = new EventHandler(DefaultEventHandler);
                maximizedChanged += value;
            }
            remove
            {
                if (maximizedChanged == null)
                    maximizedChanged = new EventHandler(DefaultEventHandler);
                maximizedChanged -= value;
            }
        }

        private abstract class Frame : Panel
        {
            protected Point initialPoint, finalPoint;
            protected int thickness = 2, cornerThickness = 4;

            public Frame(AppWindow owner)
            {
                this.BackColor = owner.DarkColor;

                this.MouseDown += delegate (object sender, MouseEventArgs e)
                {
                    initialPoint = e.Location;
                };
            }
        }
        private class TopFrame : Frame
        {
            public TopFrame(AppWindow owner) : base(owner)
            {
                this.Dock = DockStyle.Top;
                this.Height = thickness;

                this.MouseMove += delegate (object sender, MouseEventArgs e)
                {
                    if (!owner.Maximized && !owner.IsFixed)
                    {


                        if (e.Button == System.Windows.Forms.MouseButtons.Left)
                        {
                            finalPoint = e.Location;

                            int deltaX = (finalPoint.X - initialPoint.X);
                            int deltaY = (finalPoint.Y - initialPoint.Y);

                            if (this.Cursor == Cursors.SizeNS)
                            {
                                owner.Location = new Point(owner.Location.X, owner.Location.Y + deltaY);
                                owner.Height -= deltaY;
                            }
                            else if (this.Cursor == Cursors.SizeNWSE)
                            {
                                owner.Location = new Point(owner.Location.X + deltaX, owner.Location.Y + deltaY);
                                owner.Size = new Size(owner.Width - deltaX, owner.Height - deltaY);
                            }
                            else if (this.Cursor == Cursors.SizeNESW)
                            {
                                deltaX = -deltaY;//a hack to solve a problem I cant figure out
                                owner.Location = new Point(owner.Location.X, owner.Location.Y + deltaY);
                                owner.Height -= deltaY;
                                owner.Width += deltaX;
                            }
                        }
                        else
                        {
                            if (e.X <= cornerThickness)
                            {
                                this.Cursor = Cursors.SizeNWSE;
                            }
                            else if ((this.Width - e.X) <= cornerThickness)
                            {
                                this.Cursor = Cursors.SizeNESW;
                            }
                            else
                            {
                                this.Cursor = Cursors.SizeNS;
                            }
                        }
                    }
                };
            }
        }
        private class BottomFrame : Frame
        {
            public BottomFrame(AppWindow owner)
                : base(owner)
            {
                this.Dock = DockStyle.Bottom;
                this.Height = thickness;

                this.MouseMove += delegate (object sender, MouseEventArgs e)
                {
                    if (!owner.Maximized && !owner.IsFixed)
                    {
                        if (e.Button == System.Windows.Forms.MouseButtons.Left)
                        {
                            finalPoint = e.Location;

                            int deltaX = (finalPoint.X - initialPoint.X);
                            int deltaY = (finalPoint.Y - initialPoint.Y);

                            if (this.Cursor == Cursors.SizeNS)
                            {
                                owner.Height += deltaY;
                            }
                            else if (this.Cursor == Cursors.SizeNWSE)
                            {
                                deltaX = deltaY;//a hack to solve a problem I cant figure out
                                owner.Size = new Size(owner.Width + deltaX, owner.Height + deltaY);
                            }
                            else if (this.Cursor == Cursors.SizeNESW)
                            {
                                deltaY = -deltaX;//a hack to solve a problem I cant figure out
                                owner.Location = new Point(owner.Location.X + deltaX, owner.Location.Y);
                                owner.Size = new Size(owner.Width - deltaX, owner.Height + deltaY);
                            }
                        }
                        else
                        {
                            if (e.X <= cornerThickness)
                            {
                                this.Cursor = Cursors.SizeNESW;
                            }
                            else if ((this.Width - e.X) <= cornerThickness)
                            {
                                this.Cursor = Cursors.SizeNWSE;
                            }
                            else
                            {
                                this.Cursor = Cursors.SizeNS;
                            }
                        }
                    }
                };
            }
        }
        private class LeftFrame : Frame
        {
            public LeftFrame(AppWindow owner)
                : base(owner)
            {
                this.Dock = DockStyle.Left;
                this.Width = thickness;

                this.MouseMove += delegate (object sender, MouseEventArgs e)
                {
                    if (!owner.Maximized && !owner.IsFixed)
                    {
                        if (e.Button == System.Windows.Forms.MouseButtons.Left)
                        {
                            finalPoint = e.Location;

                            int deltaX = (finalPoint.X - initialPoint.X);
                            int deltaY = (finalPoint.Y - initialPoint.Y);

                            if (this.Cursor == Cursors.SizeWE)
                            {
                                owner.Location = new Point(owner.Location.X + deltaX, owner.Location.Y);
                                owner.Width -= deltaX;
                            }
                            else if (this.Cursor == Cursors.SizeNWSE)
                            {
                                owner.Location = new Point(owner.Location.X + deltaX, owner.Location.Y + deltaY);
                                owner.Size = new Size(owner.Width - deltaX, owner.Height - deltaY);
                            }
                            else if (this.Cursor == Cursors.SizeNESW)
                            {
                                deltaY = -deltaX;//a hack to solve a problem I cant figure out
                                owner.Location = new Point(owner.Location.X + deltaX, owner.Location.Y);
                                owner.Size = new Size(owner.Width - deltaX, owner.Height + deltaY);
                            }
                        }
                        else
                        {
                            if (e.Y <= cornerThickness)
                            {
                                this.Cursor = Cursors.SizeNWSE;
                            }
                            else if ((this.Height - e.Y) <= cornerThickness)
                            {
                                this.Cursor = Cursors.SizeNESW;
                            }
                            else
                            {
                                this.Cursor = Cursors.SizeWE;
                            }
                        }
                    }
                };
            }
        }
        private class RightFrame : Frame
        {
            public RightFrame(AppWindow owner)
                : base(owner)
            {
                this.Dock = DockStyle.Right;
                this.Width = thickness;

                this.MouseMove += delegate (object sender, MouseEventArgs e)
                {
                    if (!owner.Maximized && !owner.IsFixed)
                    {
                        if (e.Button == System.Windows.Forms.MouseButtons.Left)
                        {
                            finalPoint = e.Location;

                            int deltaX = (finalPoint.X - initialPoint.X);
                            int deltaY = (finalPoint.Y - initialPoint.Y);

                            if (this.Cursor == Cursors.SizeWE)
                            {
                                owner.Width += deltaX;
                            }
                            else if (this.Cursor == Cursors.SizeNWSE)
                            {
                                deltaX = deltaY;//a hack to solve a problem I cant figure out
                                owner.Size = new Size(owner.Width + deltaX, owner.Height + deltaY);
                            }
                            else if (this.Cursor == Cursors.SizeNESW)
                            {
                                deltaY = -deltaX;//a hack to solve a problem I cant figure out
                                owner.Location = new Point(owner.Location.X, owner.Location.Y + deltaY);
                                owner.Size = new Size(owner.Width + deltaX, owner.Height - deltaY);
                            }
                        }
                        else
                        {
                            if (e.Y <= cornerThickness)
                            {
                                this.Cursor = Cursors.SizeNESW;
                            }
                            else if ((this.Height - e.Y) <= cornerThickness)
                            {
                                this.Cursor = Cursors.SizeNWSE;
                            }
                            else
                            {
                                this.Cursor = Cursors.SizeWE;
                            }
                        }
                    }
                };
            }
        }
    }

    public class TitleBar : Panel
    {
        AppWindow owner;
        Label btnIconBox, lblAppName, btnMinimize, btnMaximize, btnClose;
        List<Control> customControls;
        Panel dynamicPanel;
        int offset = 1, smSz = 22, sz = 24;

        public TitleBar(AppWindow owner)
        {
            this.owner = owner;

            this.BackColor = owner.LightColor;//this.BackColor = owner.DarkColor;
            this.Dock = DockStyle.Top;
            this.ForeColor = owner.DarkColor;
            this.Height = sz;

            ToolTip tooltip = new ToolTip();

            //customControls
            customControls = new List<Control>();

            //btnMinimize
            btnMinimize = new Label()
            {
                BackgroundImage = Properties.Resources.Minimize,
                BackgroundImageLayout = ImageLayout.Stretch,
                Size = new Size(smSz, smSz)
            };
            btnMinimize.MouseEnter += delegate { btnMinimize.BackgroundImage = Properties.Resources.Minimize2; };
            btnMinimize.MouseLeave += delegate { btnMinimize.BackgroundImage = Properties.Resources.Minimize; };
            btnMinimize.Click += delegate { owner.WindowState = FormWindowState.Minimized; };

            if (owner.Maximized)
            {
                MaximizeImageNormal = Properties.Resources.Restore;
                MaximizeImageFocus = Properties.Resources.Restore2;
            }
            else
            {
                MaximizeImageNormal = Properties.Resources.Maximize;
                MaximizeImageFocus = Properties.Resources.Maximize2;
            }
            //btnMaximize
            btnMaximize = new Label()
            {
                BackgroundImage = MaximizeImageNormal,
                BackgroundImageLayout = ImageLayout.Stretch,
                Size = new Size(smSz, smSz)
            };
            btnMaximize.MouseEnter += delegate { btnMaximize.BackgroundImage = MaximizeImageFocus; };
            btnMaximize.MouseLeave += delegate { btnMaximize.BackgroundImage = MaximizeImageNormal; };
            btnMaximize.Click += delegate { owner.Maximized = !owner.Maximized; };

            //btnClose
            btnClose = new Label()
            {
                BackgroundImage = Properties.Resources.Close,
                BackgroundImageLayout = ImageLayout.Stretch,
                Size = new Size(smSz, smSz)
            };
            btnClose.MouseEnter += delegate { btnClose.BackgroundImage = Properties.Resources.Close2; };
            btnClose.MouseLeave += delegate { btnClose.BackgroundImage = Properties.Resources.Close; };
            btnClose.Click += delegate { owner.Close(); };

            //dynamicPanel
            dynamicPanel = new Panel()
            {
                Height = 30,
                Width = 0
            };
            this.Controls.Add(dynamicPanel);

            //btnIconBox
            btnIconBox = new Label()
            {
                BackgroundImage = Properties.Resources.IconImage,
                BackgroundImageLayout = ImageLayout.Stretch,
                Location = new Point(offset, offset),
                Size = new Size(smSz, smSz)
            };
            owner.RegisterToMoveWindow(btnIconBox);
            this.Controls.Add(btnIconBox);

            //lblAppName
            lblAppName = new Label()
            {
                AutoEllipsis = true,
                Font = new System.Drawing.Font(this.Font.FontFamily, 11.0F),
                Height = smSz,
                Text = owner.Text,
                TextAlign = ContentAlignment.BottomLeft
            };
            lblAppName.Location = new Point(btnIconBox.Location.X + btnIconBox.Width + offset, offset);
            this.Controls.Add(lblAppName);
            owner.RegisterToMoveWindow(lblAppName);
            owner.SizeChanged += delegate { lblAppName.Width = owner.Width / 2; };
            owner.TextChanged += delegate { lblAppName.Text = owner.Text; };

            owner.MaximizedChanged += owner_MaximizedChanged;

            this.SizeChanged += TitleBar_SizeChanged;

            RefreshTitleBar(true, true, true);
        }
        void owner_MaximizedChanged(object sender, EventArgs e)
        {
            if (owner.Maximized)
            {
                //MaximizeImageNormal = Properties.Resources.Restore;
                //MaximizeImageFocus = Properties.Resources.Restore2;
            }
            else
            {
                //MaximizeImageNormal = Properties.Resources.Maximize;
                //MaximizeImageFocus = Properties.Resources.Maximize2;
            }

            btnMaximize.BackgroundImage = MaximizeImageNormal;
        }

        void TitleBar_SizeChanged(object sender, EventArgs e)
        {
            dynamicPanel.Location = new Point(this.Width - (dynamicPanel.Width + offset), 0);
        }

        public void RefreshTitleBar(bool showMaximizeButton, bool showMinimizeButton, bool showCustomControls)
        {
            dynamicPanel.Controls.Clear();

            dynamicPanel.Width = 0;
            int currentX = offset;
            int increment = 0;
            dynamicPanel.Location = new Point(this.Width - (dynamicPanel.Width + offset), 0);

            //add custom controls
            if (showCustomControls)
            {
                foreach (var control in customControls)
                {
                    increment = (offset + control.Width);
                    control.Location = new Point(currentX, offset);
                    dynamicPanel.Width += increment;
                    dynamicPanel.Location = new Point(this.Width - (dynamicPanel.Width + offset), 0);
                    currentX += increment;
                    dynamicPanel.Controls.Add(control);
                }
            }

            //add minimize
            if (showMinimizeButton)
            {
                increment = (offset + btnMinimize.Width);
                btnMinimize.Location = new Point(currentX, offset);
                dynamicPanel.Width += increment;
                dynamicPanel.Location = new Point(this.Width - (dynamicPanel.Width + offset), 0);
                currentX += increment;
                dynamicPanel.Controls.Add(btnMinimize);
            }

            //add maximize
            if (showMaximizeButton)
            {
                increment = (offset + btnMaximize.Width);
                btnMaximize.Location = new Point(currentX, offset);
                dynamicPanel.Width += increment;
                dynamicPanel.Location = new Point(this.Width - (dynamicPanel.Width + offset), 0);
                currentX += increment;
                dynamicPanel.Controls.Add(btnMaximize);
            }

            //add close
            increment = (offset + btnClose.Width);
            btnClose.Location = new Point(currentX, offset);
            dynamicPanel.Width += increment;
            dynamicPanel.Location = new Point(this.Width - (dynamicPanel.Width + offset), 0);
            currentX += increment;
            dynamicPanel.Controls.Add(btnClose);
        }

        /// <summary>
        /// Adds a custom control to the title bar
        /// </summary>
        /// <param name="control"></param>
        public void AddCustomControl(Control control)
        {
            if (customControls.Contains(control))
                customControls.Remove(control);
            customControls.Add(control);
        }
        /// <summary>
        /// Removes a custom control from the title bar
        /// </summary>
        /// <param name="control"></param>
        public void RemoveCustomControl(Control control)
        {
            if (customControls.Contains(control))
            {
                customControls.Remove(control);
            }
        }

        Image MaximizeImageNormal { get; set; }
        Image MaximizeImageFocus { get; set; }
    }

    public class Container : Panel
    {
        public Container(AppWindow owner)
        {
            this.BackColor = owner.LightColor;
            this.Dock = DockStyle.Fill;
        }
    }

    public class StatusBar : Panel
    {
        int sz = 24;

        public StatusBar(AppWindow owner)
        {
            this.BackColor = owner.DarkColor;
            this.Dock = DockStyle.Bottom;
            this.ForeColor = owner.LightColor;
            this.Height = sz;
        }
    }

    public class LeftPanel : Panel
    {
        RegistrationForm regForm;
        VerificationForm veriForm;
        public LeftPanel(AppWindow owner)
        {
            this.BackColor = owner.DarkColor;
            this.Dock = DockStyle.Left;
            this.Width = 300;

            Label lblLogo = new Label()
            {
                AutoSize = false,
                Dock = DockStyle.Top,
                Height = 150
            };
            lblLogo.BackgroundImage = Properties.Resources.LogoWhite;
            lblLogo.BackgroundImageLayout = ImageLayout.Zoom;
            this.Controls.Add(lblLogo);

            Label lblName = new Label()
            {
                AutoSize = false,
                Font = new Font(this.Font.FontFamily, 13.0f, FontStyle.Bold),
                ForeColor = owner.LightColor,
                Location = new Point(0, lblLogo.Bottom + 10),
                Size = new Size(300, 30),
                Text = owner.Text,
                TextAlign = ContentAlignment.MiddleCenter
            };
            this.Controls.Add(lblName);

            //registration
            LeftPanelButton btnReg = new LeftPanelButton(owner)
            {
                Text = "Enrollment"
            };
            btnReg.Location = new Point(this.Width - btnReg.Width, lblName.Bottom + 10);
            btnReg.Click += delegate 
            {
                if (regForm == null)
                {
                    regForm = new RegistrationForm(owner);
                }
                owner.Body.Controls.Clear();
                owner.Body.Controls.Add(regForm);
                owner.Text = btnReg.Text + " - IDPs Biometric Enumeration";
            };
            this.Controls.Add(btnReg);
            //verification
            LeftPanelButton btnVeri = new LeftPanelButton(owner)
            {
                Text = "Verification"
            };
            btnVeri.Location = new Point(btnReg.Left, btnReg.Bottom + 10);
            btnVeri.Click += delegate
            {
                if (veriForm == null)
                {
                    veriForm = new VerificationForm(owner);
                }
                owner.Body.Controls.Clear();
                owner.Body.Controls.Add(veriForm);
                owner.Text = btnVeri.Text + " - IDPs Biometric Enumeration";
            };
            this.Controls.Add(btnVeri);
            //sync

            //
            btnReg.Siblings.Add(btnVeri);
            btnVeri.Siblings.Add(btnReg);

            owner.RegisterToMoveWindow(this);
            owner.RegisterToMoveWindow(lblLogo);
            owner.RegisterToMoveWindow(lblName);
        }
    }

    public class LeftPanelButton : Button
    {
        AppWindow owner;

        public LeftPanelButton(AppWindow owner)
        {
            this.owner = owner;

            this.Font = new Font(this.Font.FontFamily, 15.0f);
            this.FlatAppearance.BorderSize = 0;
            this.FlatStyle = FlatStyle.Flat;
            this.Height = 40;
            this.Width = 280;
            this.TextImageRelation = TextImageRelation.ImageBeforeText;

            this.Click += delegate
            {
                this.Highlight();
                this.Siblings.ForEach(c => c.Unhighlight());
            };

            this.Unhighlight();
        }
        public void Highlight()
        {
            this.BackColor = owner.LightColor;
            this.ForeColor = owner.DarkColor;
        }
        public void Unhighlight()
        {
            this.BackColor = owner.DarkColor;
            this.ForeColor = owner.LightColor;
        }
        public List<LeftPanelButton> Siblings { get; set; } = new List<LeftPanelButton>();
    }
}
