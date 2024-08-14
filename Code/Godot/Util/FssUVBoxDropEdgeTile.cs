using System;
using Godot;

public class FssUvBoxDropEdgeTile
{
    // Define the basic corners of the tile (or subtile) in UV co-ordinates of its texture
    public Vector2 TopLeft     { get; private set; }
    public Vector2 BottomRight { get; private set; }

    // The edge offset for the tile, to prevent texture bleeding
    public float BoxEdgeOffset  { get; private set; }
    public float BoxInsetOffset { get; private set; }

    // The grid of UVs output for the tile, accomodating the drop-edge and insets
    private Vector2[,] UVGrid;

    // FssUvBoxDropEdgeTile.UVTopLeft and FssUvBoxDropEdgeTile.BottomRight
    public static Vector2 UVTopLeft     = new Vector2(0.0f, 0.0f);
    public static Vector2 UVBottomRight = new Vector2(1.0f, 1.0f);

    // --------------------------------------------------------------------------------------------

    // Constructor that sets up the corners and initializes the UV grid

    public FssUvBoxDropEdgeTile(Vector2 topLeft, Vector2 bottomRight, int horizSize, int vertSize, float edgeOffset = 0.001f, float insetOffset = 0.001f)
    {
        TopLeft     = topLeft;
        BottomRight = bottomRight;

        BoxEdgeOffset  = edgeOffset;
        BoxInsetOffset = insetOffset;

        InitializeUvGrid(horizSize, vertSize);
    }

    // Constructor that derives the UV box from a parent tile and subgrid position

    public FssUvBoxDropEdgeTile(FssUvBoxDropEdgeTile parentBox, int horizSize, int vertSize, Fss2DGridPos gridPos)
    {
        // Calculate the new UV box from the parent box and grid position
        float minParentX = TopLeft.X;
        float maxParentX = BottomRight.X;
        float minParentY = TopLeft.Y;
        float maxParentY = BottomRight.Y;

        float diffParentX = maxParentX - minParentX;
        float diffParentY = maxParentY - minParentY;

        float childMinX = minParentX + gridPos.LeftEdgeFraction * diffParentX;
        float childMaxX = minParentX + gridPos.RightEdgeFraction * diffParentX;
        float childMinY = minParentY + gridPos.TopEdgeFraction * diffParentY;
        float childMaxY = minParentY + gridPos.BottomEdgeFraction * diffParentY;

        Vector2 childTopLeft     = new Vector2(childMinX, childMinY);
        Vector2 childBottomRight = new Vector2(childMaxX, childMaxY);

        BoxEdgeOffset  = parentBox.BoxEdgeOffset;
        BoxInsetOffset = parentBox.BoxInsetOffset;

        InitializeUvGrid(horizSize, vertSize);
    }

    // With no input information, return a default UV box

    public static FssUvBoxDropEdgeTile Default(int horizSize, int vertSize)
    {
        return new FssUvBoxDropEdgeTile(UVTopLeft, UVBottomRight, horizSize, vertSize);
    }

    // --------------------------------------------------------------------------------------------

    // Initializes the UV grid with offsets and insets
    private void InitializeUvGrid(int horizSize, int vertSize)
    {
        UVGrid = new Vector2[horizSize + 2, vertSize + 2];

        float horizStep = (1.0f - 2 * BoxEdgeOffset) / horizSize;
        float vertStep  = (1.0f - 2 * BoxEdgeOffset) / vertSize;

        for (int y = 0; y < vertSize + 2; y++)
        {
            for (int x = 0; x < horizSize + 2; x++)
            {
                UVGrid[x, y] = CalculateUV(x, y, horizSize, vertSize, horizStep, vertStep, BoxEdgeOffset, BoxInsetOffset);
            }
        }
    }

    // --------------------------------------------------------------------------------------------

    // Calculates individual UV coordinates based on grid position and offsets
    private Vector2 CalculateUV(int x, int y, int horizSize, int vertSize, float horizStep, float vertStep, float edgeOffset, float insetOffset)
    {
        float u = Mathf.Lerp(TopLeft.X, BottomRight.X, GetStepPosition(x, horizSize, horizStep, edgeOffset, insetOffset));
        float v = Mathf.Lerp(TopLeft.Y, BottomRight.Y, GetStepPosition(y, vertSize,  vertStep,  edgeOffset, insetOffset));

        return new Vector2(u, v);
    }

    // --------------------------------------------------------------------------------------------

    // Determines the position within the step, considering offsets and insets
    private float GetStepPosition(int index, int size, float step, float edgeOffset, float insetOffset)
    {
        if (index == 0)
            return edgeOffset;
        else if (index == size + 1)
            return 1.0f - edgeOffset;
        else if (index == 1)
            return edgeOffset + insetOffset;
        else if (index == size)
            return 1.0f - edgeOffset - insetOffset;
        else
            return edgeOffset + insetOffset + (index - 1) * step;
    }

    // --------------------------------------------------------------------------------------------

    // Provides access to the precomputed UV coordinates
    public Vector2 GetUV(int x, int y)
    {
        if (x < 0 || x >= UVGrid.GetLength(0) || y < 0 || y >= UVGrid.GetLength(1))
            throw new ArgumentOutOfRangeException($"Index out of bounds: x={x}, y={y}");

        return UVGrid[x, y];
    }

}
