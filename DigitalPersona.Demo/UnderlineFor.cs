using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DigitalPersona.Demo
{
    public class UnderlineFor : Label
    {
        public UnderlineFor(TextBoxBase target, Color activeColor, Color passiveColor)
        {
            AutoSize = false;
            Height = 1;
            if (target != null)
            {
                Width = target.Width;
                target.GotFocus += delegate { BackColor = activeColor; };
                target.LostFocus += delegate { BackColor = passiveColor; };
            }
        }
    }
}
