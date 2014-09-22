using System;
using System.Net;
using System.Threading;
using System.Windows.Threading;

namespace NetGrab
{
    public abstract class LoaderBase : NotifyingObject, ILoader
    {
        private static readonly TimeSpan NextTaskRunDelay = new TimeSpan(0, 0, 0, 2);
        private static int NextFreeId = 1;
        private static int GetId()
        {
            return NextFreeId++;
        }

        protected readonly AsyncLoaderHelper AsyncLoaderHelper = new AsyncLoaderHelper();
        private readonly TimerManager NotifyFinishedTimer;

        private int id;
        private LoaderState _state;
        private string _description;

        public event EventHandler Finished;
        public ITaskHost TaskHost { get; set; }
        public WebProxy Proxy { get; set; }
        public ILoaderTaskGroup LoaderTaskGroup { get; set; }
        public ILogger Logger { get; set; }

        public LoaderState State
        {
            get { return _state; }
            protected set { SetValue(ref _state, value, "State"); }
        }

        public string Description
        {
            get { return _description; }
            protected set { SetValue(ref _description, value, "Description"); }
        }

        public bool HasNextTask
        {
            get { return LoaderTaskGroup.HasNextTask; }
        }

        public int LoaderId
        {
            get { return id; }
            private set { SetValue(ref id, value, "LoaderId"); }
        }

        protected LoaderBase()
        {
            LoaderId = GetId();
            NotifyFinishedTimer = new TimerManager(NextTaskRunDelay, OnFinishedInternal);
        }

        public void RunNext()
        {
            LoaderTaskGroup.ReinitLoader(this);
            State = LoaderState.Running;
            DoWork();
        }

        protected void OnError(string message)
        {
            NavigateToOnFinishedInternal(string.Format("{0} | {1}", this, message), LoaderState.Error);
        }
        protected void OnError(string formatString, params object[] args)
        {
            var msg = string.Format(formatString, args);
            OnError(msg);
        }

        protected void OnFinished(string message = "")
        {
            NavigateToOnFinishedInternal(string.Format("{0} | OK {1}", this, message), LoaderState.Finished);
        }
        protected void OnFinished(string formatString, params object[] args)
        {
            var msg = string.Format(formatString, args);
            OnFinished(msg);
        }

        private void NavigateToOnFinishedInternal(string message, LoaderState state)
        {
            Description = message;
            Logger.Add(string.Format("{0:D3} | {1}", LoaderId, message));
            State = state;

            NotifyFinishedTimer.Start();
        }

        private void OnFinishedInternal()
        {
            if (Finished != null)
                Finished(this, new EventArgs());
        }

        protected abstract void DoWork();
    }
}
