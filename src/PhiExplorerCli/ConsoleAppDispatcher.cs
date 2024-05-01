using Drastic.Services;

namespace PhiExplorerCli;

public class ConsoleAppDispatcher : IAppDispatcher
{
    public bool Dispatch(Action action)
    {
        action.Invoke();
        return true;
    }
}