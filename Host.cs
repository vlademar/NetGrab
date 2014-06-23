using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Threading;
using NetGrab.Properties;

namespace NetGrab
{
    class Host : ITaskHost
    {
        private ILogger logger;
        private INameGen nameGen;
        private Dispatcher dispatcher;
        private List<ILoader> loaders = new List<ILoader>();
        private string lastLoadedFile;

        public HostState State { get; private set; }

        public Host()
        {
            nameGen = new NameGen09();
            logger = new Logger(Settings.Default.Log);
            var downloadPath = Settings.Default.DownloadPath;

            try
            {
                StreamReader sr = new StreamReader(Settings.Default.LastLoadedFile);
                lastLoadedFile = sr.ReadToEnd();
                sr.Close();
            }
            catch (Exception)
            {
                lastLoadedFile = Settings.Default.FirstDownloadingSuffix;
            }

            nameGen.Init(lastLoadedFile);

            for (int i = 0; i < Settings.Default.ThreadCount; i++)
            {
                var loader = new KnowyourmemeComSyncLoader();
                loader.Init(this, logger, downloadPath);
                loaders.Add(loader);
            }

            State = HostState.Idle;
        }

        public void Run()
        {
            if (State != HostState.Idle)
                throw new Exception();

            dispatcher = Dispatcher.CurrentDispatcher;

            nameGen.Init(lastLoadedFile);

            foreach (var loader in loaders)
            {
                RaiseNextJob(loader);
            }

            State = HostState.Running;
        }

        public void Stop()
        {
            if (State != HostState.Running)
                throw new Exception();

            State = HostState.Stopping;

            SaveLastLoadedFile();
        }

        public void RaiseNextJob(ILoader loader)
        {
            if (State == HostState.Stopping)
            {
                loaders.Remove(loader);

                if (loaders.Count == 0)
                {
                    State = HostState.Idle;
                }
            }

            lastLoadedFile = nameGen.NextName();

            if (nameGen.Id % 1000 == 0)
                SaveLastLoadedFile();


            ThreadPool.QueueUserWorkItem(LaunchSingleLoader, new Task { Suffix = lastLoadedFile, Loader = loader });
        }

        private void LaunchSingleLoader(object state)
        {
            var task = state as Task;

            logger.Add(task.Suffix);

            task.Loader.DoWork(task);
        }

        private void SaveLastLoadedFile()
        {
            StreamWriter sw = new StreamWriter(Settings.Default.LastLoadedFile, false);
            sw.Write(lastLoadedFile);
            sw.Close();
        }
    }

    internal enum HostState
    {
        Idle,
        Running,
        Stopping
    };
}
