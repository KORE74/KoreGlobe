
using System;
using FssJSON;
using Godot;

// FssUVBox: A struct to hold the UV co-oridnates of a box in a texture
// Also includes the functionality of a UV box within a larger texture.

public struct FssUVBox
{
    public Vector2 TopLeft     = new Vector2(0f, 0f);
    public Vector2 BottomRight = new Vector2(0f, 0f);

    public FssUVBox(Vector2 topLeft, Vector2 bottomRight)
    {
        TopLeft     = topLeft;
        BottomRight = bottomRight;
    }

    public static FssUVBox Zero
    {
        get { return new FssUVBox(); }
    }

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
    public static FssUVBox BoxFromGrid(Vector2 topLeft, Vector2 bottomRight, int horizSize, int vertSize, int horizIndex, int vertIndex)
    {
        float horizStep = 1.0f / horizSize;
        float vertStep  = 1.0f / vertSize;

        float leftValue  = topLeft.X + horizIndex * horizStep * (bottomRight.X - topLeft.X);
        float rightValue = topLeft.X + (horizIndex + 1) * horizStep * (bottomRight.X - topLeft.X);

        float topValue = topLeft.Y + vertIndex * vertStep * (bottomRight.Y - topLeft.Y);
        float botValue = topLeft.Y + (vertIndex + 1) * vertStep * (bottomRight.Y - topLeft.Y);

        return new FssUVBox(new Vector2(leftValue, topValue), new Vector2(rightValue, botValue));
    }
}



