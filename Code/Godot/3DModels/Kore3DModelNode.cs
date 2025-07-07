using KoreCommon;
using Godot;

// A class to hold 3D model and info in the library .

#nullable enable

public partial class Kore3DModelNode : Node3D
{
    public Kore3DModelInfo Info { get; set; } = Kore3DModelInfo.Default();
    public Node3D? Node { get; set; } = null;

    // --------------------------------------------------------------------------------------------
    // MARK: Node3D
    // --------------------------------------------------------------------------------------------

    public override void _Ready()
    {
    }

    // public override void _Process(double delta)
    // {
    // }


    // --------------------------------------------------------------------------------------------

    // Instatiate a new copy of the 3D model node, to be added somewhere in the scene tree.

    public Kore3DModelNode Clone()
    {
        Kore3DModelNode clone = new Kore3DModelNode
        {
            Info = this.Info,
            Node = this.Node?.Duplicate() as Node3D
        };
        if (clone.Node != null)
        {
            clone.Node.Name = this.Node.Name;   // Keep the original name.
        }

        return clone;
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Scale
    // --------------------------------------------------------------------------------------------


}