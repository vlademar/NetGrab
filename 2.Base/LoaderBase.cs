using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NetGrab
{
    abstract class LoaderBase : ILoader
    {
        public event EventHandler Finished;
        public ITaskHost TaskHost { get; set; }
        public ITaskGroup TaskGroup { get; set; }
        public ILogger Logger { get; set; }
        public LoaderState State { get; protected set; }

        public bool HasNextTask
        {
            get { return TaskGroup.HasNextTask; }
        }

        public void RunNext()
        {
            TaskGroup.ReinitLoader(this);
            DoWork();
        }

        public ILoader Clone()
        {
            return TaskGroup.NewTaskLoader();
        }

        protected void OnFinished()
        {
            if (Finished != null)
                Finished(this, new EventArgs());
        }

        protected abstract void DoWork();
    }
}
