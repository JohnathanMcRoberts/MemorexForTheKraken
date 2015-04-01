using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using log4net;

using Logger;
using WpfPressureViewer.Models;
using WpfPressureViewer.Utilities;

namespace WpfPressureViewer
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static ILog Log { get { return DebugLoggingUtilities.Log; } }
        protected override void OnStartup(StartupEventArgs e)
        {
            // hook on error before app really starts
            AppDomain.CurrentDomain.UnhandledException += App_UnhandledException;

            try
            {
                log4net.Config.XmlConfigurator.Configure();

                DebugLoggingUtilities.Log.Info("Start up");
            }
            catch (Exception ex)
            {
                DebugLoggingUtilities.Log.Error(ex);
            }

            base.OnStartup(e);
        }

        public void App_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Log.Error("Unhandled Exception:" + e.ExceptionObject);
            MessageBox.Show(e.ExceptionObject.ToString());
        }
    }
}
