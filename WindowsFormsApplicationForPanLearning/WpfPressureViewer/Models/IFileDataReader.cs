using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LogDataInterface;

namespace WpfPressureViewer.Models
{
    public interface IFileDataReader
    {
        List<string> FileExtensions { get; }
        bool GetDataLog(string fileName, out FileSystemDataLog dataLog);
    }
}
