using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Net;

namespace NetGrab
{
    public interface ITaskHost : INotifyPropertyChanged
    {
        bool Running { get; }
        ObservableCollection<ILoader> Loaders { get; }
        ISpeedTest SpeedTest { get; set; }

        void AddTask(ILoaderTaskGroup task, int parallelCount);
        void Run();
        void Stop();
    }
}