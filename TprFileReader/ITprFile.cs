using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TprFileReader
{
    public interface ITprFile
    {
        string FileName { get; }
        List<string> HeaderComments { get;  }

        DateTime StartTime { get;  }
        DateTime EndTime { get; }

        List<TprColumnDefinition> ColumnDefinitions { get; }

        void SelectPressureColumn(int column);
        void SelectTimeColumn(int column);
        void SelectTimeColumnsAndFormat(List<int> column, string timeFormat);

        List<double> Times { get; }
        List<double> Pressures { get; }

    }
}
