using System;
using System.Collections.Generic;

using Godot;

public partial class TestLabelMaker : Node3D
{
    public const float KPixelSize = 0.0033f;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        // Distance of each label from the origin
        float distance = (float)(FssEarthCore.EarthRadiusM) + 0.1f;

        // Loop through lat and long ranges, at 10 degree intervals, and create a Label3D at each point to display on our globe
        for (int lat = 80; lat >= -80; lat -= 10)
        {
            int lonInc = 10;
            if (Math.Abs(lat) > 65) lonInc = 15;
            if (Math.Abs(lat) > 75) lonInc = 30;

            for (int lon = -180; lon < (180 - 5); lon += lonInc)
            {
                // Define the position and associated up direction for the label
                Vector3 position      = FssGeoConvOperations.RealWorldToGodot(distance, lat, lon);
                Vector3 northPosition = FssGeoConvOperations.RealWorldToGodot(distance, lat + 1, lon);
                Vector3 eastPosition  = FssGeoConvOperations.RealWorldToGodot(distance, lat, lon + 10);
                Vector3 southPosition = FssGeoConvOperations.RealWorldToGodot(distance, lat - 10, lon);
                Vector3 upDirection = (northPosition - position).Normalized();

                // find the centre of this object
                Vector3 centre = this.Position;

                Label3D label = FssLabel3DFactory.CreateLabel($"Lat {lat}\nLon {lon}", KPixelSize);

                AddChild(label);

                // Position and orient the label to face away from the center and use the up direction
                label.Position = position;
                label.LookAt(GlobalTransform.Origin, upDirection);
            }
        }
    }
}
