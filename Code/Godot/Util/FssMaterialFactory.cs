using Godot;

public static class FssMaterialFactory
{
    // --------------------------------------------------------------------------------------------
    // #MARK: Mature / fixed functions
    // --------------------------------------------------------------------------------------------


    // Function to create a simple colored material
    public static StandardMaterial3D SimpleColoredMaterial(Color color)
    {
        StandardMaterial3D material = new StandardMaterial3D();
        material.AlbedoColor = color;
        return material;
    }

    // Function to create a transparent colored material
    // Color Alpha value sets the extent of transparency
    public static StandardMaterial3D TransparentColoredMaterial(Color color)
    {
        StandardMaterial3D material = new StandardMaterial3D();
        material.AlbedoColor        = color;
        material.Transparency       = BaseMaterial3D.TransparencyEnum.Alpha;
        return material;
    }

    // Apply a wireframe shader, to see each triangle in a mesh.
    // Usage:
    //   meshInstance.MaterialOverride = FssMaterialFactory.WireframeWhiteMaterial();
    public static ShaderMaterial WireframeWhiteMaterial()
    {
        // Load the wireframe shader
        Shader shader = (Shader)GD.Load("res://Shaders/wireframe_white_v001.gdshader");
        ShaderMaterial wireframeMaterial = new ShaderMaterial();
       // wireframeMaterial.AlbedoColor = color;
        wireframeMaterial.Shader = shader;

        return wireframeMaterial;
    }

    // --------------------------------------------------------------------------------------------
    // #MARK: Experimental / new functions
    // --------------------------------------------------------------------------------------------

    // Apply a wireframe shader, to see each triangle in a mesh.
    // Usage:
    //   meshInstance.MaterialOverride = FssMaterialFactory.WireframeShaderMaterial();
    public static ShaderMaterial WireframeShaderMaterial(Color color)
    {
        // Load the wireframe shader
        Shader shader = (Shader)GD.Load("res://Shaders/wireframe_test.gdshader");
        ShaderMaterial wireframeMaterial = new ShaderMaterial();
       // wireframeMaterial.AlbedoColor = color;
        wireframeMaterial.Shader = shader;
        return wireframeMaterial;
    }
}
