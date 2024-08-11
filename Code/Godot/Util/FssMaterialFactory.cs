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
    //   meshInstance.MaterialOverride = FssMaterialFactory.WireframeMaterial(FssColorUtil.Colors["White"]);
    public static ShaderMaterial WireframeMaterial(Color col)
    {
        // Load the wireframe shader
        Shader shader = (Shader)GD.Load("res://Shaders/wireframe_v001.gdshader");
        ShaderMaterial wireframeMaterial = new ShaderMaterial();

        wireframeMaterial.SetShaderParameter("wirecolor", col);

       // wireframeMaterial.AlbedoColor = color;
        wireframeMaterial.Shader = shader;

        return wireframeMaterial;
    }

    // --------------------------------------------------------------------------------------------
    // #MARK: Experimental / new functions
    // --------------------------------------------------------------------------------------------

    public static ShaderMaterial VertexColorMaterial()
    {
        // Load the wireframe shader
        Shader shader = (Shader)GD.Load("res://Shaders/VertexColor2.gdshader");
        ShaderMaterial material = new ShaderMaterial();
       // wireframeMaterial.AlbedoColor = color;
        material.Shader = shader;

        return material;
    }

    public static ShaderMaterial TexMaterial()
    {
        // Load the wireframe shader
        Shader shader = (Shader)GD.Load("res://Shaders/VertexColor2.gdshader");
        ShaderMaterial material = new ShaderMaterial();
       // wireframeMaterial.AlbedoColor = color;
        material.Shader = shader;

        return material;
    }

    // Usage: meshInstance.MaterialOverride = FssMaterialFactory.WaterMaterial();
    public static ShaderMaterial WaterMaterial()
    {
        // Load the .tres file
        ShaderMaterial material = (ShaderMaterial)GD.Load("res://Materials/Water_002.tres");

    //     // Load the wireframe shader
    //     Shader shader = (Shader)GD.Load("res://Shaders/water_001.gdshader");
    //     ShaderMaterial material = new ShaderMaterial();
    //    // wireframeMaterial.AlbedoColor = color;
    //     material.Shader = shader;

        return material;
    }
}
