using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace NetGrab
{
    abstract class LoaderBase : ILoader
    {
        private static int NextFreeId = 1;
        private static int GetId()
        {
            return NextFreeId++;
        }

        protected static readonly AsyncLoaderHelper AsyncLoaderHelper = new AsyncLoaderHelper();

        private int id = GetId();

        public event EventHandler Finished;
        public ITaskHost TaskHost { get; set; }
        public IWebProxy Proxy { get; set; }
        public ILoaderTaskGroup LoaderTaskGroup { get; set; }
        public ILogger Logger { get; set; }
        public LoaderState State { get; protected set; }

        public bool HasNextTask
        {
            get { return LoaderTaskGroup.HasNextTask; }
        }

        public int LoaderId
        {
            get { return id; }
        }

        public void RunNext()
        {
            LoaderTaskGroup.ReinitLoader(this);
            State = LoaderState.Finished;
            DoWork();
        }

        protected void OnFinished()
        {
            State = LoaderState.Finished;
            if (Finished != null)
                Finished(this, new EventArgs());
        }

        protected abstract void DoWork();
    }
}
