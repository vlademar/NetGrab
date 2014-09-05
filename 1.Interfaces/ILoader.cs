using System;

namespace NetGrab
{
    interface ILoader
    {
        event EventHandler Finished;

        LoaderState State { get; }
        ITaskHost TaskHost { get; set; }
        ITaskGroup TaskGroup { get; set; }
        ILogger Logger { get; set; }
        bool HasNextTask { get; }

        void RunNext();
        ILoader Clone();
    }

    internal interface ILoader<T> : ILoader where T : ILoader<T>
    {
        ILoaderInitializer<T> Initializer { get; set; }
    }
}