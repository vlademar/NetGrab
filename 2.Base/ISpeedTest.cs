using System.ComponentModel;

namespace NetGrab
{
    public interface ISpeedTest : INotifyPropertyChanged
    {
        string Speed
        {
            get;
        }
        void RegisterDownload(int bytesDownloaded);
    }
}