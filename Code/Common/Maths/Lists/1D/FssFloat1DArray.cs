using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class FssFloat1DArray
{
    public float[] Data { get; private set; }

    public int Length => Data.Length;

    // --------------------------------------------------------------------------------------------
    // Constructors
    // --------------------------------------------------------------------------------------------

    public FssFloat1DArray(int newSize)
    {
        if ((newSize < 1) || (newSize > 100000))
            throw new ArgumentException($"Unexpected Create Size: {newSize}");

        Data = new float[newSize];
    }

    public FssFloat1DArray(float[] initialData)
    {
        Data = initialData ?? throw new ArgumentNullException(nameof(initialData));
    }

    public float this[int index]
    {
        get => Data[index];
        set => Data[index] = value;
    }

    // --------------------------------------------------------------------------------------------
    // Operations
    // --------------------------------------------------------------------------------------------

    // Return values regarding the list
    public float Average() { return Data.Average(); }
    public float Min()     { return Data.Min(); }
    public float Max()     { return Data.Max(); }
    public float Sum()     { return Data.Sum(); }
    public float SumAbs()  { return Data.Sum(x => Math.Abs(x)); }

    public FssFloat1DArray Multiply(FssFloat1DArray other)
    {
        if (other == null)          throw new ArgumentNullException(nameof(other));
        if (Length != other.Length) throw new ArgumentException("Arrays must be the same length", nameof(other));

        float[] result = new float[Length];
        for (int i = 0; i < Length; i++)
            result[i] = Data[i] * other[i];

        return new FssFloat1DArray(result);
    }

    // --------------------------------------------------------------------------------------------
    // Fraction Operations
    // --------------------------------------------------------------------------------------------

    // Returns the fractional position of an index in the array.
    public float FractionForIndex(int index)
    {
        return index / (float)(Length - 1);
    }

    public int IndexForFraction(float fraction)
    {
        fraction = Math.Clamp(fraction, 0, 1); // Clamp fraction between 0 and 1
        return (int)Math.Round(fraction * (Length - 1));
    }

    // Interpolates a value at a given fraction along the array.
    public float InterpolateAtFraction(float fraction)
    {
        int lowerIndex = (int)(fraction * (Length - 1));
        int upperIndex = Math.Min(lowerIndex + 1, Length - 1);
        float blend = (fraction * (Length - 1)) - lowerIndex;

        return Data[lowerIndex] * (1 - blend) + Data[upperIndex] * blend;
    }

    // --------------------------------------------------------------------------------------------
    // Resize Operations
    // --------------------------------------------------------------------------------------------

    public FssFloat1DArray Resize(int newSize)
    {
        // Check if the new size is within the acceptable range (1 to 100000)
        if ((newSize < 1) || (newSize > 100000))
            throw new ArgumentException($"Unexpected Resize Size: {newSize}");

        // Create a new float array of the new size
        float[] newData = new float[newSize];

        // If the original array has only one element, fill the new array with this element
        if (Length == 1)
        {
            float value = Data[0];
            for (int i = 0; i < newSize; i++)
            {
                newData[i] = value;
            }
        }
        else
        {
            // If the original array has more than one element, interpolate values from the original array to the new array
            for (int i = 0; i < newSize; i++)
            {
                // Calculate the fraction of the current index over the new size minus one
                float fraction = i / (float)(newSize - 1);

                // Use this fraction to interpolate a value from the original array
                newData[i] = InterpolateAtFraction(fraction);
            }
        }

        // Return a new FssFloat1DArray with the resized data
        return new FssFloat1DArray(newData);
    }

    // Refers to array index values, so counts from 0 to Length - 1.
    public FssFloat1DArray ArrayForIndexRange(int firstIndex, int lastIndex)
    {
        // Clamp indices to valid range
        firstIndex = FssValueUtils.Clamp(firstIndex, 0, Length - 1);
        lastIndex  = FssValueUtils.Clamp(lastIndex,  0, Length - 1);

        // Swap indices if they are in the wrong order (C# tuple operation for brevity)
        if (firstIndex > lastIndex)
            (firstIndex, lastIndex) = (lastIndex, firstIndex);

        // Create a new array with the correct size
        int newSize = lastIndex - firstIndex + 1;
        FssFloat1DArray newArray = new FssFloat1DArray(newSize);

        // Copy the data from the original array to the new array
        for (int i = 0; i < newSize; i++)
            newArray[i] = Data[firstIndex + i];

        return newArray;
    }

    // --------------------------------------------------------------------------------------------
    // MARK: ToString Operations
    // --------------------------------------------------------------------------------------------

    public string ToString(string format)
    {
        return string.Join(", ", Data.Select(x => x.ToString(format)));
    }

}
