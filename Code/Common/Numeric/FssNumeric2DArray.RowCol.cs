
// Fssbal using FssFloat2DArray  = FssNumeric2DArray<float>;
// Fssbal using FssDouble2DArray = FssNumeric2DArray<double>;


using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

public partial class FssNumeric2DArray<T> where T : struct, INumber<T>
{
    // --------------------------------------------------------------------------------------------
    // MARK: Row Col Access
    // --------------------------------------------------------------------------------------------

    public FssNumeric1DArray<T> GetRow(int row)
    {
         FssNumeric1DArray<T> rowArray = new  FssNumeric1DArray<T>(Width);
        for (int x = 0; x < Width; x++)
            rowArray[x] = Data[x, row];
        return rowArray;
    }

    public void SetRow(int row,  FssNumeric1DArray<T> rowArray)
    {
        if (rowArray.Length != Width)
            throw new ArgumentException("Length of rowArray must be equal to Width of the array.", nameof(rowArray));

        for (int x = 0; x < Width; x++)
            Data[x, row] = rowArray[x];
    }

    public  FssNumeric1DArray<T> GetCol(int col)
    {
         FssNumeric1DArray<T> colArray = new  FssNumeric1DArray<T>(Height);
        for (int y = 0; y < Height; y++)
            colArray[y] = Data[col, y];
        return colArray;
    }

    public void SetCol(int col,  FssNumeric1DArray<T> colArray)
    {
        if (colArray.Length != Height)
            throw new ArgumentException("Length of colArray must be equal to Height of the array.", nameof(colArray));

        for (int y = 0; y < Height; y++)
            Data[col, y] = colArray[y];
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Edge functions
    // --------------------------------------------------------------------------------------------

    // array.GetEdge(FssFloat2DArray.Edge.Top)

    public FssNumeric1DArray<T> GetEdge(Edge e)
    {
        FssNumeric1DArray<T> edgeArray;
        switch (e)
        {
            case Edge.Top:
                edgeArray = new FssNumeric1DArray<T>(Width);
                for (int x = 0; x < Width; x++)
                    edgeArray[x] = Data[x, 0];
                break;

            case Edge.Bottom:
                edgeArray = new FssNumeric1DArray<T>(Width);
                for (int x = 0; x < Width; x++)
                    edgeArray[x] = Data[x, Height - 1];
                break;

            case Edge.Left:
                edgeArray = new FssNumeric1DArray<T>(Height);
                for (int y = 0; y < Height; y++)
                    edgeArray[y] = Data[0, y];
                break;

            case Edge.Right:
                edgeArray = new FssNumeric1DArray<T>(Height);
                for (int y = 0; y < Height; y++)
                    edgeArray[y] = Data[Width - 1, y];
                break;

            default:
                throw new ArgumentOutOfRangeException(nameof(e), "Invalid edge specified");
        }
        return edgeArray;
    }

    public void SetEdge(FssNumeric1DArray<T> edgeArray, Edge e)
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
