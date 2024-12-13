using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

public class FssNumeric2DArray<T> where T : struct, INumber<T>
{
    private T[,] Data;
    public int Width { get; }
    public int Height { get; }
    public int Count => Width * Height;

    public bool Populated { get; set; }

    // --------------------------------------------------------------------------------------------
    // Constructors
    // --------------------------------------------------------------------------------------------

    public FssNumeric2DArray(int width, int height)
    {
        if (width < 1 || height < 1)
            throw new ArgumentException("Width and Height must be greater than 0.");

        Width = width;
        Height = height;
        Data = new T[Width, Height];
        Populated = false;
    }

    public FssNumeric2DArray(T[,] initialData)
    {
        Width = initialData.GetLength(0);
        Height = initialData.GetLength(1);
        Data = new T[Width, Height];

        Array.Copy(initialData, Data, initialData.Length);
        Populated = true;
    }

    public FssNumeric2DArray(FssNumeric2DArray<T> other)
    {
        Width = other.Width;
        Height = other.Height;
        Data = new T[Width, Height];

        Array.Copy(other.Data, Data, other.Data.Length);
        Populated = other.Populated;
    }

    // --------------------------------------------------------------------------------------------
    // Indexer
    // --------------------------------------------------------------------------------------------

    public T this[int x, int y]
    {
        get => Data[x, y];
        set => Data[x, y] = value;
    }

    // --------------------------------------------------------------------------------------------
    // Operations
    // --------------------------------------------------------------------------------------------

    public T MinVal()
    {
        return Data.Cast<T>().Min();
    }

    public T MaxVal()
    {
        return Data.Cast<T>().Max();
    }

    public T Sum()
    {
        return Data.Cast<T>().Aggregate(T.Zero, (current, value) => current + value);
    }

    public FssNumeric2DArray<T> Scale(T newMin, T newMax)
    {
        T oldMin = MinVal();
        T oldMax = MaxVal();

        T oldRange = oldMax - oldMin;
        T newRange = newMax - newMin;

        FssNumeric2DArray<T> scaledArray = new FssNumeric2DArray<T>(Width, Height);

        for (int y = 0; y < Height; y++)
        {
            for (int x = 0; x < Width; x++)
            {
                T value = (Data[x, y] - oldMin) / oldRange * newRange + newMin;
                scaledArray[x, y] = value;
            }
        }

        return scaledArray;
    }

    public void SetAll(T value)
    {
        for (int y = 0; y < Height; y++)
            for (int x = 0; x < Width; x++)
                Data[x, y] = value;

        Populated = true;
    }

    public void SetRow(int row, T value)
    {
        for (int x = 0; x < Width; x++)
            Data[x, row] = value;
    }

    public void SetCol(int col, T value)
    {
        for (int y = 0; y < Height; y++)
            Data[col, y] = value;
    }

    public FssNumeric2DArray<T> Transpose()
    {
        FssNumeric2DArray<T> transposed = new FssNumeric2DArray<T>(Height, Width);

        for (int y = 0; y < Height; y++)
        {
            for (int x = 0; x < Width; x++)
            {
                transposed[y, x] = Data[x, y];
            }
        }

        return transposed;
    }

    public FssNumeric2DArray<T> ReverseRows()
    {
        FssNumeric2DArray<T> reversed = new FssNumeric2DArray<T>(Width, Height);

        for (int y = 0; y < Height; y++)
        {
            for (int x = 0; x < Width; x++)
            {
                reversed[x, Height - 1 - y] = Data[x, y];
            }
        }

        return reversed;
    }

    public FssNumeric2DArray<T> ReverseCols()
    {
        FssNumeric2DArray<T> reversed = new FssNumeric2DArray<T>(Width, Height);

        for (int y = 0; y < Height; y++)
        {
            for (int x = 0; x < Width; x++)
            {
                reversed[Width - 1 - x, y] = Data[x, y];
            }
        }

        return reversed;
    }

    // --------------------------------------------------------------------------------------------
    // Helper Methods
    // --------------------------------------------------------------------------------------------

    public override string ToString()
    {
        var sb = new System.Text.StringBuilder();

        for (int y = 0; y < Height; y++)
        {
            for (int x = 0; x < Width; x++)
            {
                sb.Append(Data[x, y] + " ");
            }
            sb.AppendLine();
        }

        return sb.ToString();
    }
}
