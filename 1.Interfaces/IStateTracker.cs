namespace NetGrab
{
    public interface IStateTracker
    {
        string GetState();
        void SetState(string state);
    }
}
