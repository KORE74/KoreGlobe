using Godot;

public static class FssMaterialFactory
{
    // Function to create a simple colored material
    public static StandardMaterial3D SimpleColoredMaterial(Color color)
    {
        StandardMaterial3D material = new StandardMaterial3D();
        material.AlbedoColor = color;
        return material;
    }

    // Function to create a transparent colored material
    public static StandardMaterial3D TransparentColoredMaterial(Color color)
    {
        StandardMaterial3D material = new StandardMaterial3D();
        material.AlbedoColor      = color; // alphas sets extent of transparency
        material.Transparency = BaseMaterial3D.TransparencyEnum.Alpha;
        return material;
    }    
}
