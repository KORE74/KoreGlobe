
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

// GloFloat2DArray: A class to represent a 2D float array, and a number of involved actions we can perform on it.
// Created predominantly to hold large lists of terrain elevation values, and subdivide that into subtiles.

public class GloFloat2DArray
{
    public enum Edge {Undefined, Top, Bottom, Left, Right};

    private float[,] Data;
    public int Width { get; }
    public int Height { get; }
    public int Count => Width * Height;

    // Validity flag for contents, set after a successful file load operation, or mass-set operation.
    // Cleared by any init operation.
    public bool Populated { get; set; }

    // --------------------------------------------------------------------------------------------
    // MARK: Constructors
    // --------------------------------------------------------------------------------------------

    // Gridsize = number of points on any size of the square grid, say 20.
    // Will be indexed by 0->19 as any normal array.
    public GloFloat2DArray(int inSizeX, int inSizeY)
    {
        Width     = inSizeX;
        Height    = inSizeY;
        Data      = new float[Width, Height];
        Populated = false;
    }

    public GloFloat2DArray(GloFloat2DArray inArr)
    {
        Width     = inArr.Width;
        Height    = inArr.Height;
        Data      = new float[Width, Height];
        Populated = inArr.Populated;

        for (int i = 0; i < Width; i++)
        {
            for (int j = 0; j < Height; j++)
            {
                Data[i, j] = inArr[i, j];
            }
        }
    }

    public GloFloat2DArray()
    {
        Width     = 10;
        Height    = 10;
        Data      = new float[Width, Height];
        Populated = false;
    }

    public GloFloat2DArray(int inSizeX, int inSizeY, List<double> inList)
    {
        Width     = inSizeX;
        Height    = inSizeY;
        Data      = new float[Width, Height];
        Populated = false;

        if (inList.Count != Width * Height)
            throw new ArgumentException("List size must match the size of the array.", nameof(inList));

        int index = 0;
        for (int y = 0; y < Height; y++)
        {
            for (int x = 0; x < Width; x++)
            {
                Data[x, y] = (float)inList[index];
                index++;
            }
        }
    }

    // --------------------------------------------------------------------------------------------
    // Attributes
    // --------------------------------------------------------------------------------------------

    public string sizeStr()
    {
        return $"({Width}, {Height})";
    }

    // --------------------------------------------------------------------------------------------
    // Single set/get functions
    // --------------------------------------------------------------------------------------------

    // Function to allow the array to be indexed by [x,y]
    public float this[int x, int y]
    {
        get => Data[x, y];
        set => Data[x, y] = value;
    }

    public void SetFractionalValue(float fractionx, float fractiony, float value, double blendFraction = 1.0)
    {
        // If the point has N values in an axis, and the fraction places the point directly between two points, then what action is required?
        // needs further consideration.

        // Calculate indices
        int x = GloValueUtils.Clamp((int)(fractionx * (Data.GetLength(0) - 1)), 0, Data.GetLength(0) - 1);
        int y = GloValueUtils.Clamp((int)(fractiony * (Data.GetLength(1) - 1)), 0, Data.GetLength(1) - 1);

        // Calculate fractions for blending
        double existingValue = Data[x, y] * (1.0 - blendFraction);
        double newValue      = value * blendFraction;
        Data[x, y] = (float)(existingValue + newValue);
    }

    // --------------------------------------------------------------------------------------------
    // MARK: zero/init functions
    // --------------------------------------------------------------------------------------------

    public void InitZero()
    {
        SetAllVals(0f);
        Populated = false;
    }

    public bool IsZero()
    {
        for (int y = 0; y < Height; y++)
        {
            for (int x = 0; x < Width; x++)
            {
                if (!GloValueUtils.IsZero(Data[x, y]))
                    return false;
            }
        }
        return true;
        //return Data.Cast<float>().All(value => Math.Abs(value) <= GloConsts.ArbitraryMinDouble);
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Range Operations
    // --------------------------------------------------------------------------------------------

    public float MinVal()
    {
        float minVal = float.MaxValue;
        for (int y = 0; y < Height; y++)
        {
            for (int x = 0; x < Width; x++)
            {
                minVal = Math.Min(minVal, Data[x, y]);
            }
        }
        return minVal;
    }

    public float MaxVal()
    {
        float maxVal = float.MinValue;
        for (int y = 0; y < Height; y++)
        {
            for (int x = 0; x < Width; x++)
            {
                maxVal = Math.Max(maxVal, Data[x, y]);
            }
        }
        return maxVal;
    }

    public float ValRange()
    {
        return MaxVal() - MinVal();
    }

    // Get the range of values in the grid, get the range of new values, and scale the grid to fit the new range.

    public GloFloat2DArray ScaleToRange(float newMin, float newMax)
    {
        float oldMin = MinVal();
        float oldMax = MaxVal();
        float oldRange = oldMax - oldMin;
        float newRange = newMax - newMin;

        GloFloat2DArray scaledArray = new GloFloat2DArray(Width, Height);

        for (int y = 0; y < Height; y++)
        {
            for (int x = 0; x < Width; x++)
            {
                float oldVal = Data[x, y];
                float newVal = ((oldVal - oldMin) / oldRange) * newRange + newMin;
                scaledArray[x, y] = newVal;
            }
        }
        return scaledArray;
    }

    public GloFloat2DArray ScaleToMax(float newMax)
    {
        float oldMax = MaxVal();
        GloFloat2DArray scaledArray = new GloFloat2DArray(Width, Height);

        for (int y = 0; y < Height; y++)
        {
            for (int x = 0; x < Width; x++)
            {
                float oldVal = Data[x, y];
                float newVal = (oldVal / oldMax) * newMax;
                scaledArray[x, y] = newVal;
            }
        }
        return scaledArray;
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Multi-assignment
    // --------------------------------------------------------------------------------------------

    public void SetRow(int row, float value)
    {
        for (int x = 0; x < Width; x++)
            Data[x, row] = value;
    }

    public void SetCol(int column, float value)
    {
        for (int y = 0; y < Height; y++)
            Data[column, y] = value;
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Flip Grid
    // --------------------------------------------------------------------------------------------

    public static GloFloat2DArray FlipYAxis(GloFloat2DArray array)
    {
        GloFloat2DArray flippedArray = new GloFloat2DArray(array.Width, array.Height);

        for (int i = 0; i < array.Width; i++)
        {
            for (int j = 0; j < array.Height; j++)
            {
                flippedArray[i, j] = array[i, array.Height - 1 - j];
            }
        }
        return flippedArray;
    }

    public static GloFloat2DArray FlipXAxis(GloFloat2DArray array)
    {
        GloFloat2DArray flippedArray = new GloFloat2DArray(array.Width, array.Height);

        for (int i = 0; i < array.Width; i++)
        {
            for (int j = 0; j < array.Height; j++)
            {
                flippedArray[i, j] = array[array.Width - 1 - i, j];
            }
        }
        return flippedArray;
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Multi set/get functions
    // --------------------------------------------------------------------------------------------

    public void SetAllVals(float val)
    {
        for (int y = 0; y < Height; y++)
            for (int x = 0; x < Width; x++)
                Data[x, y] = val;
        Populated = true;
    }

    public void SetRandomVals(float minVal, float maxVal)
    {
        System.Random random = new System.Random();

        for (int y = 0; y < Height; y++)
            for (int x = 0; x < Width; x++)
                Data[x, y] = (float)GloValueUtils.RandomInRange(minVal, maxVal);
        Populated = true;
    }

    public void SetTestPattern()
    {
        for (int y = 0; y < Height; y++)
            for (int x = 0; x < Width; x++)
                Data[x, y] = (float)((x+1)*(y+1));
        Populated = true;
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Patch functions
    // --------------------------------------------------------------------------------------------

    // Assign a new set of values into a larger parent, defining a bottom right starting pos.
    public bool SetPatch(GloFloat2DArray newPatch, int blX, int blY)
    {
        // Check the list will fit, or return false.
        int trX = blX + (newPatch.Width - 1);
        int trY = blY + (newPatch.Height - 1);
        if ((trX > Width) || (trY > Height))
            return false;

        int destX = 0;
        int destY = 0;
        for (int y = 0; y < newPatch.Height; y++)
        {
            destY = blY + y;
            for (int x = 0; x < newPatch.Width; x++)
            {
                destX = blX + x;
                Data[destX, destY] = newPatch[x, y];
            }
        }
        return true;
    }

    public bool GetPatch(int blX, int blY, int patchWidth, int patchHeight, out GloFloat2DArray outPatch)
    {
        // Create the required out value
        outPatch = new GloFloat2DArray(patchWidth, patchHeight);

        // check the size will fit.
        int trX = blX + (patchWidth - 1);
        int trY = blY + (patchHeight - 1);
        if ((trX > Width) || (trY > Height))
            return false;

        for (int y = 0; y < patchHeight; y++)
        {
            int srcY = blY + y;
            for (int x = 0; x < patchWidth; x++)
            {
                int srcX = blX + x;
                outPatch[x, y] = Data[srcX, srcY];
            }
        }
        return true;
    }



    // --------------------------------------------------------------------------------------------
    // MARK: Edge functions
    // --------------------------------------------------------------------------------------------

    // array.GetEdge(GloFloat2DArray.Edge.Top)

    public GloFloat1DArray GetEdge(Edge e)
    {
        GloFloat1DArray edgeArray;
        switch (e)
        {
            case Edge.Top:
                edgeArray = new GloFloat1DArray(Width);
                for (int x = 0; x < Width; x++)
                    edgeArray[x] = Data[x, 0];
                break;

            case Edge.Bottom:
                edgeArray = new GloFloat1DArray(Width);
                for (int x = 0; x < Width; x++)
                    edgeArray[x] = Data[x, Height - 1];
                break;

            case Edge.Left:
                edgeArray = new GloFloat1DArray(Height);
                for (int y = 0; y < Height; y++)
                    edgeArray[y] = Data[0, y];
                break;

            case Edge.Right:
                edgeArray = new GloFloat1DArray(Height);
                for (int y = 0; y < Height; y++)
                    edgeArray[y] = Data[Width - 1, y];
                break;

            default:
                throw new ArgumentOutOfRangeException(nameof(e), "Invalid edge specified");
        }
        return edgeArray;
    }

    public void SetEdge(GloFloat1DArray edgeArray, Edge e)
    {
        switch (e)
        {
            case Edge.Top:
                if (edgeArray.Length != Width)
                    throw new ArgumentException("Length of edgeArray must be equal to Width of the array.", nameof(edgeArray));

                for (int x = 0; x < Width; x++)
                    Data[x, 0] = edgeArray[x];
                break;

            case Edge.Bottom:
                if (edgeArray.Length != Width)
                    throw new ArgumentException("Length of edgeArray must be equal to Width of the array.", nameof(edgeArray));

                for (int x = 0; x < Width; x++)
                    Data[x, Height - 1] = edgeArray[x];
                break;

            case Edge.Left:
                if (edgeArray.Length != Height)
                    throw new ArgumentException("Length of edgeArray must be equal to Height of the array.", nameof(edgeArray));

                for (int y = 0; y < Height; y++)
                    Data[0, y] = edgeArray[y];
                break;

            case Edge.Right:
                if (edgeArray.Length != Height)
                    throw new ArgumentException("Length of edgeArray must be equal to Height of the array.", nameof(edgeArray));

                for (int y = 0; y < Height; y++)
                    Data[Width - 1, y] = edgeArray[y];
                break;

            default:
                throw new ArgumentOutOfRangeException(nameof(e), "Invalid edge specified");
        }
    }

    // =====================================================================================
    // MARK: 1D array
    // =====================================================================================

    public GloFloat1DArray GetRow(int row)
    {
        GloFloat1DArray rowArray = new GloFloat1DArray(Width);
        for (int x = 0; x < Width; x++)
            rowArray[x] = Data[x, row];
        return rowArray;
    }

    public void SetRow(int row, GloFloat1DArray rowArray)
    {
        if (rowArray.Length != Width)
            throw new ArgumentException("Length of rowArray must be equal to Width of the array.", nameof(rowArray));

        for (int x = 0; x < Width; x++)
            Data[x, row] = rowArray[x];
    }

    public GloFloat1DArray GetCol(int col)
    {
        GloFloat1DArray colArray = new GloFloat1DArray(Height);
        for (int y = 0; y < Height; y++)
            colArray[y] = Data[col, y];
        return colArray;
    }

    public void SetCol(int col, GloFloat1DArray colArray)
    {
        if (colArray.Length != Height)
            throw new ArgumentException("Length of colArray must be equal to Height of the array.", nameof(colArray));

        for (int y = 0; y < Height; y++)
            Data[col, y] = colArray[y];
    }

    // =====================================================================================
    // MARK: Single Value Functions
    // =====================================================================================

    // Get value from the grid, based on x,y fractions interpolated around the surrounding
    // values.

    public float InterpolatedValue(float fractionx, float fractiony)
    {
        // Calculate indices
        int x = GloValueUtils.Clamp((int)(fractionx * (Data.GetLength(0) - 1)), 0, Data.GetLength(0) - 1);
        int y = GloValueUtils.Clamp((int)(fractiony * (Data.GetLength(1) - 1)), 0, Data.GetLength(1) - 1);

        // Calculate fractions for interpolation
        float fx = fractionx * (Data.GetLength(0) - 1) - x;
        float fy = fractiony * (Data.GetLength(1) - 1) - y;

        // Get surrounding values
        float v00 = Data[GloValueUtils.Clamp(x,     0, Data.GetLength(0) - 1), GloValueUtils.Clamp(y,     0, Data.GetLength(1) - 1)];
        float v10 = Data[GloValueUtils.Clamp(x + 1, 0, Data.GetLength(0) - 1), GloValueUtils.Clamp(y,     0, Data.GetLength(1) - 1)];
        float v01 = Data[GloValueUtils.Clamp(x,     0, Data.GetLength(0) - 1), GloValueUtils.Clamp(y + 1, 0, Data.GetLength(1) - 1)];
        float v11 = Data[GloValueUtils.Clamp(x + 1, 0, Data.GetLength(0) - 1), GloValueUtils.Clamp(y + 1, 0, Data.GetLength(1) - 1)];

        // Perform bilinear interpolation
        float interpolatedValue = (1 - fx) * (1 - fy) * v00 + fx * (1 - fy) * v10 + (1 - fx) * fy * v01 + fx * fy * v11;

        return interpolatedValue;
    }

    // =====================================================================================
    // MARK: Grid Functions
    // =====================================================================================

    // Create a subsampled grid, size defined by a skip counter. 1 = every other item.
    // To avoid confusion in any rounding, the function creates a list of indexes to then work with.

    public GloFloat2DArray GetInterpolatedGrid(int inNewSizeX, int inNewSizeY)
    {
        GloFloat2DArray retGrid = new GloFloat2DArray(inNewSizeX, inNewSizeY);

        if (Width <= 3 || Height <= 3)
            throw new Exception("Grid is too small to interpolate.");

        float xFactor = (float)Width  / (float)inNewSizeX;
        float yFactor = (float)Height / (float)inNewSizeY;

        for (int i = 0; i < inNewSizeX; i++)
        {
            int xIndex = Math.Min((int)(i * xFactor), Width - 2);
            float xRemainder = i * xFactor - xIndex;

            for (int j = 0; j < inNewSizeY; j++)
            {
                int yIndex = Math.Min((int)(j * yFactor), Height - 2);
                float yRemainder = j * yFactor - yIndex;

                float a = Data[xIndex,     yIndex];
                float b = Data[xIndex + 1, yIndex];
                float c = Data[xIndex,     yIndex + 1];
                float d = Data[xIndex + 1, yIndex + 1];

                float newVal =
                    (1 - xRemainder) * (1 - yRemainder) * a +
                         xRemainder  * (1 - yRemainder) * b +
                    (1 - xRemainder) *      yRemainder  * c +
                         xRemainder  *      yRemainder  * d;

                retGrid.Data[i, j] = newVal;
            }
        }
        return retGrid;
    }

    public GloFloat2DArray GetSubgrid(int startX, int startY, int subgridWidth, int subgridHeight)
    {
        // Clamp the start values fit in the grid size
        startX = GloValueUtils.Clamp(startX, 0, Width - 1);
        startY = GloValueUtils.Clamp(startY, 0, Height - 1);

        // Ensure that the subgrid does not extend beyond the bounds of the main grid
        int endX = Math.Min(startX + subgridWidth, Width);
        int endY = Math.Min(startY + subgridHeight, Height);

        // Calculate actual width and height of the subgrid
        subgridWidth  = endX - startX;
        subgridHeight = endY - startY;

        // Create the return object
        GloFloat2DArray outGrid = new GloFloat2DArray(subgridWidth, subgridHeight);

        for (int x = 0; x < subgridWidth; x++)
        {
            int srcX = x + startX;

            for (int y = 0; y < subgridHeight; y++)
            {
                int srcY = y + startY;
                outGrid.Data[x, y] = Data[srcX, srcY];
            }
        }
        return outGrid;
    }


    public GloFloat2DArray GetInterpolatedSubgrid(Glo2DGridPos gridPos, int subgridWidth, int subgridHeight)
    {
        int totalSubgridWidth  = gridPos.Width  * subgridWidth;
        int totalSubgridHeight = gridPos.Height * subgridHeight;

        GloFloat2DArray interpolatedGrid = GetInterpolatedGrid(totalSubgridWidth, totalSubgridHeight);

        int subgridStartX = gridPos.PosX * subgridWidth;
        int subgridStartY = gridPos.PosY * subgridHeight;

        return interpolatedGrid.GetSubgrid(subgridStartX, subgridStartY, subgridWidth, subgridHeight);
    }

    public GloFloat2DArray[,] GetInterpolatedSubGridCellWithOverlap(int inNumSubgridCols, int inNumSubgridRows, int inSubgridSizeX, int inSubgridSizeY)
    {
        int totalSubgridWidth  = inNumSubgridCols * (inSubgridSizeX - 1) + 1 + 1;
        int totalSubgridHeight = inNumSubgridRows * (inSubgridSizeY - 1) + 1 + 1;

        GloFloat2DArray interpolatedGrid = GetInterpolatedGrid(totalSubgridWidth, totalSubgridHeight);

        GloFloat2DArray[,] subGrid = new GloFloat2DArray[inNumSubgridCols, inNumSubgridRows];

        for (int i = 0; i < inNumSubgridCols; i++)
        {
            for (int j = 0; j < inNumSubgridRows; j++)
            {
                int subgridStartX = i * (inSubgridSizeX - 1);
                int subgridStartY = j * (inSubgridSizeY - 1);

                subGrid[i, j] = interpolatedGrid.GetSubgrid(subgridStartX, subgridStartY, inSubgridSizeX, inSubgridSizeY);
            }
        }

        return subGrid;
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Test Patterns
    // --------------------------------------------------------------------------------------------

    // Create a test pattern of a sine wave - GloFloat2DArray.AntennaPattern_001(100, 100);

    public static GloFloat2DArray AntennaPattern_001(int inSizeX, int inSizeY)
    {
        GloFloat2DArray retGrid = new GloFloat2DArray(50, 50);

        for (int y = 0; y < 25; y++)
        {
            float val = GloValueUtils.Interpolate(0.0f, 0.21f, (float)(y/25f));
            retGrid.SetRow(y, val);
        }
        for (int y = 25; y < 40; y++)
        {
            float val = GloValueUtils.Interpolate(0.12f, 0.24f, (float)((y-25)/15f));
            retGrid.SetRow(y, val);
        }
        for (int y = 40; y < 50; y++)
        {
            float val = GloValueUtils.Interpolate(0.12f, 0.26f, (float)((y-40)/10f));
            retGrid.SetRow(y, val);
        }

        GloFloat2DArray retGrid2 = retGrid.GetInterpolatedGrid(inSizeX, inSizeY);

        return retGrid2;
    }

}
