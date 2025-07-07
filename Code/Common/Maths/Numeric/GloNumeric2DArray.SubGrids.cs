
// global using GloFloat2DArray  = GloNumeric2DArray<float>;
// global using GloDouble2DArray = GloNumeric2DArray<double>;


using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

public partial class GloNumeric2DArray<T> where T : struct, INumber<T>
{
    public GloNumeric2DArray<T> GetInterpolatedGrid(int inNewSizeX, int inNewSizeY)
    {
        GloNumeric2DArray<T> retGrid = new GloNumeric2DArray<T>(inNewSizeX, inNewSizeY);

        if (Width <= 3 || Height <= 3)
            throw new Exception("Grid is too small to interpolate.");

        T xFactor = T.CreateChecked(Width) / T.CreateChecked(inNewSizeX);
        T yFactor = T.CreateChecked(Height) / T.CreateChecked(inNewSizeY);
        float xFactorFloat = float.CreateChecked(xFactor);
        float yFactorFloat = float.CreateChecked(yFactor);

        for (int i = 0; i < inNewSizeX; i++)
        {
            int xIndex = Math.Min((int)(i * xFactorFloat), Width - 2);
            T xRemainder = T.CreateChecked(i) * xFactor - T.CreateChecked(xIndex);

            for (int j = 0; j < inNewSizeY; j++)
            {
                int yIndex = Math.Min((int)(j * yFactorFloat), Height - 2);
                T yRemainder = T.CreateChecked(j) * yFactor - T.CreateChecked(yIndex);

                T a = Data[xIndex, yIndex];
                T b = Data[xIndex + 1, yIndex];
                T c = Data[xIndex, yIndex + 1];
                T d = Data[xIndex + 1, yIndex + 1];

                T newVal = (T.One - xRemainder) * (T.One - yRemainder) * a +
                           xRemainder * (T.One - yRemainder) * b +
                           (T.One - xRemainder) * yRemainder * c +
                           xRemainder * yRemainder * d;

                retGrid[i, j] = newVal;
            }
        }
        return retGrid;
    }

    // --------------------------------------------------------------------------------------------

    public GloNumeric2DArray<T> GetSubgrid(int startX, int startY, int subgridWidth, int subgridHeight)
    {
        // Clamp the start values to fit in the grid size
        startX = Math.Clamp(startX, 0, Width - 1);
        startY = Math.Clamp(startY, 0, Height - 1);

        // Ensure that the subgrid does not extend beyond the bounds of the main grid
        int endX = Math.Min(startX + subgridWidth, Width);
        int endY = Math.Min(startY + subgridHeight, Height);

        // Calculate actual width and height of the subgrid
        subgridWidth  = endX - startX;
        subgridHeight = endY - startY;

        // Create the return object
        GloNumeric2DArray<T> outGrid = new GloNumeric2DArray<T>(subgridWidth, subgridHeight);

        for (int x = 0; x < subgridWidth; x++)
        {
            int srcX = x + startX;

            for (int y = 0; y < subgridHeight; y++)
            {
                int srcY = y + startY;
                outGrid[x, y] = Data[srcX, srcY];
            }
        }
        return outGrid;
    }

    // --------------------------------------------------------------------------------------------

    public GloNumeric2DArray<T> GetInterpolatedSubgrid(Glo2DGridPos gridPos, int subgridWidth, int subgridHeight)
    {
        int totalSubgridWidth  = gridPos.Width * subgridWidth;
        int totalSubgridHeight = gridPos.Height * subgridHeight;

        GloNumeric2DArray<T> interpolatedGrid = GetInterpolatedGrid(totalSubgridWidth, totalSubgridHeight);

        int subgridStartX = gridPos.PosX * subgridWidth;
        int subgridStartY = gridPos.PosY * subgridHeight;

        return interpolatedGrid.GetSubgrid(subgridStartX, subgridStartY, subgridWidth, subgridHeight);
    }

    // --------------------------------------------------------------------------------------------

    public GloNumeric2DArray<T>[,] GetInterpolatedSubGridCellWithOverlap(int inNumSubgridCols, int inNumSubgridRows, int inSubgridSizeX, int inSubgridSizeY)
    {
        int totalSubgridWidth  = inNumSubgridCols * (inSubgridSizeX - 1) + 1 + 1;
        int totalSubgridHeight = inNumSubgridRows * (inSubgridSizeY - 1) + 1 + 1;

        GloNumeric2DArray<T> interpolatedGrid = GetInterpolatedGrid(totalSubgridWidth, totalSubgridHeight);

        GloNumeric2DArray<T>[,] subGrid = new GloNumeric2DArray<T>[inNumSubgridCols, inNumSubgridRows];

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
}
