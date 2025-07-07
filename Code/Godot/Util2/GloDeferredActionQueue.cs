
using System.Collections.Concurrent;
using System.Threading.Tasks;
using Godot;

public partial class GloDeferredActionQueue : Node
{
    private static ConcurrentQueue<GloDeferredAction> actionQueue = new ConcurrentQueue<GloDeferredAction>();

    private bool isProcessing    = false;
    private int  actionsPerFrame = 10; // Tune for performance

    public static void Enqueue(GloDeferredAction action)
    {
        actionQueue.Enqueue(action);
    }

    public override void _Process(double delta)
    {
        if (!isProcessing && !actionQueue.IsEmpty)
            ProcessQueue();
    }

    private async void ProcessQueue()
    {
        isProcessing = true;

        while (!actionQueue.IsEmpty)
        {
            int count = Mathf.Min(actionsPerFrame, actionQueue.Count);

            for (int i = 0; i < count; i++)
            {
                if (actionQueue.TryDequeue(out GloDeferredAction action))
                {
                    CallDeferred(nameof(ExecuteAction), action);
                }
            }

            await ToSignal(GetTree(), "process_frame"); // Throttle execution
        }

        isProcessing = false;
    }

    private void ExecuteAction(GloDeferredAction action)
    {
        action.ExecuteDeferredAction();
    }
}