using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;

namespace NetGrab
{
    class TaskHost : ITaskHost
    {
        public bool Running { get; private set; }
        public ILogger Logger { get; set; }
        public IWebProxy Proxy { get; set; }

        public ObservableCollection<ILoader> Loaders { get; private set; }

        public TaskHost()
        {
            Loaders = new ObservableCollection<ILoader>();
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
            {
                Loaders.Remove(loader);
                return;
            }

            lock (this)
            {
                Loaders.Move(Loaders.IndexOf(loader), Loaders.Count - 1);
            }

            loader.RunNext();
        }
    }
}
