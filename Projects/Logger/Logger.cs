using System.IO;
using log4net;
using log4net.Appender;
using log4net.Core;
using log4net.Layout;
using log4net.Repository.Hierarchy;

namespace Logger
{
    public class Logger
    {
        private static bool _logInitialized;

        private readonly PatternLayout _layout = new PatternLayout();
        private const string LogPattern = "%d [%t] %-5p %m%n";

        public string DefaultPattern
        {
            get { return LogPattern; }
        }

        public Logger()
        {
            _layout.ConversionPattern = DefaultPattern;
            _layout.ActivateOptions();
        }

        public PatternLayout DefaultLayout
        {
            get { return _layout; }
        }

        public void AddAppender(IAppender appender)
        {
            var hierarchy =
                (Hierarchy)LogManager.GetRepository();

            hierarchy.Root.AddAppender(appender);
        }

        static void InitializeLogger(string fileLocation, string loggerName = "DefaultLog.log")
        {
            var hierarchy = (Hierarchy)LogManager.GetRepository();
            var tracer = new TraceAppender();
            var patternLayout = new PatternLayout { ConversionPattern = LogPattern };

            patternLayout.ActivateOptions();

            tracer.Layout = patternLayout;
            tracer.ActivateOptions();
            hierarchy.Root.AddAppender(tracer);

            var roller = new RollingFileAppender
            {
                Layout = patternLayout,
                AppendToFile = true,
                RollingStyle = RollingFileAppender.RollingMode.Size,
                MaxSizeRollBackups = 4,
                MaximumFileSize = "10MB",
                StaticLogFileName = true,
                File = Path.Combine(GetLicenseFileFolder(fileLocation), loggerName)
            };
            roller.ActivateOptions();
            hierarchy.Root.AddAppender(roller);

            hierarchy.Root.Level = Level.Debug;
            hierarchy.Configured = true;
        }

        private static string GetLicenseFileFolder(string fileLocation)
        {
            string licenseFileFolder = fileLocation;
            if (File.Exists(fileLocation))
            {
                licenseFileFolder = Path.GetDirectoryName(fileLocation);
            }
            return licenseFileFolder;
        }

        public static ILog Create(string fileLocation)
        {
            if (!_logInitialized)
            {
                InitializeLogger(fileLocation, "DataReaderTester.log");
                _logInitialized = true;
            }

            return LogManager.GetLogger("DataReaderTester");
        }
    }
}

