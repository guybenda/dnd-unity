
using System;
using System.Collections.Concurrent;

// This is used to execute actions inside the main thread, since Unity allows some operations only in the main thread.
public class SyncExecutor
{
    readonly ConcurrentQueue<Action> actions = new();

    public SyncExecutor()
    {
    }

    public void Enqueue(Action action)
    {
        actions.Enqueue(action);
    }

    public void Execute(int limit = -1)
    {
        while (actions.TryDequeue(out var action))
        {
            action();
            if (--limit == 0) break;
        }
    }
}