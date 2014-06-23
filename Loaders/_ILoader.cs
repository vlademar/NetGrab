namespace NetGrab
{
    internal interface ILoader
    {
        ILoader New();
        void Init(ITaskHost _taskHost, ILogger _logger, string _downloadPathBase);
        void DoWork(Task task);
    }
}