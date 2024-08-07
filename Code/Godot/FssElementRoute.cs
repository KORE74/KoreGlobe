
using Godot;
using System;
using System.Collections.Generic;

// Create a contrail of points for a platform.
// Node rooted off of the zero point, not moving with the platform.

public partial class FssElementRoute : Node3D
{
    List<FssLLAPoint>    RoutePoints = new List<FssLLAPoint>();
    List<Node3D>         RouteNodes  = new List<Node3D>();
    List<MeshInstance3D> RouteLinks  = new List<MeshInstance3D>();

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        // Init materials
        // matColorRed  = FssMaterialFactory.SimpleColoredMaterial(new Color(0.9f, 0.3f, 0.3f, 1f));
        // matColorBlue = FssMaterialFactory.SimpleColoredMaterial(new Color(0.3f, 0.3f, 0.9f, 1f));
        // matWire      = FssMaterialFactory.WireframeWhiteMaterial();
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
        UpdateRoute();
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Create
    // --------------------------------------------------------------------------------------------

    public void InitElement(string platformName)
    {
        // *this* is a node added to the ZeroPoint
        Name = $"{platformName}-RouteRoot";
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Update
    // --------------------------------------------------------------------------------------------

    public void SetRoutePoints(List<FssLLAPoint> routePoints)
    {
        // Create a copy of the list to avoid external modifications affecting the internal state
        foreach (FssLLAPoint point in routePoints)
            RoutePoints.Add(point);

        int numNodes = routePoints.Count;
        int numLinks = RoutePoints.Count - 1;

        // Take the LLA RoutePoints, output spheres in RouteNodes.
        for (int i = 0; i < numNodes; i++)
        {
            Node3D newNode = FssPrimitiveFactory.CreateSphereNode($"Node_{i}", Vector3.Zero, 0.025f, FssColorUtil.Colors["Magenta"]);
            RouteNodes.Add(newNode);
            AddChild(newNode);
        }

        // Create the links between the nodes in RouteLinks.
        for (int i = 0; i < numLinks; i++)
        {
            FssEntityV3 platformV3 = FssGeoConvOperations.RwToGeStruct(RoutePoints[i], RoutePoints[i+1]);

            Vector3 vecToDest = platformV3.PosAhead - platformV3.Pos;

            float length = vecToDest.Length();
            Vector3 cylinderforwardvec = new Vector3(0, 0, length);

            FssMeshBuilder meshBuilder = new FssMeshBuilder();
            meshBuilder.AddCylinder(Vector3.Zero, cylinderforwardvec, 0.02f, 0.002f, 12, true);
            ArrayMesh meshData = meshBuilder.Build2($"cylinder_{i}", false);

            // Add the mesh to the current Node3D
            MeshInstance3D meshInstance   = new() { Name = $"Link_{i}" };
            meshInstance.Mesh             = meshData;
            meshInstance.MaterialOverride = FssMaterialFactory.SimpleColoredMaterial(new Color(0.5f, 0.5f, 0.5f, 1f));
            RouteLinks.Add(meshInstance);
            AddChild(meshInstance);

            meshInstance.Position = platformV3.Pos;
        }


    }

    // Update called to keep the route in place with the zero point.
    // Iterate through the points, creating new segments as needed and moving old ones to the latest position.
    public void UpdateRoute()
    {
        int numNodes  = RouteNodes.Count;
        int numLinks  = RouteLinks.Count;

        // Now we have the balance, loop through the points and update the nodes
        for (int i = 0; i < numNodes; i++)
        {
            // Update the node position
            RouteNodes[i].Position = FssGeoConvOperations.RwToOffsetGe(RoutePoints[i]);
        }




        // for (int i=0; i<numLinks; i++)
        // {
        //     FssEntityV3 platformV3 = FssGeoConvOperations.RwToGeStruct(RoutePoints[i], RoutePoints[i+1]);

        //     Vector3 p1 = RouteNodes[i].Position;
        //     Vector3 p2 = RouteNodes[i+1].Position;
        //     Vector3 vecToDest = p2 - p1;

        // //     // Vector3 fromPoint = FssZeroOffset.GeZeroPointOffset(RoutePoints[i].ToXYZ());
        // //     // Vector3 toPoint   = FssZeroOffset.GeZeroPointOffset(RoutePoints[i+1].ToXYZ());

        //     RouteLinks[i].Position = p1;
        //     RouteLinks[i].LookAt(p1, Vector3.Up);
        // }









        for (int i=0; i<numLinks; i++)
        {
            FssEntityV3 platformV3 = FssGeoConvOperations.RwToGeStruct(RoutePoints[i], RoutePoints[i+1]);

        //     Vector3 p1 = platformV3.Pos;
        //     Vector3 p2 = platformV3.PosAhead;

        //     Vector3 vecToDest = p2 - p1;

        //     Vector3 vecUp = platformV3.VecUp;

        // //     // Vector3 fromPoint = FssZeroOffset.GeZeroPointOffset(RoutePoints[i].ToXYZ());
        // //     // Vector3 toPoint   = FssZeroOffset.GeZeroPointOffset(RoutePoints[i+1].ToXYZ());

        //     RouteLinks[i].Position = p1;
        //     RouteLinks[i].LookAt(p2, vecUp);



            RouteLinks[i].LookAtFromPosition(
                platformV3.Pos,
                platformV3.PosAhead,
                platformV3.VecUp,
                true);

        }







    }

}
