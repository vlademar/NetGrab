using System;
using System.Net;
using System.Windows;
using System.Windows.Threading;
using NetGrab.Properties;
using System.Windows.Forms;

namespace NetGrab
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        private TaskHost taskHost;
        private ILogger logger;
        private readonly NotifyIcon trayIcon;

        public App()
        {
            trayIcon = new NotifyIcon
            {
                ContextMenu = new ContextMenu(new[]
                {
                    new MenuItem("Show", OnMenuShowClick),
                    new MenuItem("Exit", OnMenuCloseClick)
                }),
                Icon = NetGrab.Properties.Resources.tray
            };
        }

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
            trayIcon.Visible = true;

            new MainWindow(taskHost).Show();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            trayIcon.Visible = false;
            base.OnExit(e);
        }

        private void OnAppDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            logger.Add("AppDispatcherUnhandledException : {0}\r\n {1}", e.Exception.Message, e.Exception.StackTrace);
            logger.Flush();
            e.Handled = true;
        }

        private void OnMenuShowClick(object sender, EventArgs eventArgs)
        {
            new MainWindow(taskHost).Show();
        }

        private void OnMenuCloseClick(object sender, EventArgs eventArgs)
        {
            Shutdown();
        }
    }
}
