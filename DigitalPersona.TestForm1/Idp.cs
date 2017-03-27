using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitalPersona.TestForm1
{
    public class Idp
    {
        public string ID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string OtherNames { get; set; }
        public DateTime DoB { get; set; }
        public int YoB { get; set; } = 0;
        public Image Photo { get; set; }
        public DPFP.Template[] FingerTemplates { get; set; } = new DPFP.Template[10];
        public string Gender { get; set; }
        public string MaritalStatus { get; set; }
        public string State { get; set; }
        public string LGA { get; set; }
    }
}
