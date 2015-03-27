using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogDataInterface
{


    [Serializable]
    public class FileSystemLogInfo
    {
        public LogCurveInfo CurveInfo { get; set; }
        public string FileName { get; set; }
        public double MaxValue { get; set; }
        public double MinValue { get; set; }
        public bool NullChannel { get; set; }
        public bool ContainsNulls { get; set; }
    }
}
