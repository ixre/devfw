namespace J6.DevFw.Framework.TaskBox
{
    public interface ITaskExecuteClient
    {
        void Execute(ITask task);
        string ClientName { get; }
    }
}