using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

using Godot;

#nullable enable

// ZeroNode map tile:
// - A tile placed at an offset from the zeronode.
// - Orientation is zero, with the onus on the central point to angle it according to its LL.

public partial class GloZeroNodeMapTile : Node3D
{
    // --------------------------------------------------------------------------------------------
    // MARK: Visibility
    // --------------------------------------------------------------------------------------------

    // Applying each stage of the visibility rules to the tile and its children, split out for clarity
    // and to ease debugging and development steps.

    // Each function checks the current state, and if it is not the desired state, sets it.

    private void SetVisibility(bool visible)
    {
        if (VisibleState != visible)
        {
            VisibleState = visible;
            //GD.Print($"Setting visibility for {TileCode} to {visible}");

            bool showDebug = GloGodotFactory.Instance.UIState.ShowTileInfo  && visible;

            if (MeshInstance  != null) MeshInstance.Visible  = visible;
            if (MeshInstanceW != null) MeshInstanceW.Visible = showDebug;
            if (TileCodeLabel != null) TileCodeLabel.Visible = showDebug;
        }

        if (!visible)
        {
            if (MeshInstance  != null) MeshInstance.Visible  = false;
            if (MeshInstanceW != null) MeshInstanceW.Visible = false;
            if (TileCodeLabel != null) TileCodeLabel.Visible = false;
        }
    }

    // --------------------------------------------------------------------------------------------

    private void SetChildrenVisibility(bool visible)
    {
        //GD.Print($"{TileCode} - {VisibleState} {visible}");

        foreach (GloZeroNodeMapTile currTile in ChildTiles)
        {
            currTile.SetVisibility(visible);

            // cascade if hiding children
            if (!visible)
                currTile.SetChildrenVisibility(false);
        }
    }

    private void SetChildrenActive(bool active)
    {
        foreach (GloZeroNodeMapTile currTile in ChildTiles)
        {
            currTile.ActiveVisibility = active;

            // cascade if deactivating children
            if (!active)
                currTile.SetChildrenActive(false);
        }
    }

    // --------------------------------------------------------------------------------------------

    public void UpdateInfoVisibility(bool infoVisible)
    {
        // If the tile is not visible, there is no action to progress here
        if (!ActiveVisibility)
            return;

        if (!VisibleState)
            return;

        //GD.Print($"UpdateInfoVisibility: {TileCode} {infoVisible}");

        if (MeshInstanceW != null) MeshInstanceW.Visible = infoVisible;
        if (TileCodeLabel != null) TileCodeLabel.Visible = infoVisible;
    }

    public void UpdateChildInfoVisibility(bool infoVisible, bool meshVisible = true, bool labelVisible = true)
    {
        // If the tile is not visible, there is no action to progress here
        if (!ActiveVisibility)
            return;

        foreach (GloZeroNodeMapTile currTile in ChildTiles)
        {
            currTile.UpdateInfoVisibility(infoVisible);
            currTile.UpdateChildInfoVisibility(infoVisible);
        }
    }

    // --------------------------------------------------------------------------------------------

    private async Task BackgroundProcessing()
    {
        try
        {
            bool liveTile = true;
            while(liveTile)
            {
                GloZeroTileVisibilityStats newStats = new GloZeroTileVisibilityStats();

                if ((ActiveVisibility) && (ConstructionComplete))
                {
                    newStats.distanceToTileCenterM = (float)(GloZeroNodeMapManager.LoadRefXYZ.DistanceTo(RwTileCenterXYZ));
                    newStats.distanceFraction      = (float)(newStats.distanceToTileCenterM / GloWorldConsts.EarthRadiusM);

                    TileVisibilityStats.LatestValue = newStats;
                }

                // Yield and delay for the next cycle
                await Task.Yield();
                await Task.Delay(200); // 200ms = 0.2s

                // Check if 'this' object is still valid
                if (this == null)
                    liveTile = false;
            }
        }
        catch (Exception e)
        {
            GD.Print($"BackgroundProcessing: {e.Message}");
        }
    }

    // --------------------------------------------------------------------------------------------

    // ActiveVisibility: Means active within the "Towers of Hanoi" tree of tiles, but not necessarily
    //                   visible, its children may be visible. This flag means its assessed

    private void UpdateVisbilityRules()
    {
        if (ActiveVisibility)
        {
            // Get the latest background stats (Endeavours to offload effort to background threads)
            GloZeroTileVisibilityStats latestStats = TileVisibilityStats.LatestValue;
            float distanceToTileCenterM = latestStats.distanceToTileCenterM;
            float distanceFraction      = latestStats.distanceFraction;

            if (GloZeroNodeMapManager.DistanceToHorizonM < 10000) GloZeroNodeMapManager.DistanceToHorizonM = 10000; // minimise value at 10km

            // The minimum view distance is at least the display distance for the tile level.
            double minViewDist = ChildTileDisplayMForLvl[TileCode.MapLvl];

            int maxMapLvl = GloZeroNodeMapManager.CurrMaxMapLvl;

            // The logic could get complex, so factored it all out into a set of statement flags.
            bool withinChildDisplayDistance = distanceFraction < ChildTileDisplayForLvl[TileCode.MapLvl];
            bool withinChildCreateDistance  = distanceFraction < CreateChildTilesForLvl[TileCode.MapLvl];
            bool beyondChildDeleteDistance  = distanceFraction > DeleteChildTilesForLvl[TileCode.MapLvl];
            bool tileOverHorizon            = distanceToTileCenterM > (GloZeroNodeMapManager.DistanceToHorizonM * 2);
            bool tileInMinViewDistance      = distanceToTileCenterM < (minViewDist * 1.5);

            // A child tile will be beyond max level when we are at max level
            bool childTileBeyondMaxLvl      = TileCode.MapLvl >= maxMapLvl;

            bool childTilesExist            = DoChildTilesExist();
            bool childTilesLoaded           = AreChildTilesLoaded();

            bool shouldDisplaySelf          = true; //(!tileOverHorizon) || (tileInMinViewDistance);
            bool shouldCreateChildTiles     = withinChildCreateDistance && ConstructionComplete && !childTilesExist && (TileCode.MapLvl < maxMapLvl) && ChildTileDataAvailable;
            bool shouldDisplayChildTiles    = withinChildDisplayDistance && childTilesLoaded;
            bool shouldDeleteChildTiles     = childTilesLoaded && (beyondChildDeleteDistance || childTileBeyondMaxLvl);

            bool hideSelfEvent              = (!shouldDisplayChildTiles) && !shouldDisplaySelf && VisibleState;
            bool showSelfEvent              = (!shouldDisplayChildTiles) && shouldDisplaySelf && !VisibleState;
            bool displayChildTileEvent      = shouldDisplayChildTiles && !ChildrenVisibleState;
            bool hideChildTileEvent         = !shouldDisplayChildTiles && ChildrenVisibleState;

            // Events to hide/show self to improve performance. Over the horizon tiles are not visible etc.
            if (hideSelfEvent) SetVisibility(false);
            if (showSelfEvent) SetVisibility(true);

            // If we should create child tiles, and they don't exist, create them, kick off the background processing
            if (shouldCreateChildTiles)
            {
                //CreateSubtileNodes();
                CallDeferred(nameof(DeferredCreateSubtileNodes)); // call the creation deferred, to avoid blocking the main thread.
            }

            // If we should display child tiles, then make ourselves invisible, and make the children visible.
            if (displayChildTileEvent)
            {
                if (!ChildrenVisibleState)
                {
                    ChildrenVisibleState = true;
                    SetChildrenActive(true);
                    SetChildrenVisibility(true);
                    SetVisibility(false);
                }
            }
            if (hideChildTileEvent)
            {
                if (ChildrenVisibleState)
                {
                    ChildrenVisibleState = false;
                    SetChildrenActive(false);
                    SetChildrenVisibility(false);
                    SetVisibility(true);
                }
            }

            if (shouldDeleteChildTiles)
            {
                GD.Print($"Deleting child tiles: {TileCode}");

                SetChildrenActive(false);
                SetChildrenVisibility(false);
                SetVisibility(true);

                DeleteSubtileNodes();
            }
        }
        else
        {
            // Not active state - hide tile.
            SetVisibility(false);
        }
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Create Subtile Nodes
    // --------------------------------------------------------------------------------------------

    // Create the child tile nodes, if they do not exist. The first stage of any tile creation.

    private void DeferredCreateSubtileNodes()
    {
        CreateSubtileNodes();
    }

    // --------------------------------------------------------------------------------------------

    private void CreateSubtileNodes()
    {
        if (!TileCode.IsValid())
        {
            GloCentralLog.AddEntry($"Invalid parent tile code: {TileCode}");
            return;
        }

        // Compile the list of child node names - the lat/log boxes of the child tiles can all be driven from the code.
        List<GloMapTileCode> childTileCodes = TileCode.ChildCodesList();

        // Validate the list - we wouldn't create them on too low a level, or if the parent tile is invalid.
        if ((childTileCodes == null) || (childTileCodes.Count == 0))
        {
            GloCentralLog.AddEntry($"No child tile codes for: {TileCode}");
            return;
        }

        // If the current (parent) tile/node is not in the tree, we have some orphaning tile error to catch.
        if (!IsInsideTree())
        {
            GloCentralLog.AddEntry($"Node not in tree: {TileCode}");
            return;
        }

        // Loop through the list of child node names, and create a new node for each one, if it does not exist.
        foreach (GloMapTileCode currTileCode in childTileCodes)
        {
            string tileName = currTileCode.ToString();

            // Check if the node already exists - prevent duplicates
            if (HasNode(tileName)) continue;

            // Create a new node
            GloZeroNodeMapTile childTile   = new GloZeroNodeMapTile(currTileCode);
            childTile.ParentTile           = this;
            childTile.ActiveVisibility     = false;

            // Add the new child node to the parent and overall scene tree
            AddChild(childTile);

            // Add child nodes to our own list, will be quicker to iterate through our own list than the tree.
            ChildTiles.Add(childTile);
        }
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Delete Subtiles
    // --------------------------------------------------------------------------------------------

    // Cascades - so we don't get an inconsistency in the tree on a mid-tree node getting serviced before the leaf node.
    void DeleteSubtileNodes()
    {
        // Assume visibility is already set to false
        foreach (GloZeroNodeMapTile currTile in ChildTiles)
        {
            // Cascade any deletion
            currTile.DeleteSubtileNodes();

            // Remove the child tile from the tree and free it - actioned at the end of frame
            currTile.QueueFree();
        }

        // Clear the child tile list
        ChildTiles.Clear();
    }

}
