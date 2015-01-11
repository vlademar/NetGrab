using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace NetGrab
{
    public class SpeedTest : NotifyingObject, ISpeedTest, INotifyPropertyChanged
    {
        private readonly List<KeyValuePair<DateTime, int>> _downloads = new List<KeyValuePair<DateTime, int>>();
        private string _speed;
        private TimerManager _timer;
        public string Speed
        {
            get
            {
                return this._speed;
            }
            private set
            {
                base.SetValue<string>(ref this._speed, value, "Speed");
            }
        }
        public SpeedTest()
        {
            this._timer = new TimerManager(new TimeSpan(0, 0, 0, 5), new Action(this.Update), true);
            this._timer.Start();
        }
        public void RegisterDownload(int bytesDownloaded)
        {
            this._downloads.Add(new KeyValuePair<DateTime, int>(DateTime.Now, bytesDownloaded));
        }
        public void Update()
        {
            DateTime t = DateTime.Now.AddMinutes(3.0);
            while (this._downloads.Count > 0)
            {
                if (this._downloads[0].Key < t)
                {
                    this._downloads.RemoveAt(0);
                }
            }
            int num = this._downloads.Sum((KeyValuePair<DateTime, int> pair) => pair.Value);
            int num2 = num / 3;
            if (num2 < 1024)
            {
                this.Speed = string.Format("{0} B/s", num2);
                return;
            }
            if (num2 < 1048576)
            {
                this.Speed = string.Format("{0} kB/s", num2 / 1024);
                return;
            }
            if (num2 < 1073741824)
            {
                this.Speed = string.Format("{0} mB/s", num2 / 1048576);
                return;
            }
            this.Speed = string.Format("{0} gB/s", num2 / 1073741824);
        }
    }
}