using System;
using System.Collections.Generic;
using System.Linq;

public static class FssFloat1DArrayOperations
{
    // --------------------------------------------------------------------------------------------
    // Value Ranges
    // --------------------------------------------------------------------------------------------

    public static FssFloatRange FindValueRange(FssFloat1DArray array)
    {
        return new FssFloatRange(array.Min(), array.Max());
    }

    public static FssFloat1DArray NormalizeToValueRange(FssFloat1DArray array, FssFloatRange newRange)
    {
        FssFloatRange currentRange = FindValueRange(array);
        FssFloat1DArray normalizedArray = new FssFloat1DArray(array.Length);

        for (int i = 0; i < array.Length; i++)
        {
            normalizedArray[i] = (((array[i] - currentRange.Min) / (currentRange.Max - currentRange.Min)) * (newRange.Max - newRange.Min)) + newRange.Min;
        }

        return normalizedArray;
    }

    public static FssFloat1DArray CropToValueRange(FssFloat1DArray array, FssFloatRange newRange)
    {
        FssFloat1DArray croppedArray = new FssFloat1DArray(array.Length);

        for (int i = 0; i < array.Length; i++)
        {
            croppedArray[i] = Math.Clamp(array[i], newRange.Min, newRange.Max);
        }

        return croppedArray;
    }

    public static FssFloat1DArray Softmax(FssFloat1DArray array)
    {
        FssFloat1DArray result = new FssFloat1DArray(array.Length);
        float max = array.Max();
        float scale = 0.0f;

        for (int i = 0; i < array.Length; i++)
        {
            result[i] = (float)Math.Exp(array[i] - max);
            scale += result[i];
        }

        for (int i = 0; i < array.Length; i++)
        {
            result[i] /= scale;
        }

        return result;
    }

    public static FssFloat1DArray CreateDifferenceList(FssFloat1DArray array)
    {
        float[] differences = new float[array.Length - 1];

        for (int i = 1; i < array.Length; i++)
            differences[i - 1] = Math.Abs(array[i] - array[i - 1]);

        return new FssFloat1DArray(differences);
    }

    // --------------------------------------------------------------------------------------------
    // Smoothing
    // --------------------------------------------------------------------------------------------

    public enum SmoothType { Undefined, MovingAverage5 };

    public static FssFloat1DArray Smooth(FssFloat1DArray array, SmoothType type)
    {
        switch (type)
        {
            case SmoothType.MovingAverage5:
                return SmoothMovingAverage5(array);
            default:
                throw new Exception($"Smooth type {type} not implemented.");
        }
    }

    private static FssFloat1DArray SmoothMovingAverage5(FssFloat1DArray array)
    {
        FssFloat1DArray smoothedArray = new FssFloat1DArray(array.Length);

        // loop across the whole array - the index we want to return
        for (int i = 0; i < array.Length; i++)
        {
            // Get the index range we want to average - doesn't matter if we're going out of
            //  bounds, the ArrayForIndexRange function will clamp the indices to the array bounds
            FssFloat1DArray range = array.ArrayForIndexRange(i - 2, i + 2);
            smoothedArray[i] = range.Average();
        }

        return smoothedArray;
    }
}

