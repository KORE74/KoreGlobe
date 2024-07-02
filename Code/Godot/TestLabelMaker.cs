using Godot;
using System.Collections.Generic;

public partial class TestLabelMaker : Node3D
{
    public const float KPixelSize = 0.0003f;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        // Distance of each label from the origin
        float distance = 1.221f;

        // Loop through lat and long ranges, at 10 degree intervals, and create a Label3D at each point to display on our globe
        for (int lat = 80; lat >= -80; lat -= 10)
        {
            for (int lon = 0; lon <= 360; lon += 10)
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


                // Create a small sphere at the north position for visual verification
                //CreateSphere(northPosition, 0.0025f);
                //CreateSphere(position, 0.0025f);
                CreateCylinder(position, eastPosition, 0.0025f);

                //if (lat > -80) // Avoid creating a cylinder at the poles where we don't have a point to connect to
                //{
                //    CreateCylinder(position, southPosition, 0.0025f);
                //}
            }
        }
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
        cylinder.MaterialOverride = FssMaterialFactory.SimpleColoredMaterial(new Color(1.0f, 0.5f, 0.5f, 1.0f));

        Transform3D cylinderTransform = new Transform3D();
        cylinderTransform.Origin = (start + end) / 2.0f;

        // Create basis where the z-axis points from start to end
        Vector3 zAxis = direction;
        Vector3 xAxis = Vector3.Up.Cross(zAxis).Normalized();
        if (xAxis == Vector3.Zero)
        {
            xAxis = Vector3.Right;
        }
        Vector3 yAxis = zAxis.Cross(xAxis).Normalized();

        cylinderTransform.Basis = new Basis(Vector3.Right, Vector3.Up, direction);


        cylinder.Transform = cylinderTransform;
        AddChild(cylinder);
    }
}