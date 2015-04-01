using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using log4net;

using Logger;

namespace WpfPressureViewer.Utilities
{
    public class DebugLoggingUtilities
    {
        private static readonly ILog _log =
            Logger.Logger.Create(
            System.IO.Path.GetDirectoryName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName));

        public static ILog Log { get { return _log; } }

    }
}
