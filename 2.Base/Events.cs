using System;

namespace NetGrab
{
    public delegate void LoaderFinishedDelegate(object sender , LoaderFinishedEventArgs e);

    public class LoaderFinishedEventArgs : EventArgs
    {
        public int BytesDownloaded { get; private set; }

        public LoaderFinishedEventArgs(int _bytesDownloaded)
        {
            BytesDownloaded = _bytesDownloaded;
        }
    }
}
