using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SPIPware.Util
{
	static class UpdateCheck
	{
        private static readonly ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        static WebClient client;
		static Regex versionRegex = new Regex("\"tag_name\":\\s*\"v([0-9\\.]+)\",");
		static Regex releaseRegex = new Regex("\"html_url\":\\s*\"([^\"]*)\",");

		public static void CheckForUpdate()
		{
			client = new WebClient();
			client.Headers["User-Agent"] = "Mozilla/5.0 (Windows; U; Windows NT 6.1; en-US; rv:1.9.2.15) Gecko/20110303 Firefox/3.6.15";
			client.Proxy = null;
			client.DownloadStringCompleted += Client_DownloadStringCompleted;
			client.DownloadStringAsync(new Uri("https://api.github.com/repos/alexbeattie42/SPIPware/releases/latest"));
        }

		private static void Client_DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
		{
			try
			{
				if (e.Error != null)
				{
					_log.Error("Error while checking for new version:");
					_log.Error(e.Error.Message);
					return;
				}

				Match m = versionRegex.Match(e.Result);

				if (!m.Success)
				{
					_log.Error("No matching tag_id found");
                    _log.Error(e.Result);
                    return;
				}

				Version latest;

				if (!Version.TryParse(m.Groups[1].Value, out latest))
				{
					_log.Error($"Error while parsing version string <{m.Groups[1].Value}>");
					return;
				}

				_log.Info($"Latest version on GitHub: {latest}");

				if (System.Reflection.Assembly.GetEntryAssembly().GetName().Version < latest)
				{
					Match urlMatch = releaseRegex.Match(e.Result);

					string url = "https://api.github.com/repos/alexbeattie42/SPIPware/releases";

					if (urlMatch.Success)
					{
						url = urlMatch.Groups[1].Value;
					}

					if (MessageBox.Show("There is an update available!\nOpen in browser?", "Update", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
						System.Diagnostics.Process.Start(url);
				}
			}
			catch (Exception ex) {
                _log.Error("Encountered error checking for updates!" + ex);
            }	//update check is non-critical and should never interrupt normal application operation
		}
	}
}
