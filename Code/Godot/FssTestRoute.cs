using System;
using System.Collections.Generic;

using Godot;

// Class to add an entity from the PlatformModel.

public partial class FssTestRoute : Node3D
{

    private List<FssLLAPoint> RoutePoints = new List<FssLLAPoint>();

    private FssLLAPoint pos   = new FssLLAPoint() { LatDegs = 35, LonDegs = 30, AltMslM = 1.22f };
    Node3D ModelNode         = null;

    public override void _Ready()
    {

        // Create the route real world points
        RoutePoints.Add(new FssLLAPoint() { LatDegs = 35, LonDegs = 29, AltMslM = 1.24f });
        RoutePoints.Add(new FssLLAPoint() { LatDegs = 37, LonDegs = 31, AltMslM = 1.25f });
        RoutePoints.Add(new FssLLAPoint() { LatDegs = 36, LonDegs = 32, AltMslM = 1.26f });
        RoutePoints.Add(new FssLLAPoint() { LatDegs = 38, LonDegs = 35, AltMslM = 1.24f });

        // Create the Vector3 list
        List<Vector3> RouteVectors = new List<Vector3>();
        foreach (FssLLAPoint point in RoutePoints)
            RouteVectors.Add(FssGeoConvOperations.RealWorldToGodot(point));

        // Create the materials
        var matWire  = FssMaterialFactory.WireframeWhiteMaterial();
        var matGrey  = FssMaterialFactory.TransparentColoredMaterial(new Color(0.5f, 0.5f, 0.5f, 1f));


        // Create the spheres and cylinders
        for (int i = 0; i < RouteVectors.Count - 1; i++)
        {
            Vector3 point   = RouteVectors[i];
            Vector3 topoint = RouteVectors[i+1];
            Vector3 pointdiff = topoint - point;

            // Add sphere for the current point
            MeshInstance3D childSphere = FssPrimitiveFactory.CreateGodotSphere(point, 0.01f, new Color(.5f, 1f, .5f, 1f));
            AddChild(childSphere);
            childSphere.Name = $"Sphere{i:D2}";


            // MeshInstance3D pointCylinder = FssPrimitiveFactory.CreateCylinder(pointdiff, 0.005f, new Color(.4f, .4f, .4f, 1f));
            // childSphere.AddChild(pointCylinder);
            // pointCylinder.Name = $"Cylinder{i:D2}";
            // pointCylinder.Position = pointdiff / 2.0f;
            // pointCylinder.LookAt(pointdiff, Vector3.Up);


            FssMeshBuilder meshBuilder = new FssMeshBuilder();
            meshBuilder.AddCylinder(Vector3.Zero, pointdiff, 0.005f, 0.005f, 12, true);



            ArrayMesh meshData = meshBuilder.Build2("Wedge", false);

            // Add the mesh to the current Node3D
            MeshInstance3D meshInstance   = new();
            meshInstance.Mesh             = meshData;
            meshInstance.MaterialOverride = matGrey; //matTransBlue;

            // Add the mesh to the current Node3D
            MeshInstance3D meshInstanceW   = new();
            meshInstanceW.Mesh             = meshData;
            meshInstanceW.MaterialOverride = matWire; // matTestWite; //

            childSphere.AddChild(meshInstance);
            childSphere.AddChild(meshInstanceW);

            //meshBuilder.Init();
        }

    }

    public override void _Process(double delta)
    {
        // UpdateModelPosition();
    }

    public void UpdateModelPosition()
    {
        // // --- Define positions -----------------------

        // // Define the position and associated up direction for the label
        // FssLLAPoint posAbove = pos;
        // posAbove.AltMslM += 0.04f;

        // // Get the position 5 seconds ahead, or just north if stationary
        // FssLLAPoint posAhead = FssLLAPoint.Zero;

        //     posAhead = pos;
        //     posAhead.LatDegs += 0.001;

        // // --- Define vectors -----------------------

        // // Define the Vector3 Offsets
        // Vector3 vecPos   = FssGeoConvOperations.RealWorldToGodot(pos);
        // Vector3 vecAbove = FssGeoConvOperations.RealWorldToGodot(posAbove);
        // Vector3 vecAhead = FssGeoConvOperations.RealWorldToGodot(posAhead);

        // FssEntityV3 platVecs = FssGeoConvOperations.ReadWorldToStruct(pos, FssCourse.Zero);

        // // Update node position and orientation
        // ModelNode.Position = platVecs.Position;// vecPos;
        // ModelNode.LookAt(platVecs.PosAhead, platVecs.PosAbove);

        // // Update camera position and orientation
        // // FssXYZPoint camOffsetXYZ = CameraOffset.ToXYZ();
        // // ModelCamera.Position = new Vector3((float)camOffsetXYZ.X, -(float)camOffsetXYZ.Y, -(float)camOffsetXYZ.Z);
        // // ModelCamera.LookAt(vecPos, vecAbove);
    }
}
