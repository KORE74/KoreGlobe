using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Godot;

#nullable enable

// ZeroNode map tile:
// - A tile placed at an offset from the zeronode.
// - Orientation is zero, with the onus on the central point to angle it according to its LL.

public partial class FssZeroNodeMapTile : Node3D
{
    double HorizonAngleAtCenter(double altitudeM)
    {
        double R = FssPosConsts.EarthRadiusM;
        return Math.Acos(R / (R + altitudeM));
    }


    private void UpdateVisibility()
    {
        //(bool validUnproject, float pixelsPerTriangle) = UnprojectedTriangleSize();
        //if (validUnproject) LatestPixelsPerTriangle = pixelsPerTriangle;

        // Use the camera LLA and the tile centre LLA to determine the angle between the two
        FssLLAPoint camPosLLA = FssGodotFactory.Instance.CameraMoverWorld.CamPos;
        FssLLAPoint tileCentreLLA = new FssLLAPoint(TileCode.LLBox.CenterPoint);
        double angleToCameraRads = camPosLLA.AngleToRads(tileCentreLLA);
        double angleToCameraDegs = angleToCameraRads * FssConsts.RadsToDegsMultiplier;

        double horizonEleRads = camPosLLA.HorizonElevationRads();
        double horizonEleDegs = horizonEleRads * FssConsts.RadsToDegsMultiplier;

        double horizonAngleRads = HorizonAngleAtCenter(camPosLLA.AltMslM);
        double horizonAngleDegs = horizonAngleRads * FssConsts.RadsToDegsMultiplier;

        horizonAngleDegs += TileCode.LLBox.LargestHalfDeltaDegs;
        horizonAngleDegs += 10;

        double visibleLimitDegs = FssNumericUtils<double>.Clamp(horizonAngleDegs, 10, 95);

        // --------------------------------------------------------------

        bool displayChildByDistance = false;
        bool createChildByDistance  = false;
        bool deleteChildByDistance  = false;
        double cameraDistanceM = RwXYZCenter.DistanceTo(FssGodotFactory.Instance.CameraMoverWorld.CamPosXYZ);

        double currTileChildDisplayDistM = childTileDisplayDistKmForLvl[TileCode.MapLvl] * 1000;
        double currTileChildCreateDistM  = currTileChildDisplayDistM * 1.2;
        double currTileChildDeleteDistM  = currTileChildDisplayDistM * 1.5;

        if (cameraDistanceM < currTileChildDisplayDistM) displayChildByDistance = true;





        // Flags to determine behavior
        bool displayTileByAngle = (angleToCameraDegs > visibleLimitDegs);
        bool childTile = TileCode.MapLvl > 0;

        bool createChildTiles = displayTileByAngle && !ChildTilesExist() && !ChildCreationRejected;
        bool destroyChildTiles = !displayTileByAngle & childTile;







        //GD.Print($"TileCode: {TileCode} Angle to Camera: {angleToCameraDegs:F0} // horizonEleDegs: {horizonEleDegs:F0} // CamDist: {cameraDistanceM:F0} ");

        bool angleVisible = true;
        if (angleToCameraDegs > visibleLimitDegs)
        {
            angleVisible = false;
        }

        if (!ChildrenSetVisible)
            SetVisibility(angleVisible);

        if (TileCode.MapLvl == 0)
            SetActiveState(true);

        if (ActiveState)
        {
            if (!ChildCreationRejected && createChildByDistance && !ChildTilesExist())
            {
                CreateChildTiles();
            }

            if (displayChildByDistance)
            {
                if (ChildTilesContructed())
                {
                    SetChildrenActive(true);
                    SetVisibility(false);
                }
            }
            else
            {
                SetChildrenActive(false);
                SetVisibility(true);
            }
        }
        else
        {
            SetVisibility(false);
            SetChildrenActive(false);

            if (deleteChildByDistance)
            {
                DeleteChildTiles();
            }
        }
    }

    // --------------------------------------------------------------------------------------------

    // Other functions deal with traversing trees of parent child relationships, this is just for this tile.
    private void SetVisibility(bool newVisibleState)
    {
        LineMesh3D.Visible       = newVisibleState;
        TileMeshInstance.Visible = newVisibleState;
    }

    // --------------------------------------------------------------------------------------------

    // Other functions deal with traversing trees of parent child relationships, this is just for this tile.
    private void SetActiveState(bool newActiveState)
    {
        // Ensure we are dealing with a state change on this tile
        if (ActiveState == newActiveState)
            return;

        // If changing the child to active, set it visible in the moment
        if (!ActiveState && newActiveState)
        {
            SetVisibility(true);
        }

        // If changing the child to inactive, set it invisible and cascade to any of its children
        if (ActiveState && !newActiveState)
        {
            SetVisibility(false);
        }

        ActiveState = newActiveState;
    }

    // --------------------------------------------------------------------------------------------

    // On setting a child tile active, it is immediately set to visible.
    // On setting a child tile inactive, it is immediately set to invisible, and all children are set inactive

    private void SetChildrenActive(bool newActiveState)
    {
        ChildrenSetVisible = newActiveState;

        // Ensure we have child tiles in a correct state to update
        if (ChildTilesExist() && ChildTilesContructed())
        {
            foreach (FssZeroNodeMapTile childTile in ChildTiles)
            {
                // Assign the new active state
                childTile.SetActiveState(newActiveState);
            }
        }
    }

    private void DeleteChildTiles()
    {
        if (ChildTilesExist())
        {
            foreach (FssZeroNodeMapTile childTile in ChildTiles)
            {
                childTile.DeleteChildTiles();
                RemoveChild(childTile);
                childTile.QueueFree();
            }
            ChildTiles.Clear();
        }
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Visibility Helper Routines
    // --------------------------------------------------------------------------------------------

    private (bool, float) UnprojectedTriangleSize()
    {
        bool  retValid     = false;
        float retPixelSize = 1000f; // Large initial value, will be minimized

        // Get the real-world corners of the tile's LL box
        FssLLBox   tileLLBox = TileCode.LLBox;
        FssLLPoint llaTileTL = tileLLBox.PosTopLeft;
        FssLLPoint llaTileTR = tileLLBox.PosTopRight;
        FssLLPoint llaTileBL = tileLLBox.PosBottomLeft;
        FssLLPoint llaTileBR = tileLLBox.PosBottomRight;

        // Get the game-engine version of those points
        Vector3 v3TL = FssZeroOffsetOperations.RwToOffsetGe(llaTileTL);
        Vector3 v3TR = FssZeroOffsetOperations.RwToOffsetGe(llaTileTR);
        Vector3 v3BL = FssZeroOffsetOperations.RwToOffsetGe(llaTileBL);
        Vector3 v3BR = FssZeroOffsetOperations.RwToOffsetGe(llaTileBR);

        // Get the number of triangles along the box edges
        int vertTriangleCount  = RwEleData.Height - 1;
        int horizTriangleCount = RwEleData.Width  - 1;

        // Get the current scene camera and viewport, requirements for the unproject calls.
        Viewport viewport   = GetViewport(); //Engine.GetMainLoop() is SceneTree sceneTree ? sceneTree.Root : null;
        Camera3D currCamera = viewport.GetCamera3D();

        // Unproject the tile corners into screen space
        bool unprojectTLValid, unprojectTRValid, unprojectBLValid, unprojectBRValid;
        Vector2 unprojectTL, unprojectTR, unprojectBL, unprojectBR;

        // Unproject the points, to get an XY screen position and a validity
        (unprojectTL, unprojectTLValid) = FssUnprojectUtils.UnprojectPoint(v3TL, currCamera, viewport);
        (unprojectTR, unprojectTRValid) = FssUnprojectUtils.UnprojectPoint(v3TR, currCamera, viewport);
        (unprojectBL, unprojectBLValid) = FssUnprojectUtils.UnprojectPoint(v3BL, currCamera, viewport);
        (unprojectBR, unprojectBRValid) = FssUnprojectUtils.UnprojectPoint(v3BR, currCamera, viewport);

        // Determine the validity around the edges, so we can perform an assessment even with some corners out of bounds.
        bool topEdgeValid    = unprojectTLValid && unprojectTRValid;
        bool bottomEdgeValid = unprojectBLValid && unprojectBRValid;
        bool leftEdgeValid   = unprojectTLValid && unprojectBLValid;
        bool rightEdgeValid  = unprojectTRValid && unprojectBRValid;

        // Early exit if no edges are valid
        if (!topEdgeValid && !bottomEdgeValid && !leftEdgeValid && !rightEdgeValid)
        {
            return (false, retPixelSize);
        }

        // A top or bottom edge may be compromised by being at a pole, so we need to make sure the edge has at least some distance to it.
        const float minEdgePixels = 5f;

        // Based on each edge validity, lets see what the smallest triangle-pixel-size is.
        if (topEdgeValid)
        {
            float topEdgeScreenSpace = (unprojectTL - unprojectTR).Length();
            if (topEdgeScreenSpace > minEdgePixels)
            {
                float topEdgePixelsPerTriangle = topEdgeScreenSpace / horizTriangleCount;
                retPixelSize = Math.Min(topEdgePixelsPerTriangle, retPixelSize);
                retValid = true;
            }
        }
        if (bottomEdgeValid)
        {
            float bottomEdgeScreenSpace = (unprojectBL - unprojectBR).Length();
            if (bottomEdgeScreenSpace > minEdgePixels)
            {
                float bottomEdgePixelsPerTriangle = bottomEdgeScreenSpace / horizTriangleCount;
                retPixelSize = Math.Min(bottomEdgePixelsPerTriangle, retPixelSize);
                retValid = true;
            }
        }
        if (leftEdgeValid)
        {
            float leftEdgeScreenSpace = (unprojectTL - unprojectBL).Length();
            float leftEdgePixelsPerTriangle = leftEdgeScreenSpace / vertTriangleCount;
            retPixelSize = Math.Min(leftEdgePixelsPerTriangle, retPixelSize);
            retValid = true;
        }
        if (rightEdgeValid)
        {
            float rightEdgeScreenSpace = (unprojectTR - unprojectBR).Length();
            float rightEdgePixelsPerTriangle = rightEdgeScreenSpace  / vertTriangleCount;
            retPixelSize = Math.Min(rightEdgePixelsPerTriangle, retPixelSize);
            retValid = true;
        }

        return (retValid, retPixelSize);
    }

}
