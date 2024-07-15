using Godot;
using System;

public partial class FssTestCylinder2 : Node3D
{
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        FssCentralLog.AddEntry("TestCylinder2 // _Ready");

        // Distance of each label from the origin
        float distance = 2f;

        // Loop through lat and long ranges, at 10 degree intervals, and create a Label3D at each point to display on our globe
        for (int lat = 80; lat >= -80; lat -= 10)
        {
            int latInc = 10;
            int lonInc = 10;
            if (Math.Abs(lat) > 65) lonInc = 20;
            if (Math.Abs(lat) > 75) lonInc = 40;

            for (int lon = -180; lon < (180 - 5); lon += lonInc)
            {
                // Define the position and associated up direction for the label
                Vector3 position      = FssGeoConvOperations.RealWorldToGodot(distance, lat, lon);
                Vector3 northPosition = FssGeoConvOperations.RealWorldToGodot(distance, lat + 1, lon);
                Vector3 eastPosition  = FssGeoConvOperations.RealWorldToGodot(distance, lat, lon + lonInc);
                Vector3 southPosition = FssGeoConvOperations.RealWorldToGodot(distance, lat - latInc, lon);
                Vector3 upDirection = (northPosition - position).Normalized();

                // find the centre of this object
                Vector3 centre = this.Position;

                CreateSphere(position, 0.015f);
                CreateCylinder(position, eastPosition, 0.0025f);
                CreateCylinder(position, southPosition, 0.0025f);
            }
        }
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
    }

    private void CreateSphere(Vector3 position, float radius)
    {
        MeshInstance3D sphere = new MeshInstance3D();
        sphere.Mesh = new SphereMesh
        {
            Radius = radius,
            Height = radius * 2.0f
        };
        sphere.Position = position;
        AddChild(sphere);
    }

    private void CreateCylinder(Vector3 start, Vector3 end, float radius)
    {
        Vector3 direction = end - start;
        float length = direction.Length();
        direction = direction.Normalized();

        // Midpoint
        Vector3 midPoint = (start + end) / 2.0f;

        // above the midpoint
        Vector3 aboveMidPoint = midPoint * 1.1f;

        MeshInstance3D cylinder = new MeshInstance3D();
        CylinderMesh cylinderMesh = new CylinderMesh
        {
            TopRadius = radius,
            BottomRadius = radius,
            Height = length,
            RadialSegments = 6,
            Rings = 3
        };
        cylinder.Mesh = cylinderMesh;
        cylinder.MaterialOverride = FssMaterialFactory.SimpleColoredMaterial(new Color(0.0f, 0.5f, 1.0f, 1.0f));

        AddChild(cylinder);

        cylinder.Position = midPoint;
        cylinder.LookAt(aboveMidPoint, end);
    }
}
