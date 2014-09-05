using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace NetGrab.Interfaces
{
    class TaskHost : ITaskHost
    {
        private bool running = false;

        public ObservableCollection<ILoader> Loaders { get; private set; }

        public TaskHost()
        {
            Loaders = new ObservableCollection<ILoader>();
        }

        public void Run()
        {
            running = true;
            foreach (var loader in Loaders)
                if (loader.State == LoaderState.Idle)
                    loader.Run();
        }

        public void Stop()
        {
            running = false;
        }

        public void AddTask(ILoader loaderPrototype, int parallelCount)
        {
            for (int i = 0; i < parallelCount; i++)
            {
                var newLoader = loaderPrototype.Clone();
                newLoader.Finished += LoaderFinished;
                Loaders.Add(newLoader);
                if (running)
                    newLoader.Run();
            }
        }

        private void LoaderFinished(object sender, EventArgs eventArgs)
        {
            var loader = (ILoader)sender;
            if (!running || !loader.HasNextTask)
            {
                Loaders.Remove(loader);
                return;
            }

            Loaders.Move(Loaders.IndexOf(loader), Loaders.Count - 1);
            loader.Run();
        }
    }
}
