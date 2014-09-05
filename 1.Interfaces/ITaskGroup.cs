namespace NetGrab
{
    interface ITaskGroup
    {
        ILoader NewTaskLoader();
        bool HasNextTask { get; }
        void ReinitLoader(ILoader loader );
    }
}
