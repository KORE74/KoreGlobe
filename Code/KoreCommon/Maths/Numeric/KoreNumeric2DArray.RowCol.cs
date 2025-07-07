
// global using KoreFloat2DArray  = KoreNumeric2DArray<float>;
// global using KoreDouble2DArray = KoreNumeric2DArray<double>;


using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace KoreCommon;

public partial class KoreNumeric2DArray<T> where T : struct, INumber<T>
{
    // --------------------------------------------------------------------------------------------
    // MARK: Row
    // --------------------------------------------------------------------------------------------

    public KoreNumeric1DArray<T> GetRow(int row)
    {
        if (row < 0 || row >= Height)
            throw new ArgumentOutOfRangeException(nameof(row), "Row index is out of range.");

        KoreNumeric1DArray<T> rowData = new KoreNumeric1DArray<T>(Width);
        for (int x = 0; x < Width; x++)
        {
            rowData[x] = Data[x, row];
        }
        return rowData;
    }

    public void SetRow(int row, KoreNumeric1DArray<T> values)
    {
        if (row < 0 || row >= Height)
            throw new ArgumentOutOfRangeException(nameof(row), "Row index is out of range.");
        if (values.Length != Width)
            throw new ArgumentException("Values length must match the width of the array.");

        for (int x = 0; x < Width; x++)
        {
            Data[x, row] = values[x];
        }
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Col
    // --------------------------------------------------------------------------------------------

    public KoreNumeric1DArray<T> GetCol(int col)
    {
        if (col < 0 || col >= Width)
            throw new ArgumentOutOfRangeException(nameof(col), "Column index is out of range.");

        KoreNumeric1DArray<T> colData = new KoreNumeric1DArray<T>(Height);
        for (int y = 0; y < Height; y++)
        {
            colData[y] = Data[col, y];
        }
        return colData;
    }

    public void SetCol(int col, KoreNumeric1DArray<T> values)
    {
        if (col < 0 || col >= Width)
            throw new ArgumentOutOfRangeException(nameof(col), "Column index is out of range.");
        if (values.Length != Height)
            throw new ArgumentException("Values length must match the height of the array.");

        for (int y = 0; y < Height; y++)
        {
            Data[col, y] = values[y];
        }
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Edge functions
    // --------------------------------------------------------------------------------------------

    // array.GetEdge(KoreFloat2DArray.Edge.Top)

    public KoreNumeric1DArray<T> GetEdge(Edge e)
    {
        KoreNumeric1DArray<T> edgeArray;
        switch (e)
        {
            case Edge.Top:
                edgeArray = new KoreNumeric1DArray<T>(Width);
                for (int x = 0; x < Width; x++)
                    edgeArray[x] = Data[x, 0];
                break;

            case Edge.Bottom:
                edgeArray = new KoreNumeric1DArray<T>(Width);
                for (int x = 0; x < Width; x++)
                    edgeArray[x] = Data[x, Height - 1];
                break;

            case Edge.Left:
                edgeArray = new KoreNumeric1DArray<T>(Height);
                for (int y = 0; y < Height; y++)
                    edgeArray[y] = Data[0, y];
                break;

            case Edge.Right:
                edgeArray = new KoreNumeric1DArray<T>(Height);
                for (int y = 0; y < Height; y++)
                    edgeArray[y] = Data[Width - 1, y];
                break;

            default:
                throw new ArgumentOutOfRangeException(nameof(e), "Invalid edge specified");
        }
        return edgeArray;
    }

    public void SetEdge(KoreNumeric1DArray<T> edgeArray, Edge e)
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

}


