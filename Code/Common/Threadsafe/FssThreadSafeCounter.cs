

using System;

class FssThreadSafeCounter
{
    private int             _counter = 0;
    private readonly object _lock = new object();

    public FssThreadSafeCounter(int initialValue = 0)
    {
        _counter = initialValue;
    }

    // Threadsafe simple set / get operations - no value checks

    public void SetValue(int value)
    {
        lock (_lock)
        {
            _counter = value;
        }
    }

    public int GetValue()
    {
        lock (_lock)
        {
            return _counter;
        }
    }

    // Threadsafe simple inc / dec operations - no value checks

    public int Increment()
    {
        lock (_lock)
        {
            return ++_counter;
        }
    }

    public int Decrement()
    {
        lock (_lock)
        {
            _counter--;
            return _counter;
        }
    }

    // Threadsafe decrement with check - returns true when a value was available to decrement.

    public bool DecrementAndCheck()
    {
        lock (_lock)
        {
            if (_counter <= 0)
                return false;

            _counter--;
            return true;
        }
    }
}

