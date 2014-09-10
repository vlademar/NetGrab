using System.Net;
using System.Windows;

namespace NetGrab
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();
        }


        private void BtnGo_OnClick(object sender, RoutedEventArgs e)
        {
            var host = new TaskHost
            {
                Logger = new Logger("./log.txt"),
                Proxy = WebProxy.GetDefaultProxy()
            };

            host.Proxy.Credentials = new NetworkCredential("marchenko.v", "vLADEMAR1543");

            var task = new KnowyourmemeComLoaderTaskGroup
            {
                StartSuffix = "1380"
            };

            host.AddTask(task, 10);

            host.Run();
        }
    }
}
