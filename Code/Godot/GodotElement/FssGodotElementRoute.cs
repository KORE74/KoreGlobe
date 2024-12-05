
using Godot;
using System;
using System.Collections.Generic;

public partial class FssGodotPlatformElementRoute : FssGodotPlatformElement
{
    List<FssLLAPoint>    RoutePoints = new List<FssLLAPoint>();
    List<Node3D>         RouteNodes  = new List<Node3D>();
    List<MeshInstance3D> RouteLinks  = new List<MeshInstance3D>();

    FssLineMesh3D LineMesh = new FssLineMesh3D();

    private static float BaseNodeSize = (float)(150 * FssZeroOffset.RwToGeDistMultiplier);

    // --------------------------------------------------------------------------------------------
    // MARK: Node3D Routines
    // --------------------------------------------------------------------------------------------

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        //Name     = "Route";
        //ElemType = "Route";

        // Add the child on create - every subsequent calls change it, but not its presence.
        AddChild(LineMesh);
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
        if (!Visible) return;

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
        RoutePoints.Clear();

        // Create a copy of the list to avoid external modifications affecting the internal state
        foreach (FssLLAPoint point in routePoints)
            RoutePoints.Add(point);
    }

    // Update called to keep the route in place with the zero point.
    // Iterate through the points, creating new segments as needed and moving old ones to the latest position.
    public void UpdateRoute()
    {
        LineMesh.Clear();

        double distLimitM = 10000; // 10km distance limit

        // Loop over the index values of each of point, and consider a +1 point for the end of the line
        int numNodes = RoutePoints.Count;
        for (int i = 0; i <= numNodes-2; i++)
        {
            FssLLAPoint startLLA = RoutePoints[i];
            FssLLAPoint endLLA   = RoutePoints[i+1];

            // lift any point off the zero point, so it doesn't clash with terrain
            if (FssValueUtils.EqualsWithinTolerance(startLLA.AltMslM, -0.5, 0.5)) startLLA.AltMslM = 2;
            if (FssValueUtils.EqualsWithinTolerance(  endLLA.AltMslM, -0.5, 0.5))   endLLA.AltMslM = 2;

            // get the route positions in GE units, offset from ZeroPoint
            Vector3 startPos = FssZeroOffsetOperations.RwToOffsetGe(startLLA);
            Vector3 endPos   = FssZeroOffsetOperations.RwToOffsetGe(endLLA);

            // Drop the start point line - so we see in anchored to a point in th ground.
            FssLLAPoint botLLA = RoutePoints[i];
            botLLA.AltMslM = -1000;
            Vector3 topPos = startPos;
            Vector3 botPos = FssZeroOffsetOperations.RwToOffsetGe(botLLA);
            LineMesh.AddLine(topPos, botPos, FssColorUtil.Colors["Gray"]);

            // Anything over a distance threshold, divide up into sublines to interpolate along
            // (otherwise long routes don't respect Earth curvature, a sea-routes will cut through ground)
            double legDistM = startLLA.StraightLineDistanceToM(endLLA);
            if (legDistM > distLimitM)
            {
                int numSubLines = (int)(legDistM / distLimitM);
                if (numSubLines < 4) numSubLines = 4;

                List<FssLLAPoint> pointList = FssLLAPointOperations.DividedRhumbLine(startLLA, endLLA, numSubLines);

                for (int subLegCount=0; subLegCount < pointList.Count-1; subLegCount++)
                {
                    Vector3 subStartPos = FssZeroOffsetOperations.RwToOffsetGe(pointList[subLegCount]);
                    Vector3 subEndPos   = FssZeroOffsetOperations.RwToOffsetGe(pointList[subLegCount+1]);
                    LineMesh.AddLine(subStartPos, subEndPos, FssColorUtil.Colors["Green"]);
                }
            }
            else // else draw the straight line to the end point, when the distance is short
            {
                LineMesh.AddLine(startPos, endPos, FssColorUtil.Colors["Green"]);
            }

            // if last point, add the dropdown line to the end point
            if (i == numNodes-2)
            {
                botLLA = RoutePoints[i+1];
                botLLA.AltMslM = -1000;
                topPos = endPos;
                botPos = FssZeroOffsetOperations.RwToOffsetGe(botLLA);
                LineMesh.AddLine(topPos, botPos, FssColorUtil.Colors["Gray"]);
            }
        }
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Set Visibility
    // --------------------------------------------------------------------------------------------

    public void SetVisibility(bool visible)
    {
        Visible = visible;
        LineMesh.Visible = visible;
    }

}
