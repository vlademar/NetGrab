namespace NetGrab
{
    interface ITaskHost
    {
        void ReportProgress(int threadId, string progress);
        void RaiseNextJob(int threadId);
    }
}