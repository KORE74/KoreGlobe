using Godot;
using System;

// FssLabel3DFactory: Class that creates new Label3D instances

public static class FssLabel3DFactory
{
    public static Label3D CreateLabel(string text, float pixelSize)
    {
        // Create a new Label3D instance
        Label3D label = new Label3D();

        // Set the text of the label
        label.Text = text;
        label.Name = text;

        // Set the font of the label
        label.Font = (Font)GD.Load("res://Fonts/RussoOne-Regular.ttf");

        // Set the font size of the label
        label.FontSize  = 32;
        label.PixelSize = pixelSize;

        return label;
    }
}
