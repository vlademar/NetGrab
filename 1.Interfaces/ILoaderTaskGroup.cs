using System.Net;

namespace NetGrab
{
    interface ILoaderTaskGroup
    {
        ILoader NewTaskLoader(ITaskHost taskHost, WebProxy proxy, ILogger logger);
        bool HasNextTask { get; }
        void ReinitLoader(ILoader loader);
    }
}
