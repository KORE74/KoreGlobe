
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

// Float2DArray: A class to represent a 2D float array, and a number of involved actions we can perform on it.
// Created predominantly to hold large lists of terrain elevation values, and subdivide that into subtiles.

public static class FssFloat2DArrayOperations
{
    public enum GridPlacement {Undefined, North, South, East, West};

    // - Function to return a new Float2DArray, based on the input refGrid with its relevant edge lined up with adjacentGrid
    // - Note that two two grids may contain different numbers of points, so they will need to do interpolation.
    // - Note that two adjacent map tiles contain points that occupy the same geographic positions, so copying one edge over 
    //   another works in this specific use case.

    public static Float2DArray SmoothGridEdge(Float2DArray refGrid, Float2DArray adjacentGrid, GridPlacement adjacentPosition)
    {
        Float2DArray.Edge refEdge      = Float2DArray.Edge.Undefined;
        Float2DArray.Edge adjacentEdge = Float2DArray.Edge.Undefined;

        switch (adjacentPosition)
        {
            case GridPlacement.North: refEdge = Float2DArray.Edge.Top;    adjacentEdge = Float2DArray.Edge.Bottom; break;
            case GridPlacement.South: refEdge = Float2DArray.Edge.Bottom; adjacentEdge = Float2DArray.Edge.Top;    break;
            case GridPlacement.East:  refEdge = Float2DArray.Edge.Right;  adjacentEdge = Float2DArray.Edge.Left;   break;
            case GridPlacement.West:  refEdge = Float2DArray.Edge.Left;   adjacentEdge = Float2DArray.Edge.Right;  break;
        }

        FssFloat1DArray edgeRef      = refGrid.GetEdge(refEdge);
        FssFloat1DArray edgeAdjacent = adjacentGrid.GetEdge(adjacentEdge);

        // If the length of the adjacent edge doesn't line up, resize the array to match.
        if (edgeRef.Length != edgeAdjacent.Length)
            edgeAdjacent = edgeAdjacent.Resize(edgeAdjacent.Length);
        
        // Create the new array and setup the edge from the adjacent array
        Float2DArray newRefGrid = new Float2DArray(refGrid);
        newRefGrid.SetEdge(edgeAdjacent, refEdge);

        return newRefGrid;
    }

    // --------------------------------------------------------------------------------------------
    // Value Ranges
    // --------------------------------------------------------------------------------------------

    public static FssFloatRange FindValueRange(Float2DArray array)
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

    public static Float2DArray NormalizeToRange(Float2DArray array, FssFloatRange newRange)
    {
        FssFloatRange currentRange = FindValueRange(array);
        Float2DArray normalizedArray = new Float2DArray(array.Width, array.Height);

        for (int i = 0; i < array.Width; i++)
        {
            for (int j = 0; j < array.Height; j++)
            {
                normalizedArray[i, j] = (((array[i, j] - currentRange.Min) / (currentRange.Max - currentRange.Min)) * (newRange.Max - newRange.Min)) + newRange.Min;
            }
        }

        return normalizedArray;
    }

    public static Float2DArray CropToRange(Float2DArray array, FssFloatRange newRange)
    {
        Float2DArray croppedArray = new Float2DArray(array.Width, array.Height);

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

