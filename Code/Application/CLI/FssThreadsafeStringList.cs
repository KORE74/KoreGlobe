
using System.Collections.Generic;
using System.Threading;

// FssThreadsafeStringList: A thread-safe queue to hold console input strings.

public class FssThreadsafeStringList
{
    private readonly LinkedList<string> stringList = new LinkedList<string>();
    private readonly object lockObject = new object();

    // Adds a new string to the queue
    public void AddString(string input)
    {
        lock (lockObject)
        {
            stringList.AddLast(input);
        }
    }

    // Checks if the queue is empty
    public bool IsEmpty()
    {
        lock (lockObject)
        {
            return stringList.Count == 0;
        }
    }

    // Retrieves and removes the first string from the queue
    public string RetrieveString()
    {
        lock (lockObject)
        {
            if (stringList.Count > 0)
            {
                string firstString = stringList.First.Value;
                stringList.RemoveFirst();
                return firstString;
            }
            return string.Empty;
        }
    }
}
