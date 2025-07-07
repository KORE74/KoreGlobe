
using Godot;
using System;
using System.Collections.Generic;

public partial class GloGodotPlatformElementRoute : GloGodotPlatformElement
{
    List<GloLLAPoint>    RoutePoints = new List<GloLLAPoint>();
    List<Node3D>         RouteNodes  = new List<Node3D>();
    List<MeshInstance3D> RouteLinks  = new List<MeshInstance3D>();

    GloLineMesh3D LineMesh   = new GloLineMesh3D();
    GloLineMesh3D GCLineMesh = new GloLineMesh3D();

    private Color LineColor;
    private Color LineColor2;
    private Color LineVertColor;

    private static float BaseNodeSize = (float)(150 * GloZeroOffset.RwToGeDistanceMultiplier);

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
        AddChild(GCLineMesh);

        LineColor     = GloColorUtil.LookupColor("Green", 0.1f);
        LineColor2    = GloColorUtil.LookupColor("Green", 0.8f);
        LineVertColor = GloColorUtil.LookupColor("Grey",  0.2f);
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
        if (!Visible) return;

        if (GloZeroNode.ZeroNodeUpdateCycle)
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

    public void SetRoutePoints(List<GloLLAPoint> routePoints)
    {
        RoutePoints.Clear();

        // Create a copy of the list to avoid external modifications affecting the internal state
        foreach (GloLLAPoint point in routePoints)
            RoutePoints.Add(point);
    }

    // Update called to keep the route in place with the zero point.
    // Iterate through the points, creating new segments as needed and moving old ones to the latest position.
    public void UpdateRoute()
    {
        LineMesh.Clear();
        GCLineMesh.Clear();

        double distLimitM = 1000; // 1km distance limit

        // Loop over the index values of each of point, and consider a +1 point for the end of the line
        int numNodes = RoutePoints.Count;
        for (int i = 0; i <= numNodes-2; i++)
        {
            GloLLAPoint startLLA = RoutePoints[i];
            GloLLAPoint endLLA   = RoutePoints[i+1];

            // Loop through the points manually, as we need that +1 for the next point.
            // List<GloLLAPoint> gcPoints = GloGCPathOperations.PointsOnGCPath2(startLLA, endLLA, 10);
            // for (int gcIndex = 0; gcIndex < gcPoints.Count - 1; gcIndex++)
            // {
            //     Vector3 gcPos     = GloGeoConvOperations.RwToOffsetGe(gcPoints[gcIndex]);
            //     Vector3 gcNextPos = GloGeoConvOperations.RwToOffsetGe(gcPoints[gcIndex+1]);
            //     GCLineMesh.AddLine(gcPos, gcNextPos, GloColorUtil.Colors["Red"]);
            // }

            // lift any point off the zero point, so it doesn't clash with terrain
            if (GloValueUtils.EqualsWithinTolerance(startLLA.AltMslM, -0.5, 0.5)) startLLA.AltMslM = 2;
            if (GloValueUtils.EqualsWithinTolerance(  endLLA.AltMslM, -0.5, 0.5))   endLLA.AltMslM = 2;

            // get the route positions in GE units, offset from ZeroPoint
            Vector3 startPos = GloGeoConvOperations.RwToOffsetGe(startLLA);
            Vector3 endPos   = GloGeoConvOperations.RwToOffsetGe(endLLA);

            // Drop the start point line - so we see in anchored to a point in th ground.
            GloLLAPoint botLLA = RoutePoints[i];
            botLLA.AltMslM = -1000;
            Vector3 topPos = startPos;
            Vector3 botPos = GloGeoConvOperations.RwToOffsetGe(botLLA);
            LineMesh.AddLine(topPos, botPos, LineVertColor);

            // Anything over a distance threshold, divide up into sublines to interpolate along
            // (otherwise long routes don't respect Earth curvature, a sea-routes will cut through ground)
            double legDistM = startLLA.StraightLineDistanceToM(endLLA);
            if (legDistM > distLimitM)
            {
                int numSubLines = (int)(legDistM / distLimitM);
                if (numSubLines < 4) numSubLines = 4;

                List<GloLLAPoint> pointList = GloLLAPointOperations.DividedGCLine(startLLA, endLLA, numSubLines);

                for (int subLegCount=0; subLegCount < pointList.Count-1; subLegCount++)
                {
                    Vector3 subStartPos = GloGeoConvOperations.RwToOffsetGe(pointList[subLegCount]);
                    Vector3 subEndPos   = GloGeoConvOperations.RwToOffsetGe(pointList[subLegCount+1]);
                    LineMesh.AddLine(subStartPos, subEndPos, LineColor, LineColor2);
                }
            }
            else // else draw the straight line to the end point, when the distance is short
            {
                LineMesh.AddLine(startPos, endPos, LineColor, LineColor2);
            }

            // if last point, add the dropdown line to the end point
            if (i == numNodes-2)
            {
                botLLA = RoutePoints[i+1];
                botLLA.AltMslM = -1000;
                topPos = endPos;
                botPos = GloGeoConvOperations.RwToOffsetGe(botLLA);
                LineMesh.AddLine(topPos, botPos, LineVertColor);
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
        GCLineMesh.Visible = visible;
    }

}
