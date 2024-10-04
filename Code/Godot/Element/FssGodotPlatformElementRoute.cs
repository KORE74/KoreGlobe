
using Godot;
using System;
using System.Collections.Generic;

public partial class FssGodotPlatformElementRoute : FssGodotPlatformElement
{
    List<FssLLAPoint>    RoutePoints = new List<FssLLAPoint>();
    List<Node3D>         RouteNodes  = new List<Node3D>();
    List<MeshInstance3D> RouteLinks  = new List<MeshInstance3D>();

    FssLineMesh3D LineMesh = new FssLineMesh3D();

    private static float BaseNodeSize = (float)(150 * FssZeroOffset.RwToGeDistanceMultiplierM);

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        //Name     = "Route";
        //ElemType = "Route";
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
        Name = $"{platformName}_RouteRoute";
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

        int numNodes = RoutePoints.Count;

        for (int i = 0; i < numNodes-1; i++)
        {
            FssLLAPoint startLLA = RoutePoints[i];
            FssLLAPoint endLLA   = RoutePoints[i+1];

            // lift any point off the zero point, so it doesn't clash with terrain
            if (FssValueUtils.EqualsWithinTolerance(startLLA.AltMslM, 0, 0.5)) startLLA.AltMslM = 2;
            if (FssValueUtils.EqualsWithinTolerance(  endLLA.AltMslM, 0, 0.5))   endLLA.AltMslM = 2;

            Vector3     startPos = FssGeoConvOperations.RwToOffsetGe(startLLA);
            Vector3     endPos   = FssGeoConvOperations.RwToOffsetGe(endLLA);

            // Drop the start point line:
            FssLLAPoint botLLA = RoutePoints[i];
            botLLA.AltMslM = -1000;
            Vector3 topPos = startPos;
            Vector3 botPos = FssGeoConvOperations.RwToOffsetGe(botLLA);
            LineMesh.AddLine(topPos, botPos, FssColorUtil.Colors["Gray"]);

            // Anything over a threshold, divide up into sublines to interpolate along
            double legDistM = startLLA.StraightLineDistanceToM(endLLA);
            double distLimitM = 10000;
            if (legDistM > distLimitM)
            {
                int numSubLines = (int)(legDistM / distLimitM);
                if (numSubLines < 4) numSubLines = 4;

                List<FssLLAPoint> pointList = FssLLAPointOperations.DividedRhumbLine(startLLA, endLLA, numSubLines);

                for (int subLegCount=0; subLegCount < pointList.Count-1; subLegCount++)
                {
                    Vector3 subStartPos = FssGeoConvOperations.RwToOffsetGe(pointList[subLegCount]);
                    Vector3 subEndPos   = FssGeoConvOperations.RwToOffsetGe(pointList[subLegCount+1]);
                    LineMesh.AddLine(subStartPos, subEndPos, FssColorUtil.Colors["Green"]);
                }
            }
            else // else draw the straight line to the end point
            {
                LineMesh.AddLine(startPos, endPos, FssColorUtil.Colors["Green"]);
            }

            // if last point, add the dropdown line to the end point too
            if (i == numNodes-2)
            {
                botLLA = RoutePoints[i+1];
                botLLA.AltMslM = -1000;
                topPos = endPos;
                botPos = FssGeoConvOperations.RwToOffsetGe(botLLA);
                LineMesh.AddLine(topPos, botPos, FssColorUtil.Colors["Gray"]);
            }
        }
        AddChild(LineMesh);
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
