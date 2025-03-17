
// Fssbal using FssFloat2DArray  = FssNumeric2DArray<float>;
// Fssbal using FssDouble2DArray = FssNumeric2DArray<double>;


using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

public partial class FssNumeric2DArray<T> where T : struct, INumber<T>
{
    private T[,] Data;
    public int Width  { get; }
    public int Height { get; }
    public int Count => Width * Height;

    public enum Edge {Undefined, Top, Bottom, Left, Right};

    public bool Populated { get; set; }

    // --------------------------------------------------------------------------------------------
    // MARK: Constructors
    // --------------------------------------------------------------------------------------------

    public FssNumeric2DArray(int width, int height)
    {
        if (width < 1 || height < 1)
            throw new ArgumentException("Width and Height must be greater than 0.");

        Width     = width;
        Height    = height;
        Data      = new T[Width, Height];
        Populated = false;
    }

    public FssNumeric2DArray(T[,] initialData)
    {
        Width  = initialData.GetLength(0);
        Height = initialData.GetLength(1);
        Data   = new T[Width, Height];

        Array.Copy(initialData, Data, initialData.Length);
        Populated = true;
    }

    public FssNumeric2DArray(FssNumeric2DArray<T> other)
    {
        Width  = other.Width;
        Height = other.Height;
        Data   = new T[Width, Height];

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
    // MARK: Operations
    // --------------------------------------------------------------------------------------------

    public T MinVal() => Data.Cast<T>().Min();
    public T MaxVal() => Data.Cast<T>().Max();
    public T Sum()    => Data.Cast<T>().Aggregate(T.Zero, (current, value) => current + value);

    public FssNumeric2DArray<T> Scale(T newMin, T newMax)
    {
        T oldMin   = MinVal();
        T oldMax   = MaxVal();
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

    // --------------------------------------------------------------------------------------------
    // MARK: Set Value
    // --------------------------------------------------------------------------------------------

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

    // --------------------------------------------------------------------------------------------
    // MARK: Flip
    // --------------------------------------------------------------------------------------------

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
            int destinationY = Height - 1 - y;
            for (int x = 0; x < Width; x++)
            {
                reversed[x, destinationY] = Data[x, y];
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
                int destinationX = Width - 1 - x;
                reversed[destinationX, y] = Data[x, y];
            }
        }

        return reversed;
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Interpolation
    // --------------------------------------------------------------------------------------------

    // Get value from the grid, based on x,y fractions interpolated around the surrounding
    // values.

    public T InterpolatedValue(T fractionx, T fractiony)
    {
        // Calculate indices
        T   scaleX = T.CreateChecked(Width - 1);
        T   scaleY = T.CreateChecked(Height - 1);
        int x      = int.CreateChecked(fractionx * scaleX);
        int y      = int.CreateChecked(fractiony * scaleY);

        x = Math.Clamp(x, 0, Width  - 1);
        y = Math.Clamp(y, 0, Height - 1);

        // Calculate fractions for interpolation
        T fx = fractionx * scaleX - T.CreateChecked(x);
        T fy = fractiony * scaleY - T.CreateChecked(y);

        // Get surrounding values
        T v00 = Data[x, y];
        T v10 = Data[Math.Clamp(x + 1, 0, Width - 1), y];
        T v01 = Data[x, Math.Clamp(y + 1, 0, Height - 1)];
        T v11 = Data[Math.Clamp(x + 1, 0, Width - 1), Math.Clamp(y + 1, 0, Height - 1)];

        // Perform bilinear interpolation
        T interpolatedValue = (T.One - fx) * (T.One - fy) * v00
                            + fx           * (T.One - fy) * v10
                            + (T.One - fx) * fy           * v01
                            + fx           * fy           * v11;

        return interpolatedValue;
    }

    // --------------------------------------------------------------------------------------------
    // MARK: String
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
