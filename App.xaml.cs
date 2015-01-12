﻿using System;
using System.IO;
using System.Net;
using System.Text;
using System.Windows;
using System.Windows.Threading;
using System.Xml.Serialization;
using NetGrab.Extensions;
using NetGrab.Properties;
using System.Windows.Forms;
using NetGrab.TOR;

namespace NetGrab
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        private TaskHost taskHost;
        private ILogger logger;
        private ILogger faultLogger;
        private readonly NotifyIcon trayIcon;

        private HttpToSocksRelay relay;

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
            relay = new HttpToSocksRelay(
                9999,
                new SocksServer
                {
                    ProxyAddress = "187.3.9.152",
                    ProxyPort = 1849,
                    DestinationAddress = "ya.ru",
                    DestinationPort = 80,
                    Login = "proxuser",
                    Pass = "freeuse"
                });

            logger = new Logger(Settings.Default.Log);
            faultLogger = new Logger(Settings.Default.FaultLog, true);
            WebProxy proxy = null;
            if (Settings.Default.UseProxy)
            {
                proxy = (Settings.Default.Proxy.IsNullOrEmpty())
                    ? WebProxy.GetDefaultProxy()
                    : new WebProxy("127.0.0.1:8888");

                if (!Settings.Default.ProxyLogin.IsNullOrEmpty())
                {
                    proxy.Credentials = new NetworkCredential(
                        Settings.Default.ProxyLogin,
                        Settings.Default.ProxyPassword);
                }
            }

            var host = new TaskHost
            {
                Logger = logger,
                Proxy = proxy,
                SpeedTest = new SpeedTest()
            };

            taskHost = host;
            trayIcon.Visible = true;

            if (Settings.Default.AutoResume && File.Exists(Settings.Default.LastStateFile))
            {
                var state = File.ReadAllText(Settings.Default.LastStateFile, Encoding.Default);

                var task = new KnowyourmemeComLoaderTaskGroup();
                task.SetState(state);
                taskHost.AddTask(task, Settings.Default.ThreadCount);
                taskHost.Run();
            }
            else
                new MainWindow(taskHost).Show();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            trayIcon.Visible = false;
            base.OnExit(e);
        }

        private void OnAppDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            try
            {
                faultLogger.Add("AppDispatcherUnhandledException : {0}\r\n {1}", e.Exception.Message, e.Exception.StackTrace);
                e.Handled = true;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("FATAL ERRROR : " + ex.Message, ex);
            }
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
