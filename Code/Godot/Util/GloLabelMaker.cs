using Godot;

using System;

using Godot.Collections;
using Godot.NativeInterop;


public static class GloLabelMaker
{
    private static Font font;
    private const int padding = 10;
    private const int fontSize = 32;

    static GloLabelMaker()
    {
        // Load a default font
        font = (Font)GD.Load("res://Fonts/RussoOne-Regular.ttf");
    }

    public static Texture2D CreateLabelTexture(string text)
    {
        // Determine the size of the text
        Vector2 textSize = font.GetStringSize(text, HorizontalAlignment.Left, -1, fontSize);
        int width = (int)textSize.X + 2 * padding;
        int height = (int)textSize.Y + 2 * padding;

        // Create the image with the determined size, and fill with background color
        Image image = Image.CreateEmpty(width, height, false, Image.Format.Rgba8);
        image.Fill(new Color(1, 0.4f, 0.1f, 1));

        // Draw the text onto the image
        Color textColor = new Color(0, 0, 0, 1); // Black text color
        Vector2 textPosition = new Vector2(padding, padding + font.GetAscent(fontSize));

        //image.Draw(textPosition, text, textColor, font, fontSize);
        //image.DrawText(font, textPosition, text, textColor, fontSize);

        Texture2D texture = new Texture2D();

        // using (Bitmap bmpText = new Bitmap(width, height))
        // {

        // }

        // // set the texture size
        // //Texture2D.Create(width, height, Image.Format.Rgba8);

        // Texture2D tex = ImageTexture.CreateFromImage(image);
        // //t.DrawString(font, textPosition, text, textColor, fontSize);

        // tex.DrawText(font, textPosition, text, textColor, fontSize);

        // // Unlock the image
        // //image.Unlock();

        // // Create a final texture from the image
        // //ImageTexture texture = new ImageTexture();
        // //texture.CreateFromImage(image);

        // // Debug save the image to disk, with a timestamp filename
        // image.SavePng("label_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".png");

        return texture;
    }
}
