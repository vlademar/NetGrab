using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Threading;

namespace NetGrab
{
    public class TimerManager : IDisposable
    {
        readonly DispatcherTimer _dispatcherTimer;
        public Action TimeElapsed;

        public TimerManager(TimeSpan timeSpan)
        {
            _dispatcherTimer = new DispatcherTimer { Interval = timeSpan };
            _dispatcherTimer.Tick += _dispatcherTimer_Tick;
        }

        public TimerManager(TimeSpan timeSpan, Action timeElapsed)
            : this(timeSpan)
        {
            TimeElapsed = timeElapsed;
        }

        void _dispatcherTimer_Tick(object sender, EventArgs e)
        {
            if (TimeElapsed != null)
                TimeElapsed();
            Stop();
        }

        public void Restart()
        {
            _dispatcherTimer.Stop();
            _dispatcherTimer.Start();
        }

        public void Start()
        {
            _dispatcherTimer.Start();
        }

        public void Stop()
        {
            _dispatcherTimer.Stop();
        }

        public void SetTimeSpan(TimeSpan timeSpan)
        {
            _dispatcherTimer.Interval = timeSpan;
            Restart();
        }

        public void Dispose()
        {
            Cleanup();
            GC.SuppressFinalize(this);
        }

        ~TimerManager()
        {
            Cleanup();
        }

        private bool _disposed;
        void Cleanup()
        {
            if (!_disposed)
            {
                if (_dispatcherTimer.IsEnabled)
                    _dispatcherTimer.Stop();
                _dispatcherTimer.Tick -= _dispatcherTimer_Tick;
            }
            _disposed = true;
        }
    }
}
