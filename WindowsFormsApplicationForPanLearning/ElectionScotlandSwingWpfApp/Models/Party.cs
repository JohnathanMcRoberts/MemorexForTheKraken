using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElectionScotlandSwingWpfApp.Models
{
    public class Party
    {
        public string Name { get; set; }
        public double Percentage { get; set; }

        public Party() 
        { 
            Percentage = 0;
            Name ="Not Set";
        }
    }
}
