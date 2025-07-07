using System;
using System.Collections.Generic;
using System.Linq;

public static partial class GloFloat1DArrayOperations
{
    // --------------------------------------------------------------------------------------------
    // MARK: Value Ranges
    // --------------------------------------------------------------------------------------------

    public static GloFloatRange FindValueRange(GloFloat1DArray array)
    {
        return new GloFloatRange(array.Min(), array.Max());
    }

    public static GloFloat1DArray NormalizeToValueRange(GloFloat1DArray array, GloFloatRange newRange)
    {
        GloFloatRange currentRange = FindValueRange(array);
        GloFloat1DArray normalizedArray = new GloFloat1DArray(array.Length);

        for (int i = 0; i < array.Length; i++)
        {
            normalizedArray[i] = (((array[i] - currentRange.Min) / (currentRange.Max - currentRange.Min)) * (newRange.Max - newRange.Min)) + newRange.Min;
        }

        return normalizedArray;
    }

    public static GloFloat1DArray CropToValueRange(GloFloat1DArray array, GloFloatRange newRange)
    {
        GloFloat1DArray croppedArray = new GloFloat1DArray(array.Length);

        for (int i = 0; i < array.Length; i++)
        {
            croppedArray[i] = Math.Clamp(array[i], newRange.Min, newRange.Max);
        }

        return croppedArray;
    }

    public static GloFloat1DArray Softmax(GloFloat1DArray array)
    {
        GloFloat1DArray result = new GloFloat1DArray(array.Length);
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

    public static GloFloat1DArray CreateDifferenceList(GloFloat1DArray array)
    {
        float[] differences = new float[array.Length - 1];

        for (int i = 1; i < array.Length; i++)
            differences[i - 1] = Math.Abs(array[i] - array[i - 1]);

        return new GloFloat1DArray(differences);
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Nearest
    // --------------------------------------------------------------------------------------------

    // Usage: float nearest = GloFloat1DArrayOperations.NearestValue(array, value);

    public static float NearestValue(GloFloat1DArray array, float value)
    {
        float nearest = array[0];
        float nearestDist = Math.Abs(array[0] - value);

        for (int i = 1; i < array.Length; i++)
        {
            float dist = Math.Abs(array[i] - value);
            if (dist < nearestDist)
            {
                nearest = array[i];
                nearestDist = dist;
            }
        }

        return nearest;
    }

    // --------------------------------------------------------------------------------------------

    public static int NearestValueIndex(GloFloat1DArray array, float value)
    {
        int nearestIndex = 0;
        float nearestDist = Math.Abs(array[0] - value);

        for (int i = 1; i < array.Length; i++)
        {
            float dist = Math.Abs(array[i] - value);
            if (dist < nearestDist)
            {
                nearestIndex = i;
                nearestDist = dist;
            }
        }

        return nearestIndex;
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Range List
    // --------------------------------------------------------------------------------------------

    // Function to pre-create the values a loop will iterate across, to clean up the presentation of a loop
    // Usage: GloFloat1DArray loopLats = GloFloat1DArrayOperations.ListForRange(minLat, maxLat, 100);

    public static GloFloat1DArray ListForRange(float minVal, float maxVal, int numEntries)
    {
        if (numEntries < 2)
            throw new ArgumentException("Too Few Entries");

        GloFloat1DArray returnList = new(numEntries);

        float inc = (maxVal - minVal) / (float)(numEntries - 1);

        for (int i = 0; i < numEntries; i++)
        {
            returnList[i] = minVal + inc * (float)i;
        }

        return returnList;
    }

    public static GloFloat1DArray ListForRange(GloFloatRange range, int numEntries) => ListForRange(range.Min, range.Max, numEntries);

    // --------------------------------------------------------------------------------------------
    // MARK: Smoothing
    // --------------------------------------------------------------------------------------------

    public enum SmoothType { Undefined, MovingAverage5 };

    public static GloFloat1DArray Smooth(GloFloat1DArray array, SmoothType type)
    {
        switch (type)
        {
            case SmoothType.MovingAverage5:
                return SmoothMovingAverage5(array);
            default:
                throw new Exception($"Smooth type {type} not implemented.");
        }
    }

    private static GloFloat1DArray SmoothMovingAverage5(GloFloat1DArray array)
    {
        GloFloat1DArray smoothedArray = new GloFloat1DArray(array.Length);

        // loop across the whole array - the index we want to return
        for (int i = 0; i < array.Length; i++)
        {
            // *NOTE*: ArrayForIndexRange function will clamp the indices to the array bounds,
            // accomodating the out of bounds cases.
            GloFloat1DArray range = array.ArrayForIndexRange(i - 2, i + 2);
            smoothedArray[i] = range.Average();
        }

        return smoothedArray;
    }


    // --------------------------------------------------------------------------------------------
    // MARK Ease InOut
    // --------------------------------------------------------------------------------------------

    // function to output a list of numbers that ease in and out
    // Usage: GloFloat1DArray easeInOut = GloFloat1DArrayOperations.EaseInOut(0.0f, 1.0f, 100);
    public static GloFloat1DArray EaseInOut(float start, float end, int numEntries)
    {
        GloFloat1DArray easeInOut = new(numEntries);

        for (int i = 0; i < numEntries; i++)
        {
            float t = (float)i / (numEntries - 1);
            t = t * t * (3.0f - 2.0f * t); // Smoothstep function
            easeInOut[i] = start + (end - start) * t;
        }

        return easeInOut;
    }
}

