using System.ComponentModel;
using System.Net;

namespace NetGrab
{
    public interface ITaskHost : INotifyPropertyChanged
    {
        bool Running { get; }

        void AddTask(ILoaderTaskGroup task, int parallelCount);
        void Run();
        void Stop();
    }
}