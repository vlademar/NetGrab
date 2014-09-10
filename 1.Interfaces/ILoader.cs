using System;
using System.Net;

namespace NetGrab
{
    interface ILoader
    {
        event EventHandler Finished;

        LoaderState State { get; }
        ITaskHost TaskHost { get; set; }
        WebProxy Proxy { get; set; }
        ILoaderTaskGroup LoaderTaskGroup { get; set; }
        ILogger Logger { get; set; }
        bool HasNextTask { get; }
        int LoaderId { get; }

        void RunNext();
    }
}