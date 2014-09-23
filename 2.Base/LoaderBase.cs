using System;
using System.Net;

namespace NetGrab
{
    public abstract class LoaderBase : NotifyingObject, ILoader
    {
        private static readonly TimeSpan NextTaskRunDelay = new TimeSpan(0, 0, 0, 2);
        private static int _nextFreeId = 1;
        private static int GetId()
        {
            return _nextFreeId++;
        }

        protected readonly AsyncLoaderHelper AsyncLoaderHelper = new AsyncLoaderHelper();
        private readonly TimerManager _notifyFinishedTimer;

        private int _id;
        private LoaderState _state;
        private string _description;
        private int _bytesDownloaded;

        public event LoaderFinishedDelegate Finished;
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
            get { return _id; }
            private set { SetValue(ref _id, value, "LoaderId"); }
        }

        protected LoaderBase()
        {
            LoaderId = GetId();
            _notifyFinishedTimer = new TimerManager(NextTaskRunDelay, OnFinishedInternal);
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

        protected void OnFinished(int bytesDownloaded, string message = "")
        {
            _bytesDownloaded = bytesDownloaded;
            NavigateToOnFinishedInternal(string.Format("{0} | OK {1}", this, message), LoaderState.Finished);
        }
        protected void OnFinished(int bytesDownloaded, string formatString, params object[] args)
        {
            var msg = string.Format(formatString, args);
            OnFinished(bytesDownloaded, msg);
        }

        private void NavigateToOnFinishedInternal(string message, LoaderState state)
        {
            Description = message;
            Logger.Add(string.Format("{0:D3} | {1}", LoaderId, message));
            State = state;

            _notifyFinishedTimer.Start();
        }

        private void OnFinishedInternal()
        {
            if (Finished != null)
                Finished(this, new LoaderFinishedEventArgs(_bytesDownloaded));
        }

        protected abstract void DoWork();
    }
}
