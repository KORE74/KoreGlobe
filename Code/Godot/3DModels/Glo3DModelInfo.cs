
using Godot;

// A class to hold 3D model info in the library and in JSON files.

public class Glo3DModelInfo
{
    public string    Name         { get; set; }
    public string    FilePath     { get; set; }
    public GloXYZBox RwAABB       { get; set; }
    public Vector3   CenterOffset { get; set; }
    public Vector3   RotateDegs   { get; set; }
    public float     Scale        { get; set; }
    public float     ModelScale   { get; set; }

    // --------------------------------------------------------------------------------------------

    public override string ToString()
    {
        return $"Name:{Name}, FilePath:{FilePath}, RwAABB:{RwAABB}, CenterOffset:{CenterOffset}, Scale:{Scale}";
    }

    // --------------------------------------------------------------------------------------------

    public static Glo3DModelInfo Default()
    {
        return new Glo3DModelInfo
        {
            Name         = "Default",
            FilePath     = "Default.glb",
            RwAABB       = new GloXYZBox() { Center = GloXYZPoint.Zero, Width = 50, Height = 20, Length = 40 },
            CenterOffset = new Vector3(0, 0, 0),
            RotateDegs   = new Vector3(0, 0, 0),
            Scale        = 1.0f,
            ModelScale   = 1.0f
        };
    }
}