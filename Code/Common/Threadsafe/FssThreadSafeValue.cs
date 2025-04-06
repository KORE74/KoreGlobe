using System;

public class FssThreadSafeValue<T> where T : class
{
    private T               _value;
    private readonly object _lock = new();

    public T Value // Mark the property as nullable
    {
        get
        {
            lock (_lock)
            {
                return _value;
            }
        }
        set
        {
            lock (_lock)
            {
                _value = value;
            }
        }
    }

    public FssThreadSafeValue(T initialValue)
    {
        _value = initialValue;
    }

    // Method to update the value atomically
    // Example:
    //    var threadSafeValue = new FssThreadSafeValue<int>(10);
    //    threadSafeValue.Update(value => value + 5);
    public void Update(Func<T, T> updateFunc)
    {
        lock (_lock)
        {
            _value = updateFunc(_value);
        }
    }
}
