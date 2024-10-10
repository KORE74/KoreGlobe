
// FssInt2D: A class to store a positive 2D integer size.

public struct FssInt2D
{
    public int X
    {
        get => X;
        set => X = value < 0 ? 0 : value;
    }

    public int Y
    {
        get => Y;
        set => Y = value < 0 ? 0 : value;
    }

    public FssInt2D(int x, int y)
    {
        X = x;
        Y = y;
    }

    public override string ToString()
    {
        return $"({X}, {Y})";
    }
}

