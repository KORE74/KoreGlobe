
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
    List<MeshInstance3D> RouteRods   = new List<MeshInstance3D>();

    private static float BaseNodeSize = (float)(FssZeroOffset.GeEarthRadius / 100);

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
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

        // - - - - - - - - - - - - - - - - - - - - - - - - -
        // Create the nodes

        int numNodes = routePoints.Count;
        int numLinks = RoutePoints.Count - 1;

        // Take the LLA RoutePoints, output spheres in RouteNodes.
        for (int i = 0; i < numNodes; i++)
        {
            Node3D newNode = FssPrimitiveFactory.CreateSphereNode($"Node_{i}", Vector3.Zero, BaseNodeSize, FssColorUtil.Colors["Magenta"]);
            RouteNodes.Add(newNode);
            AddChild(newNode);
        }

        // - - - - - - - - - - - - - - - - - - - - - - - - -
        // Add the vertical rods below each node

        // for (int i = 0; i < numNodes; i++)
        // {
        //     FssLLAPoint topPoint    = RoutePoints[i];
        //     FssLLAPoint bottomPoint = topPoint;
        //     topPoint.RadiusM = 1f;
        //     float height = (float)(topPoint.RadiusM - bottomPoint.RadiusM);

        //     Vector3 topV3 = FssGeoConvOperations.RwToOffsetGe(topPoint);
        //     Vector3 botV3 = FssGeoConvOperations.RwToOffsetGe(bottomPoint);

        //     FssMeshBuilder meshBuilder = new FssMeshBuilder();
        //     meshBuilder.AddCylinder(Vector3.Zero, new Vector3(0, -height, 0), 0.02f, 0.002f, 12, true);
        //     ArrayMesh meshData = meshBuilder.Build2($"rod_{i}", false);

        //     // Add the mesh to the current Node3D
        //     MeshInstance3D meshInstance   = new() { Name = $"Rod_{i}" };
        //     meshInstance.Mesh             = meshData;
        //     meshInstance.MaterialOverride = FssMaterialFactory.SimpleColoredMaterial(new Color(0.5f, 0.5f, 0.5f, 1f));
        //     RouteRods.Add(meshInstance);
        //     AddChild(meshInstance);
        // }

        // - - - - - - - - - - - - - - - - - - - - - - - - -
        // Create the links between the nodes in RouteLinks.

        float lineRadius = BaseNodeSize * 0.8f;

        for (int i = 0; i < numLinks; i++)
        {
            FssEntityV3 platformV3 = FssGeoConvOperations.RwToGeStruct(RoutePoints[i], RoutePoints[i+1]);

            Vector3 vecToDest = platformV3.PosAhead - platformV3.Pos;

            float length = vecToDest.Length();
            Vector3 cylinderforwardvec = new Vector3(0, 0, length);

            FssMeshBuilder meshBuilder = new FssMeshBuilder();
            meshBuilder.AddCylinder(Vector3.Zero, cylinderforwardvec, lineRadius, lineRadius, 12, true);
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

        // for (int i = 0; i < numNodes; i++)
        // {
        //     FssLLAPoint topPoint = RoutePoints[i];
        //     if (i < numNodes)
        //     {
        //         FssLLAPoint toPoint = RoutePoints[i+1];

        //         FssEntityV3 rodV3 = FssGeoConvOperations.RwToGeStruct(topPoint, toPoint);


        //         RouteRods[i].LookAtFromPosition(
        //             rodV3.Pos,
        //             rodV3.PosAbove,
        //             rodV3.VecUp,
        //             true);


        //     }

        //     // Vector3 topV3 = FssGeoConvOperations.RwToOffsetGe(topPoint);
        //     // Vector3 botV3 = FssGeoConvOperations.RwToOffsetGe(bottomPoint);


        // }


        for (int i=0; i<numLinks; i++)
        {
            FssEntityV3 platformV3 = FssGeoConvOperations.RwToGeStruct(RoutePoints[i], RoutePoints[i+1]);

            RouteLinks[i].LookAtFromPosition(
                platformV3.Pos,
                platformV3.PosAhead,
                platformV3.VecUp,
                true);

        }

    }

}
