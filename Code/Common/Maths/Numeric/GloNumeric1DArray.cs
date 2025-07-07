using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

public class GloNumeric1DArray<T> where T : struct, INumber<T>
{
    public T[] Data { get; private set; }

    public int Length => Data.Length;

    public enum ListDirection { Forward, Reverse };

    // --------------------------------------------------------------------------------------------
    // Constructors
    // --------------------------------------------------------------------------------------------

    public GloNumeric1DArray(int newSize)
    {
        if (newSize < 1 || newSize > 100000)
            throw new ArgumentException($"Unexpected Create Size: {newSize}");

        Data = new T[newSize];
    }

    public GloNumeric1DArray(T[] initialData)
    {
        Data = initialData ?? throw new ArgumentNullException(nameof(initialData));
    }

    public T this[int index]
    {
        get => Data[index];
        set => Data[index] = value;
    }

    public GloNumeric1DArray(GloNumericRange<T> valueRange, int listSize, ListDirection direction = ListDirection.Forward)
    {
        Data = new T[listSize];

        if (direction == ListDirection.Forward)
        {
            T inc = valueRange.IncrementForSize(listSize);
            for (int i = 0; i < listSize; i++)
                Data[i] = valueRange.Min + inc * T.CreateChecked(i);
        }
        else
        {
            T inc = valueRange.IncrementForSize(listSize);

            for (int i = 0; i < listSize; i++)
            {
                // When creating in reverse, we flip the destination index we write to
                int destinationIndex = listSize - (i + 1);
                Data[destinationIndex] = valueRange.Min + inc * T.CreateChecked(i);
            }

        }
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Operations
    // --------------------------------------------------------------------------------------------

    // Return values regarding the list
    public T Average() { return Sum() / T.CreateChecked(Length); }
    public T Min()     { return Data.Min(); }
    public T Max()     { return Data.Max(); }
    public T Sum()     { return Data.Aggregate(T.Zero, (current, value) => current + value); }
    public T SumAbs()  { return Data.Aggregate(T.Zero, (current, value) => current + T.Abs(value)); }

    public GloNumeric1DArray<T> Multiply(GloNumeric1DArray<T> other)
    {
        if (other == null) throw new ArgumentNullException(nameof(other));
        if (Length != other.Length) throw new ArgumentException("Arrays must be the same length", nameof(other));

        T[] result = new T[Length];
        for (int i = 0; i < Length; i++)
            result[i] = Data[i] * other[i];

        return new GloNumeric1DArray<T>(result);
    }

    // --------------------------------------------------------------------------------------------
    // MARK: IEnumerable<T> Implementation
    // --------------------------------------------------------------------------------------------

    // public IEnumerator<T> GetEnumerator()
    // {
    //     return ((IEnumerable<T>)Data).GetEnumerator();
    // }

    // IEnumerator IEnumerable.GetEnumerator()
    // {
    //     return Data.GetEnumerator();
    // }

    // --------------------------------------------------------------------------------------------
    // MARK: Fraction Operations
    // --------------------------------------------------------------------------------------------

    public T FractionForIndex(int index)
    {
        return T.CreateChecked(index) / T.CreateChecked(Length - 1);
    }

    public int IndexForFraction(T fraction)
    {
        fraction = T.Clamp(fraction, T.Zero, T.One);
        return (int)Math.Round(double.Parse((fraction * T.CreateChecked(Length - 1)).ToString()));
    }

    public T InterpolateAtFraction(T fraction)
    {
        double fractionDouble = double.CreateChecked(fraction);
        int    lowerIndex     = (int)(fractionDouble * (Length - 1));
        int    upperIndex     = Math.Min(lowerIndex + 1, Length - 1);
        T      blend          = T.CreateChecked(fractionDouble * (Length - 1) - lowerIndex);

        return Data[lowerIndex] * (T.One - blend) + Data[upperIndex] * blend;
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Array Manipulation Operations
    // --------------------------------------------------------------------------------------------

    public GloNumeric1DArray<T> InterpolatedResize(int newSize)
    {
        if (newSize < 1 || newSize > 100000)
            throw new ArgumentException($"Unexpected Resize Size: {newSize}");

        T[] newData = new T[newSize];

        if (Length == 1)
        {
            T value = Data[0];
            for (int i = 0; i < newSize; i++)
                newData[i] = value;
        }
        else
        {
            for (int i = 0; i < newSize; i++)
            {
                T fraction = T.CreateChecked(i) / T.CreateChecked(newSize - 1);
                newData[i] = InterpolateAtFraction(fraction);
            }
        }

        return new GloNumeric1DArray<T>(newData);
    }

    public GloNumeric1DArray<T> ArrayForIndexRange(int firstIndex, int lastIndex)
    {
        // Sanitize the input indices
        firstIndex = Math.Clamp(firstIndex, 0, Length - 1);
        lastIndex  = Math.Clamp(lastIndex,  0, Length - 1);
        if (firstIndex > lastIndex)
            (firstIndex, lastIndex) = (lastIndex, firstIndex);

        // Create the new size, which from prior checks, is guaranteed to 0 -> Length-1
        int newSize = lastIndex - firstIndex + 1;
        GloNumeric1DArray<T> newArray = new GloNumeric1DArray<T>(newSize);

        // Copy the data to the new array
        for (int i = 0; i < newSize; i++)
            newArray[i] = Data[firstIndex + i];

        return newArray;
    }

    public GloNumeric1DArray<T> Reverse()
    {
        T[] reversedData = new T[Length];
        for (int i = 0; i < Length; i++)
            reversedData[i] = Data[Length - 1 - i];

        return new GloNumeric1DArray<T>(reversedData);
    }

    // --------------------------------------------------------------------------------------------
    // MARK: String
    // --------------------------------------------------------------------------------------------

    public string ToString(string format)
    {
        return string.Join(", ", Data.Select(x => x.ToString()));
    }
}
