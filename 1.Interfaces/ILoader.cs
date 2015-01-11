using System.ComponentModel;
using System.Net;

namespace NetGrab
{
    public interface ILoader : INotifyPropertyChanged
    {
        event LoaderFinishedDelegate Finished;

        LoaderState State { get; }
        ITaskHost TaskHost { get; set; }
        WebProxy Proxy { get; set; }
        ILoaderTaskGroup LoaderTaskGroup { get; set; }
        ILogger Logger { get; set; }
        bool HasNextTask { get; }
        int LoaderId { get; }
        string Description { get; }

        void RunNext();
    }
}