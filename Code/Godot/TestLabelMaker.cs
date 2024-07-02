using Godot;
using System.Collections.Generic;

public partial class TestLabelMaker : Node3D
{
    public const float KPixelSize = 0.0003f;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        // Distance of each label from the origin
        float distance = 1.27f;

        // Loop through lat and long ranges, at 10 degree intervals, and create a Label3D at each point to display on our globe
        for (int lat = 80; lat >= -80; lat -= 10)
        {
            for (int lon = 0; lon <= 360; lon += 10)
            {
                Vector3 position = FssGeoConvOperations.RealWorldToGodot(distance, lat, lon);

                // Create a new Label from the FssLabelMaker util class
                Label3D label = FssLabel3DFactory.CreateLabel($"Lat {lat}\nLon {lon}", KPixelSize);

                // Set the label's position
                label.Position = position;

                // Calculate the direction vector pointing away from the center
                Vector3 direction = position.Normalized();

                // Transform3D labelTransform = label.Transform;
                // labelTransform.Basis = new Basis(Vector3.Forward, direction);
                // label.Transform = labelTransform;

                // Orient the label to face away from the center
                //label.LookAt(position);
                label.LookAt(Vector3.Zero, Vector3.Up);

                AddChild(label);
            }
        }
    }
}