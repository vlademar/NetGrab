using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using NetGrab.Annotations;
using NetGrab.Properties;

namespace NetGrab
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : INotifyPropertyChanged
    {
        private ITaskHost _taskHost = null;

        public ITaskHost TaskHost
        {
            get { return _taskHost; }
            private set
            {
                _taskHost = value;
                OnPropertyChanged("TaskHost");
            }
        }

        public MainWindow(ITaskHost taskHost)
        {
            InitializeComponent();
            TaskHost = taskHost;
            ListView.ItemsSource = TaskHost.Loaders;
        }

        private void BtnGo_OnClick(object sender, RoutedEventArgs e)
        {
            var t = TaskHost;
            TaskHost = t;

            var task = new KnowyourmemeComLoaderTaskGroup
            {
                StartSuffix = TextBox.Text
            };

            TaskHost.AddTask(task, Settings.Default.ThreadCount);
            TaskHost.Run();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
