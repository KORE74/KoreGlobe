using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Godot;

#nullable enable

// ZeroNode map tile:
// - A tile placed at an offset from the zeronode.
// - Orientation is zero, with the onus on the central point to angle it according to its LL.

public partial class KoreZeroNodeMapTile : Node3D
{
    // --------------------------------------------------------------------------------------------
    // MARK: Background
    // --------------------------------------------------------------------------------------------

    // Inputs being the RwAzElBox and TileEleData, we create a mesh that represents the tile.
    // Context being the RwtoGe scaling factor.

    public void CreateMeshPoints()
    {
        // Define zero longitude center, so we can create the tile from relative (not absolute) angles and
        // more intuitively rotate the tile to the absolute longitude later.
        GloLLAPoint rwLLAZeroLonCenter = new GloLLAPoint() {
            LatDegs = RwTileCenterLLA.LatDegs,
            LonDegs = 0,
            RadiusM = GloWorldConsts.EarthRadiusM};
        GloXYZPoint rwXYZZeroLonCenter = rwLLAZeroLonCenter.ToXYZ();

        // Setup the loop control values
        int          pointCountLon   = TileEleData.Width;
        int          pointCountLat   = TileEleData.Height;
        List<double> lonZeroListRads = GloValueUtils.CreateRangeList(pointCountLon, RwTileLLBox.HalfDeltaLonRads, -RwTileLLBox.HalfDeltaLonRads); // Relative azimuth
        List<double> latListRads     = GloValueUtils.CreateRangeList(pointCountLat, RwTileLLBox.MaxLatRads,        RwTileLLBox.MinLatRads);

        // Simplicity: Create 2 x 2D arrays for the top and bottom of the tile. We'll only use the edges of the bottom.
        v3Data       = new Vector3[pointCountLon, pointCountLat];
        v3DataBottom = new Vector3[pointCountLon, pointCountLat];

        for (int ix = 0; ix < pointCountLon; ix++)
        {
            // Create limit working variables, so we know when to populate the bottom/edge array.
            bool limitX = (ix == 0) || (ix == pointCountLon - 1);

            for (int jy = 0; jy < pointCountLat; jy++)
            {
                bool limitY = (jy == 0) || (jy == pointCountLat - 1);

                // Find the Real-World (RW) position for each point in the mesh.
                double lonRads = lonZeroListRads[ix];
                double latRads = latListRads[jy];
                double ele     = TileEleData[ix, jy];

                // Determine the tile position in the RW world, and then as an offset from the tile centre
                GloLLAPoint  rwLLAPointPos     = new GloLLAPoint() { LatRads = latRads, LonRads = lonRads, AltMslM = ele };
                GloXYZPoint  rwXYZPointPos     = rwLLAPointPos.ToXYZ();
                GloXYZVector rwXYZCenterOffset = rwXYZZeroLonCenter.XYZTo(rwXYZPointPos);

                // Convert the Real-World position to the Game Engine position.
                v3Data[ix, jy] = new Vector3(
                    (float)(rwXYZCenterOffset.X * KoreZeroOffset.RwToGeDistanceMultiplier),
                    (float)(rwXYZCenterOffset.Y * KoreZeroOffset.RwToGeDistanceMultiplier),
                    (float)(rwXYZCenterOffset.Z * KoreZeroOffset.RwToGeDistanceMultiplier));

                if (limitX || limitY) // Only do the edges, we don't use the middle.
                {
                    // Determine the tile position in the RW world, and then as an offset from the tile centre
                    GloLLAPoint  rwLLABottomPos    = new GloLLAPoint() { LatRads = latRads, LonRads = lonRads, AltMslM = -1000 };
                    GloXYZPoint  rwXYZBottomPos    = rwLLABottomPos.ToXYZ();
                    GloXYZVector rwXYZBottomOffset = rwXYZZeroLonCenter.XYZTo(rwXYZBottomPos);

                    // Convert the Real-World position to the Game Engine position.
                    v3DataBottom[ix, jy] = new Vector3(
                        (float)(rwXYZBottomOffset.X * KoreZeroOffset.RwToGeDistanceMultiplier),
                        (float)(rwXYZBottomOffset.Y * KoreZeroOffset.RwToGeDistanceMultiplier),
                        (float)(rwXYZBottomOffset.Z * KoreZeroOffset.RwToGeDistanceMultiplier));
                }
            }
        }

        // Determine the label position while we're still in background processing functions, and then
        // consume it when in the _process loop.
        {
            // Determine the tile label elevation as 100m above the highest elevation in the tile.
            double tileMaxAlt = TileEleData.MaxVal();
            if (tileMaxAlt < 100) tileMaxAlt = 100;

            // Determine the label position in the RW world, and then as an offset from the tile centre
            GloLLAPoint  rwLLATileLabel       = new GloLLAPoint() { LatDegs = rwLLAZeroLonCenter.LatDegs, LonDegs = rwLLAZeroLonCenter.LonDegs, AltMslM = tileMaxAlt + 50 };

            //GloLLAPoint  rwLLATileLabel       = new GloLLAPoint() { LatDegs = RwTileCenterLLA.LatDegs, LonDegs = 0, RadiusM = GloWorldConsts.EarthRadiusM - 1000 };
            GloXYZPoint  rwXYZTileLabel       = rwLLATileLabel.ToXYZ();
            GloXYZVector rwXYZTileLabelOffset = rwXYZZeroLonCenter.XYZTo(rwXYZTileLabel);
            //GloXYZVector rwXYZTileLabelOffset = RwTileCenterXYZ.XYZTo(rwXYZTileLabel);

            // Convert the Real-World position to the Game Engine position.
            TileLabelOffset = new Vector3(
                (float)(rwXYZTileLabelOffset.X * KoreZeroOffset.RwToGeDistanceMultiplier),
                (float)(rwXYZTileLabelOffset.Y * KoreZeroOffset.RwToGeDistanceMultiplier),
                (float)(rwXYZTileLabelOffset.Z * KoreZeroOffset.RwToGeDistanceMultiplier));


            // - - - -

            GloLLAPoint rwLLATileLabelN        = rwLLATileLabel;
            rwLLATileLabelN.LatDegs            = RwTileCenterLLA.LatDegs + 0.01;
            GloXYZPoint  rwXYZTileLabelN       = rwLLATileLabelN.ToXYZ();
            GloXYZVector rwXYZTileLabelNOffset = rwXYZZeroLonCenter.XYZTo(rwXYZTileLabelN);

            TileLabelOffsetN = new Vector3(
                (float)(rwXYZTileLabelNOffset.X * KoreZeroOffset.RwToGeDistanceMultiplier),
                (float)(rwXYZTileLabelNOffset.Y * KoreZeroOffset.RwToGeDistanceMultiplier),
                (float)(rwXYZTileLabelNOffset.Z * KoreZeroOffset.RwToGeDistanceMultiplier));

            GloLLAPoint rwLLATileLabelBelow = rwLLATileLabel;
            rwLLATileLabelN.RadiusM = RwTileCenterLLA.RadiusM - 1000;
            GloXYZPoint rwXYZTileLabelBelow = rwLLATileLabelN.ToXYZ();
            GloXYZVector rwXYZTileLabelBelowOffset = rwXYZZeroLonCenter.XYZTo(rwXYZTileLabelBelow);

            TileLabelOffsetBelow = new Vector3(
                (float)(rwXYZTileLabelBelowOffset.X * KoreZeroOffset.RwToGeDistanceMultiplier),
                (float)(rwXYZTileLabelBelowOffset.Y * KoreZeroOffset.RwToGeDistanceMultiplier),
                (float)(rwXYZTileLabelBelowOffset.Z * KoreZeroOffset.RwToGeDistanceMultiplier));
        }
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Process
    // --------------------------------------------------------------------------------------------

    public void CreateMeshTileSurface()
    {
        // Rotate each tile into its position - The zero node only translates, so this is fixed after creation
        float rotAz = (float)(RwTileCenterLLA.LonRads); // We created the tile with relative azimuth, so apply the absolute value to orient it to its longitude.

        // Create the game-engine mesh from the V3s
        GloMeshBuilder meshBuilder = new();
        meshBuilder.AddSurface(v3Data, UVx, UVy, false);
        meshBuilder.AddBoxEdges(v3Data, v3DataBottom, UVx, UVy, false);

        // GloBinaryDataManager loadDB = new GloBinaryDataManager("TileGeometry2.db");
        // string tilecodestr = TileCode.ToString();

        // if (!loadDB.DataExists(tilecodestr))
        // {
        //     byte[] data = GloMeshDataIO.MeshDataToBytes2(meshBuilder.meshData);
        //     loadDB.Add(tilecodestr, data);
        // }
        // else
        // {
        //     byte[] data = loadDB.Get(tilecodestr);
        //     meshBuilder.meshData = GloMeshDataIO.BytesToMeshData2(data);
        // }

        // Build the mesh data and add it to the node
        MeshInstance      = new MeshInstance3D() { Name = "tileMesh" };
        MeshInstance.Mesh = meshBuilder.BuildWithUV("Surface");
        //MeshInstance.MaterialOverride = GloMaterialFactory.SimpleColoredMaterial(new Color(1.0f, 0.0f, 1.0f, 1.0f));
        MeshInstance.MaterialOverride = TileMaterial;

        AddChild(MeshInstance);

        MeshInstance.Rotation  = new Vector3(0, rotAz, 0);

        // Set the visibility false, later functions will assign the correct visibility rules.
        MeshInstance.Visible   = false;


        //AddChild(MeshInstance);

        GEElements.Add(MeshInstance);

    }

    // --------------------------------------------------------------------------------------------

    public void CreateMeshWireFrame()
    {
        //GloBinaryDataManager loadDB = new GloBinaryDataManager("TileGeometry.db");
        string tilecodestr = TileCode.ToString();

        // Rotate each tile into its position - The zero node only translates, so this is fixed after creation
        float rotAz = (float)(RwTileCenterLLA.LonRads); // We created the tile with relative azimuth, so apply the absolute value to orient it to its longitude.

        MeshInstanceW = new();
        MeshInstanceW.Rotation = new Vector3(0, rotAz, 0);

        // if (!loadDB.DataExists(tilecodestr))
        // {
            MeshInstanceW.AddSurface(v3Data, GloColorUtil.Colors["Grey10pct"], GloColorUtil.Colors["Grey25pct"]);
        //     loadDB.Add(tilecodestr, MeshInstanceW.ToBytes());
        // }
        // else
        // {
        //     byte[] data = loadDB.Get(tilecodestr);
        //     MeshInstanceW.FromBytes(data);
        // }

        // else
        // {

        //     if (!loadDB.DataExists(tilecodestr))
        //     {
        //         loadDB.Add(tilecodestr, MeshInstanceW.ToBytes());
        //     }
        // }

        MeshInstanceW.Visible = false;
        MeshInstanceW.Name    = "LineMesh";

        AddChild(MeshInstanceW);
    }
}