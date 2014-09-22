﻿using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using NetGrab.Properties;

namespace NetGrab
{
    public class TaskHost : NotifyingObject, ITaskHost
    {
        private bool _running;
        private ObservableCollection<ILoader> _loaders;

        private int iterationsCount = 0;
        private static readonly object SyncLock = new object();

        public ILogger Logger { get; set; }
        public WebProxy Proxy { get; set; }

        public bool Running
        {
            get { return _running; }
            private set { SetValue(ref _running, value, "Running"); }
        }

        public ObservableCollection<ILoader> Loaders
        {
            get { return _loaders; }
            private set { SetValue(ref _loaders, value, "Loaders"); }
        }

        public TaskHost()
        {
            Loaders = new ObservableCollectionEx<ILoader>();
        }

        public void Run()
        {
            Running = true;
            var idleLoaders = Loaders.Where(l => l.State == LoaderState.Idle || l.State == LoaderState.Finished);
            foreach (var loader in idleLoaders)
                if (loader.HasNextTask)
                    loader.RunNext();
        }

        public void Stop()
        {
            Running = false;
        }

        public void AddTask(ILoaderTaskGroup task, int parallelCount)
        {
            for (var i = 0; i < parallelCount; i++)
            {
                var newLoader = task.NewTaskLoader(this, Proxy, Logger);
                newLoader.Finished += LoaderFinished;
                Loaders.Add(newLoader);
                if (Running)
                    newLoader.RunNext();
            }
        }

        private void LoaderFinished(object sender, EventArgs eventArgs)
        {
            var loader = (ILoader)sender;
            if (!Running || !loader.HasNextTask)
                return;

            loader.RunNext();

            if (iterationsCount > 20)
                lock (SyncLock)
                {
                    iterationsCount = 0;
                    File.WriteAllText(Settings.Default.LastStateFile, loader.LoaderTaskGroup.GetState(), Encoding.Default);
                }

            iterationsCount++;
        }
    }
}
