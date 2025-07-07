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
        float distance = (float)(GloZeroOffset.GeEarthRadius) + 0.1f;

        // Loop through lat and long ranges, at 10 degree intervals, and create a Label3D at each point to display on our globe
        for (int lat = 80; lat >= -80; lat -= 10)
        {
            int lonInc = 10;
            if (Math.Abs(lat) > 65) lonInc = 15;
            if (Math.Abs(lat) > 75) lonInc = 30;

            for (int lon = -180; lon < (180 - 5); lon += lonInc)
            {
                // Determine the positions and orientation
                GloLLAPoint pos  = new GloLLAPoint() { LatDegs = lat, LonDegs = lon, RadiusM = GloZeroOffset.GeEarthRadius };
                GloLLAPoint posN = new GloLLAPoint() { LatDegs = lat + 0.01, LonDegs = lon, RadiusM = GloZeroOffset.GeEarthRadius };

                Godot.Vector3 v3Pos   = GloGeoConvOperations.RwToGe(pos);
                Godot.Vector3 v3PosN  = GloGeoConvOperations.RwToGe(posN);
                Godot.Vector3 v3VectN = v3PosN - v3Pos;

                // Create the label
                Label3D label = GloLabel3DFactory.CreateLabel($"Lat {lat}\nLon {lon}", KPixelSize);
                AddChild(label);

                // Position and orient the label to face away from the center and use the up direction
                label.Position = v3Pos;
                label.LookAt(GlobalTransform.Origin, v3VectN);
            }
        }
    }
}
