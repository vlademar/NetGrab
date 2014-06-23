using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows;
using System.Windows.Forms;
using NetGrab.Properties;
using Application = System.Windows.Application;

namespace NetGrab
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private NotifyIcon notifyIcon;
        private Host host;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            notifyIcon = new NotifyIcon
            {
                ContextMenu = new ContextMenu(new[]
                {
                    new MenuItem("Exit", (sender, args) => Shutdown())
                }),
                Icon = NetGrab.Properties.Resources.tray,
                Visible = true
            };

            host = new Host();

            host.Run();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            notifyIcon.Visible = false;
            host.Stop();

            base.OnExit(e);
        }
    }
}
