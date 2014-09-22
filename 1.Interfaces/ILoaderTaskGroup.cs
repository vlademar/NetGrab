using System.Net;

namespace NetGrab
{
    public interface ILoaderTaskGroup : IStateTracker
    {
        ILoader NewTaskLoader(ITaskHost taskHost, WebProxy proxy, ILogger logger);
        bool HasNextTask { get; }
        void ReinitLoader(ILoader loader);
    }
}
