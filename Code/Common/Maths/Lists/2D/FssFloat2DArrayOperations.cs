
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

// FssFloat2DArray: A class to represent a 2D float array, and a number of involved actions we can perform on it.
// Created predominantly to hold large lists of terrain elevation values, and subdivide that into subtiles.

public static class FssFloat2DArrayOperations
{
    public enum GridPlacement {Undefined, North, South, East, West};

    // - Function to return a new FssFloat2DArray, based on the input refGrid with its relevant edge lined up with adjacentGrid
    // - Note that two two grids may contain different numbers of points, so they will need to do interpolation.
    // - Note that two adjacent map tiles contain points that occupy the same geographic positions, so copying one edge over
    //   another works in this specific use case.

    public static FssFloat2DArray SmoothGridEdge(FssFloat2DArray refGrid, FssFloat2DArray adjacentGrid, GridPlacement adjacentPosition)
    {
        FssFloat2DArray.Edge refEdge      = FssFloat2DArray.Edge.Undefined;
        FssFloat2DArray.Edge adjacentEdge = FssFloat2DArray.Edge.Undefined;

        switch (adjacentPosition)
        {
            case GridPlacement.North: refEdge = FssFloat2DArray.Edge.Top;    adjacentEdge = FssFloat2DArray.Edge.Bottom; break;
            case GridPlacement.South: refEdge = FssFloat2DArray.Edge.Bottom; adjacentEdge = FssFloat2DArray.Edge.Top;    break;
            case GridPlacement.East:  refEdge = FssFloat2DArray.Edge.Right;  adjacentEdge = FssFloat2DArray.Edge.Left;   break;
            case GridPlacement.West:  refEdge = FssFloat2DArray.Edge.Left;   adjacentEdge = FssFloat2DArray.Edge.Right;  break;
        }

        FssFloat1DArray edgeRef      = refGrid.GetEdge(refEdge);
        FssFloat1DArray edgeAdjacent = adjacentGrid.GetEdge(adjacentEdge);

        // If the length of the adjacent edge doesn't line up, resize the array to match.
        if (edgeRef.Length != edgeAdjacent.Length)
            edgeAdjacent = edgeAdjacent.Resize(edgeAdjacent.Length);

        // Create the new array and setup the edge from the adjacent array
        FssFloat2DArray newRefGrid = new FssFloat2DArray(refGrid);
        newRefGrid.SetEdge(edgeAdjacent, refEdge);

        return newRefGrid;
    }

    // --------------------------------------------------------------------------------------------
    // Value Ranges
    // --------------------------------------------------------------------------------------------

    public static FssFloatRange FindValueRange(FssFloat2DArray array)
    {
        float minValue = float.MaxValue;
        float maxValue = float.MinValue;

        for (int i = 0; i < array.Width; i++)
        {
            for (int j = 0; j < array.Height; j++)
            {
                float val = array[i, j];
                if (val < minValue) minValue = val;
                if (val > maxValue) maxValue = val;
            }
        }

        return new FssFloatRange(minValue, maxValue);
    }

    public static FssFloat2DArray NormalizeToRange(FssFloat2DArray array, FssFloatRange newRange)
    {
        FssFloatRange currentRange = FindValueRange(array);
        FssFloat2DArray normalizedArray = new FssFloat2DArray(array.Width, array.Height);

        for (int i = 0; i < array.Width; i++)
        {
            for (int j = 0; j < array.Height; j++)
            {
                normalizedArray[i, j] = (((array[i, j] - currentRange.Min) / (currentRange.Max - currentRange.Min)) * (newRange.Max - newRange.Min)) + newRange.Min;
            }
        }

        return normalizedArray;
    }

    // Any value outside the range is clamped to the min or max value.

    // Usage: FssFloat2DArray croppedArray = FssFloat2DArrayOperations.CropToRange(array, new FssFloatRange(0f, 10000f));
    public static FssFloat2DArray CropToRange(FssFloat2DArray array, FssFloatRange newRange)
    {
        FssFloat2DArray croppedArray = new FssFloat2DArray(array.Width, array.Height);

        for (int i = 0; i < array.Width; i++)
        {
            for (int j = 0; j < array.Height; j++)
            {
                croppedArray[i, j] = Math.Clamp(array[i, j], newRange.Min, newRange.Max);
            }
        }

        return croppedArray;
    }

    // --------------------------------------------------------------------------------------------

}

