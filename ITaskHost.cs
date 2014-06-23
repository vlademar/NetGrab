namespace NetGrab
{
    interface ITaskHost
    {
        void RaiseNextJob(ILoader loader);
    }
}