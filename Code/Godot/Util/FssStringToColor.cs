using Godot;
using System.Collections.Generic;

// We need to have differentable, yet consistent colors for different elements in the 
// scene. So we take its name, and reduce it down to an index in a list of colors.

public static class FssStringToColor
{
    // define the static list of colors (with a size we can easily know and expand)
    private static List<Color> ColorList = new () {
        FssColorUtil.Colors["Red"],
        FssColorUtil.Colors["Green"],
        FssColorUtil.Colors["Blue"],
        FssColorUtil.Colors["Yellow"],
        FssColorUtil.Colors["Magenta"],
        FssColorUtil.Colors["Cyan"],
        FssColorUtil.Colors["LightGray"],
        FssColorUtil.Colors["Maroon"]
    };

    public static Color StringToColor(string inName)
    {
        // Convert the string to a hash code
        int hash = inName.GetHashCode();

        // Use the hash code to index into the color list
        int index = hash % ColorList.Count;

        // Return the color at that index
        return ColorList[index];

    }
}