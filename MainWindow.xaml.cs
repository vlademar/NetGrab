using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows;
using System.Windows.Threading;

namespace NetGrab
{
    class Task
    {
        public int ThreadId { get; set; }
        public string Suffix { get; set; }
    }

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, ITaskHost
    {
        private const int threadCount = 32;

        private INameGen nameGen;
        private Dispatcher dispatcher;
        private Func<int, string, bool> reportFunc;
        private SortedList<int, ILoader> loaders = new SortedList<int, ILoader>();

        private bool working = false;
        private bool abort = false;

        public MainWindow()
        {
            InitializeComponent();

            reportFunc = ReportPogressInternal;
            nameGen = new NameGen09();
            var logger = new Logger(".//grab.log");

            for (int i = 0; i < threadCount; i++)
            {
                var loader = new KnowyourmemeComSyncLoader();
                loader.Init(this, logger);
                loaders.Add(i, loader);
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (working)
            {
                btnGo.Content = "Aborting...";
                btnGo.IsEnabled = false;
                abort = true;
                working = false;
            }

            dispatcher = Dispatcher;

            nameGen.Init(tbStartName.Text);

            for (int i = 0; i < threadCount; i++)
            {
                lbLog.Items.Add("");
                RaiseNextJob(i);
            }

            working = true;
            btnGo.Content = "Abort";
        }

        public void RaiseNextJob(int threadId)
        {
            if (abort)
            {
                dispatcher.Invoke(reportFunc, threadId, "ABORTED");

                return;
            }

            var name = nameGen.NextName();


            ThreadPool.QueueUserWorkItem(LaunchSingleLoader, new Task { Suffix = name, ThreadId = threadId });

            dispatcher.Invoke(reportFunc, threadId, name);
        }

        private void LaunchSingleLoader(object state)
        {
            var task = state as Task;
            var loader = loaders[task.ThreadId];
            loader.DoWork(task);
        }

        public void ReportProgress(int threadId, string progress)
        {
            dispatcher.Invoke(reportFunc, threadId, progress);
        }

        private bool ReportPogressInternal(int threadId, string message)
        {
            lbLog.Items[threadId] = string.Format("{0:x2} - {1}", threadId, message);
            return true;
        }

    }
}
