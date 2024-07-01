using Godot;
using System;

public static class FssLabelMaker
{
    private static Font font;
    private const int padding = 10;
    private const int fontSize = 32;

    static FssLabelMaker()
    {
        // Load a default font
        font = (Font)GD.Load("res://Fonts/RussoOne-Regular.ttf");
    }

    public static Texture2D CreateLabelTexture(string text)
    {
        // Create a new Image
        Image image = new Image();

        // Determine the size of the text
        Vector2 textSize = font.GetStringSize(text, HorizontalAlignment.Left, -1, fontSize);
        int width = (int)textSize.Y + 2 * padding;
        int height = (int)textSize.Y + 2 * padding;

        // Create the image with the determined size
        Image.CreateEmpty(width, height, false, Image.Format.Rgba8);
        //image.SetBlockSignals();

        // Fill the background with a color (white for this example)
        Color backgroundColor = new Color(1, 1, 1, 1);
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                image.SetPixel(x, y, backgroundColor);
            }
        }

        // Draw the text onto the image
        Color textColor = new Color(0, 0, 0, 1); // Black text color
        Rid canvasItem = image.GetRid();
        font.DrawString(canvasItem, new Vector2(padding, padding + font.GetAscent(fontSize)), text, HorizontalAlignment.Left, -1, fontSize, textColor);

        //image.Unlock();

        // Create a Texture2D from the Image
        ImageTexture texture = ImageTexture.CreateFromImage(image);

        return texture;
    }
}
