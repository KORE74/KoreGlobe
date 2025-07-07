
using System.Collections.Concurrent;

// KoreThreadsafeStringList: A thread-safe queue to hold console input strings.

public class KoreThreadsafeStringList
{
    private readonly ConcurrentQueue<string> stringQueue = new ConcurrentQueue<string>();

    // Adds a new string to the queue
    public void AddString(string input)
    {
        stringQueue.Enqueue(input);
    }

    // Checks if the queue is empty
    public bool IsEmpty()
    {
        return stringQueue.IsEmpty;
    }

    // Retrieves and removes the first string from the queue
    public string RetrieveString()
    {
        return stringQueue.TryDequeue(out string? result) ? result : string.Empty;
    }

    // Optional: Get count (approximate in concurrent scenarios)
    public int Count => stringQueue.Count;
}
