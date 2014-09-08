using System.Windows;

namespace NetGrab
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }


        private void BtnGo_OnClick(object sender, RoutedEventArgs e)
        {
            var host = new TaskHost()
            {
                Logger = new Logger("./log.txt"),
                Proxy = null
            };

            var task = new KnowyourmemeComLoaderTaskGroup
            {
                StartSuffix = "100"
            };

            host.AddTask(task, 10);

            host.Run();
        }
    }
}
