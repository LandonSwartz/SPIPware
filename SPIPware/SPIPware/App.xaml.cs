using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using log4net;
using log4net.Config;
using SPIPware.Properties;

namespace SPIPware
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    
    public partial class App : Application
	{
        private static readonly ILog log = LogManager.GetLogger(typeof(App));
        private void Application_Startup(object sender, StartupEventArgs e)
		{
            log4net.Config.XmlConfigurator.Configure();
            log.Info("        =============  Started Logging  =============        ");
            //base.OnStartup(e);
            //if(Settings.Default.SettingsUpdateRequired)
            //{
            //	Settings.Default.Upgrade();
            //	Settings.Default.SettingsUpdateRequired = false;
            //	Settings.Default.Save();
            //}
        }
    }
}
