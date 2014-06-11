namespace NetGrab
{
    internal interface ILoader
    {
        void Init(ITaskHost _taskHost, ILogger _logger);
        void DoWork(Task task);
    }
}