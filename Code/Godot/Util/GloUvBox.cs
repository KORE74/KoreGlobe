
//using System;
//using System.Numerics;

using Godot;

// GloUVBox: A struct to hold the UV co-oridnates of a box in a texture
// Also includes the functionality of a UV box within a larger texture.

public struct GloUVBox
{
    public Vector2 TopLeft;
    public Vector2 BottomRight;

    public GloUVBox(Vector2 topLeft, Vector2 bottomRight)
    {
        TopLeft     = topLeft;
        BottomRight = bottomRight;
    }

    public readonly GloFloatRange UVRangeX { get { return new GloFloatRange(TopLeft.X, BottomRight.X); } }
    public readonly GloFloatRange UVRangeY { get { return new GloFloatRange(TopLeft.Y, BottomRight.Y); } }

    // --------------------------------------------------------------------------------------------

    public static GloUVBox Zero { get { return new GloUVBox(Vector2.Zero, Vector2.Zero); } }
    public static GloUVBox Full { get { return new GloUVBox(Vector2.Zero, Vector2.One); } }

    // --------------------------------------------------------------------------------------------

    // Get the UV coordinates within the box based on XY fractions
    // xFraction and yFraction should be in the range [0, 1]

    public Vector2 GetUV(float xFraction, float yFraction)
    {
        // Clamp the fractions to the range [0, 1] to ensure valid interpolation
        xFraction = Mathf.Clamp(xFraction, 0.0f, 1.0f);
        yFraction = Mathf.Clamp(yFraction, 0.0f, 1.0f);

        // Interpolate between the TopLeft and BottomRight coordinates
        float u = Mathf.Lerp(TopLeft.X, BottomRight.X, xFraction);
        float v = Mathf.Lerp(TopLeft.Y, BottomRight.Y, yFraction);

        return new Vector2(u, v);
    }

    // --------------------------------------------------------------------------------------------

    // create a UV box from a grid
    // (0,0) is top left box, (size-1, size-1) is bottom right box.

    // Creates a UV box for a subtile within the main tile
    public static GloUVBox BoxFromGrid(Vector2 topLeft, Vector2 bottomRight, int horizSize, int vertSize, int horizIndex, int vertIndex)
    {
        float horizStep = 1.0f / horizSize;
        float vertStep  = 1.0f / vertSize;

        float leftValue  = topLeft.X + horizIndex       * horizStep * (bottomRight.X - topLeft.X);
        float rightValue = topLeft.X + (horizIndex + 1) * horizStep * (bottomRight.X - topLeft.X);
        float topValue   = topLeft.Y + vertIndex        * vertStep  * (bottomRight.Y - topLeft.Y);
        float botValue   = topLeft.Y + (vertIndex + 1)  * vertStep  * (bottomRight.Y - topLeft.Y);

        return new GloUVBox(new Vector2(leftValue, topValue), new Vector2(rightValue, botValue));
    }

    public GloUVBox BoxFromGrid(Glo2DGridPos innerBoxPos)
    {
        // Calculate the horizontal and vertical step sizes
        float horizStep = (BottomRight.X - TopLeft.X) / innerBoxPos.Width;
        float vertStep  = (BottomRight.Y - TopLeft.Y) / innerBoxPos.Height;

        // Calculate the UV coordinates for the top-left corner of the inner box
        float leftValue   = TopLeft.X + innerBoxPos.PosX       * horizStep;
        float rightValue  = TopLeft.X + (innerBoxPos.PosX + 1) * horizStep;
        float topValue    = TopLeft.Y + innerBoxPos.PosY       * vertStep;
        float bottomValue = TopLeft.Y + (innerBoxPos.PosY + 1) * vertStep;

        // Return a new GloUVBox using the calculated values
        return new GloUVBox(new Vector2(leftValue, topValue), new Vector2(rightValue, bottomValue));
    }

}



