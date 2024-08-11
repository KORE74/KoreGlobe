using System;
using Godot;

public class FssUvBoxDropEdgeTile
{
    // Define the basic corners of the tile (or subtile) in UV co-ordinates of its texture
    public Vector2 TopLeft { get; private set; }
    public Vector2 BottomRight { get; private set; }

    // The grid of UVs output for the tile, accomodating the drop-edge and insets 
    private readonly Vector2[,] uvGrid;

    // --------------------------------------------------------------------------------------------

    // Constructor that sets up the corners and initializes the UV grid
    public FssUvBoxDropEdgeTile(Vector2 topLeft, Vector2 bottomRight, int horizSize, int vertSize, float edgeOffset = 0.001f, float insetOffset = 0.001f)
    {
        TopLeft     = topLeft;
        BottomRight = bottomRight;

        uvGrid = new Vector2[horizSize + 2, vertSize + 2];
        InitializeUvGrid(horizSize, vertSize, edgeOffset, insetOffset);
    }

    // Constructor that derives the corners from a larger UV box
    public FssUvBoxDropEdgeTile(FssUvBoxDropEdgeTile parentTile, int horizSize, int vertSize, int horizIndex, int vertIndex)
    {
        // Create the UV box for the subtile to help derive the UV grid
        FssUvBox subtileBox = FssUvBox.BoxFromGrid(parentTile.TopLeft, parentTile.BottomRight, horizSize, vertSize, horizIndex, vertIndex);
    
        TopLeft     = subtileBox.TopLeft;
        BottomRight = subtileBox.BottomRight;

        uvGrid = new Vector2[horizSize + 2, vertSize + 2];
        InitializeUvGrid(horizSize, vertSize);
    }

    // --------------------------------------------------------------------------------------------

    // Initializes the UV grid with offsets and insets
    private void InitializeUvGrid(int horizSize, int vertSize, float edgeOffset = 0.001f, float insetOffset = 0.001f)
    {
        float horizStep = (1.0f - 2 * edgeOffset) / horizSize;
        float vertStep  = (1.0f - 2 * edgeOffset) / vertSize;

        for (int y = 0; y < vertSize + 2; y++)
        {
            for (int x = 0; x < horizSize + 2; x++)
            {
                uvGrid[x, y] = CalculateUV(x, y, horizSize, vertSize, horizStep, vertStep, edgeOffset, insetOffset);
            }
        }
    }

    // --------------------------------------------------------------------------------------------

    // Calculates individual UV coordinates based on grid position and offsets
    private Vector2 CalculateUV(int x, int y, int horizSize, int vertSize, float horizStep, float vertStep, float edgeOffset, float insetOffset)
    {
        float u = Mathf.Lerp(TopLeft.X, BottomRight.X, GetStepPosition(x, horizSize, horizStep, edgeOffset, insetOffset));
        float v = Mathf.Lerp(TopLeft.Y, BottomRight.Y, GetStepPosition(y, vertSize, vertStep, edgeOffset, insetOffset));

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
        if (x < 0 || x >= uvGrid.GetLength(0) || y < 0 || y >= uvGrid.GetLength(1))
            throw new ArgumentOutOfRangeException($"Index out of bounds: x={x}, y={y}");

        return uvGrid[x, y];
    }

}
