
public class FssIntSize
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

    public FssIntSize(int x, int y)
    {
        X = x;
        Y = y;
    }

    public override string ToString()
    {
        return $"({X}, {Y})";
    }
}