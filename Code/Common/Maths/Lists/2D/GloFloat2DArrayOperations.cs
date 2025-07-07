
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

// GloFloat2DArray: A class to represent a 2D float array, and a number of involved actions we can perform on it.
// Created predominantly to hold large lists of terrain elevation values, and subdivide that into subtiles.

public static class GloFloat2DArrayOperations
{
    public enum GridPlacement {Undefined, North, South, East, West};

    // - Function to return a new GloFloat2DArray, based on the input refGrid with its relevant edge lined up with adjacentGrid
    // - Note that two two grids may contain different numbers of points, so they will need to do interpolation.
    // - Note that two adjacent map tiles contain points that occupy the same geographic positions, so copying one edge over
    //   another works in this specific use case.

    public static GloFloat2DArray SmoothGridEdge(GloFloat2DArray refGrid, GloFloat2DArray adjacentGrid, GridPlacement adjacentPosition)
    {
        GloFloat2DArray.Edge refEdge      = GloFloat2DArray.Edge.Undefined;
        GloFloat2DArray.Edge adjacentEdge = GloFloat2DArray.Edge.Undefined;

        switch (adjacentPosition)
        {
            case GridPlacement.North: refEdge = GloFloat2DArray.Edge.Top;    adjacentEdge = GloFloat2DArray.Edge.Bottom; break;
            case GridPlacement.South: refEdge = GloFloat2DArray.Edge.Bottom; adjacentEdge = GloFloat2DArray.Edge.Top;    break;
            case GridPlacement.East:  refEdge = GloFloat2DArray.Edge.Right;  adjacentEdge = GloFloat2DArray.Edge.Left;   break;
            case GridPlacement.West:  refEdge = GloFloat2DArray.Edge.Left;   adjacentEdge = GloFloat2DArray.Edge.Right;  break;
        }

        GloFloat1DArray edgeRef      = refGrid.GetEdge(refEdge);
        GloFloat1DArray edgeAdjacent = adjacentGrid.GetEdge(adjacentEdge);

        // If the length of the adjacent edge doesn't line up, resize the array to match.
        if (edgeRef.Length != edgeAdjacent.Length)
            edgeAdjacent = edgeAdjacent.Resize(edgeAdjacent.Length);

        // Create the new array and setup the edge from the adjacent array
        GloFloat2DArray newRefGrid = new GloFloat2DArray(refGrid);
        newRefGrid.SetEdge(edgeAdjacent, refEdge);

        return newRefGrid;
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Value Ranges
    // --------------------------------------------------------------------------------------------

    public static GloFloatRange FindValueRange(GloFloat2DArray array)
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

        return new GloFloatRange(minValue, maxValue);
    }

    public static GloFloat2DArray NormalizeToRange(GloFloat2DArray array, GloFloatRange newRange)
    {
        GloFloatRange currentRange = FindValueRange(array);
        GloFloat2DArray normalizedArray = new GloFloat2DArray(array.Width, array.Height);

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

    // Usage: GloFloat2DArray croppedArray = GloFloat2DArrayOperations.CropToRange(array, new GloFloatRange(0f, 10000f));
    public static GloFloat2DArray CropToRange(GloFloat2DArray array, GloFloatRange newRange)
    {
        GloFloat2DArray croppedArray = new GloFloat2DArray(array.Width, array.Height);

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

