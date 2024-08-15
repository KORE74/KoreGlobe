using System;

// A class that provides a simple pseudo-random number generator using a looped list of pre-generated random numbers.
// It generates a list of random float numbers on creation and provides a GetNext() function to return the next number in the list.
// The loop index is stored and will wrap around upon reaching the end.

public struct FssRandomLoopList
{
    private readonly float[] _randomList;
    private int _index;

    public FssRandomLoopList(int size, float minVal, float maxVal)
    {
        // Guarantee the min max order
        (minVal, maxVal) = minVal < maxVal ? (minVal, maxVal) : (maxVal, minVal);

        _randomList = new float[size];
        _index = 0;

        Random random = new Random();
        for (int i = 0; i < size; i++)
            _randomList[i] = (float)(random.NextDouble() * (maxVal - minVal) + minVal);
    }

    public float GetNext()
    {
        float result = _randomList[_index];
        _index = (_index + 1) % _randomList.Length;
        return result;
    }
}
