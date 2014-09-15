using System.Windows;

namespace NetGrab
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        public ITaskHost TaskHost { get; private set; }

        public MainWindow(ITaskHost taskHost)
        {
            TaskHost = taskHost;
            InitializeComponent();
        }

        private void BtnGo_OnClick(object sender, RoutedEventArgs e)
        {
            var task = new KnowyourmemeComLoaderTaskGroup
            {
                StartSuffix = TextBox.Text
            };

            TaskHost.AddTask(task, 32);
            TaskHost.Run();
        }
    }
}
