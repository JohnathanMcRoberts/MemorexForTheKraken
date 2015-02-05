using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using log4net;

using Logger;
using DataReaderTester.Models;

namespace DataReaderTester
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private static readonly ILog _log =
            Logger.Logger.Create(
           System.IO.Path.GetDirectoryName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName));

        public static ILog Log { get { return _log; } }

        protected override void OnStartup(StartupEventArgs e)
        {
            // hook on error before app really starts
            AppDomain.CurrentDomain.UnhandledException += App_UnhandledException;

            try
            {
                log4net.Config.XmlConfigurator.Configure();

                _log.Info("Start up");
            }
            catch (Exception ex)
            {
                _log.Error(ex);
            }

            base.OnStartup(e);
        }

        public void App_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            _log.Error("Unhandled Exception:" + e.ExceptionObject);
            MessageBox.Show(e.ExceptionObject.ToString());
        }
    }
}
