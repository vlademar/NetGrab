using System.Net;
using System.Windows;
using System.Windows.Threading;
using NetGrab.Properties;
using Application = System.Windows.Application;

namespace NetGrab
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        private ITaskHost taskHost;
        private ILogger logger;

        private void OnAppStartup(object sender, StartupEventArgs e)
        {
            logger = new Logger(Settings.Default.Log);

            WebProxy proxy = null;
            if (Settings.Default.UseProxy)
            {
                proxy = WebProxy.GetDefaultProxy();
                if (!string.IsNullOrEmpty(Settings.Default.ProxyLogin))
                {
                    proxy.Credentials = new NetworkCredential(
                        Settings.Default.ProxyLogin,
                        Settings.Default.ProxyPassword);
                }
            }

            var host = new TaskHost
            {
                Logger = logger,
                Proxy = proxy
            };

            taskHost = host;

            var mainWindow = new MainWindow(taskHost);
            mainWindow.Show();
        }

        private void OnAppDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            logger.Add(string.Format("AppDispatcherUnhandledException : {0}\r\n {1}", e.Exception.Message, e.Exception.StackTrace), true);
            e.Handled = true;
        }
    }
}
