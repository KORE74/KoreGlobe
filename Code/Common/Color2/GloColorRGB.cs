// Custom colour class, protable between frameworks.

using System;

public struct GloColorRGB
{
    public float R { set; get; }
    public float G { set; get; }
    public float B { set; get; }
    public float A { set; get; }

    public byte RByte {
        get => GloColorIO.FloatToByte(R);
        set => R = GloColorIO.ByteToFloat(value);
    }
    public byte GByte {
        get => GloColorIO.FloatToByte(G);
        set => G = GloColorIO.ByteToFloat(value);
    }
    public byte BByte {
        get => GloColorIO.FloatToByte(B);
        set => B = GloColorIO.ByteToFloat(value);
    }
    public byte AByte {
        get => GloColorIO.FloatToByte(A);
        set => A = GloColorIO.ByteToFloat(value);
    }

    // --------------------------------------------------------------------------------------------
    // Constructors
    // --------------------------------------------------------------------------------------------

    public GloColorRGB(byte r, byte g, byte b, byte a)
    {
        RByte = r;
        GByte = g;
        BByte = b;
        AByte = a;
    }

    public GloColorRGB(byte r, byte g, byte b)
    {
        RByte = r;
        GByte = g;
        BByte = b;
        AByte = 255;
    }

    public GloColorRGB(float r, float g, float b, float a)
    {
        R = GloColorIO.LimitFloat(r);
        G = GloColorIO.LimitFloat(g);
        B = GloColorIO.LimitFloat(b);
        A = GloColorIO.LimitFloat(a);
    }

    public GloColorRGB(float r, float g, float b)
    {
        R = GloColorIO.LimitFloat(r);
        G = GloColorIO.LimitFloat(g);
        B = GloColorIO.LimitFloat(b);
        A = 1.0f;
    }

    public static readonly GloColorRGB Zero = new GloColorRGB(0f, 0f, 0f, 0f);
    public static readonly GloColorRGB White = new GloColorRGB(1f, 1f, 1f, 1f);

    // --------------------------------------------------------------------------------------------
    // MARK: Changes
    // --------------------------------------------------------------------------------------------

    // Usage: GloColorRGB.Lerp(col1, col2, t)
    // 0 = col1, 1 = col2, 0.5 = halfway between col1 and col2

    public static GloColorRGB Lerp(GloColorRGB col1, GloColorRGB col2, float col2fraction)
    {
        if (col2fraction < 0.0f) col2fraction = 0.0f;
        if (col2fraction > 1.0f) col2fraction = 1.0f;

        float newR = col1.R + (col2.R - col1.R) * col2fraction;
        float newG = col1.G + (col2.G - col1.G) * col2fraction;
        float newB = col1.B + (col2.B - col1.B) * col2fraction;
        float newA = col1.A + (col2.A - col1.A) * col2fraction;

        return new GloColorRGB(newR, newG, newB, newA);
    }


}