// using System;
// using System.Collections.Generic;
// using System.IO;
// using System.Threading.Tasks;

// using Godot;

// #nullable enable

// // Note that map tile nodes always hang off the EarthCoreNode parent, they never use the ZeroPoint offset.

// public partial class GloMapTileNode : Node3D
// {
//     // --------------------------------------------------------------------------------------------
//     // MARK: Final Load instantiation steps
//     // --------------------------------------------------------------------------------------------

//     private void InstatiateMesh()
//     {
//         MeshInstance = new MeshInstance3D { Name = $"{TileCode.ToString()} mesh" };
//         MeshInstance.Mesh = TileMeshData;
//         //AddChild(MeshInstance);

//         MeshInstanceW = new MeshInstance3D { Name = $"{TileCode.ToString()} wire" };
//         MeshInstanceW.Mesh = TileMeshData;
//         MeshInstanceW.MaterialOverride = GloMaterialFactory.WireframeMaterial(new Color(0f, 0f, 0f, 0.3f));
//         MeshInstance.MaterialOverride  = TileMaterial;

//         // Will be made visible when the texture is loaded
//         MeshInstance.Visible  = false;
//         MeshInstanceW.Visible = false;
//     }

//     private void AddMeshToTree()
//     {
//         AddChild(MeshInstance);

//         LabelTile(TileCode);

//         // If we are creating an initial lvl0 tile, then initialise its visible state.
//         if (TileCode.MapLvl == 0)
//         {
//             ChildrenVisibleState = false;
//             SetChildrenActive(false);
//             SetChildrenVisibility(false);
//             SetVisibility(true);
//         }
//     }

//     // --------------------------------------------------------------------------------------------
//     // MARK: Mesh
//     // --------------------------------------------------------------------------------------------

//     private void CreateMesh()
//     {
//         // Pre-requisites:
//         // - TileCode
//         // - TileEleData
//         // - UVBox
//         // - TileMaterial

//         GloMeshBuilder meshBuilder = new();

//         GloLLBox tileBounds = GloMapTileCode.LLBoxForCode(TileCode);

//         // Create the mesh
//         meshBuilder.AddSurfaceWithUVBox(
//             (float)tileBounds.MinLonDegs, (float)tileBounds.MaxLonDegs,
//             (float)tileBounds.MinLatDegs, (float)tileBounds.MaxLatDegs,
//             (float)GloWorldConsts.EarthRadiusM,
//             TileEleData, UVBox
//         );
//         meshBuilder.AddSurfaceWedgeSides(
//             (float)tileBounds.MinLonDegs, (float)tileBounds.MaxLonDegs,
//             (float)tileBounds.MinLatDegs, (float)tileBounds.MaxLatDegs,
//             (float)GloWorldConsts.EarthRadiusM, (float)GloWorldConsts.EarthRadiusM * 0.9f,
//             TileEleData
//         ); //bool flipTriangles = false)

//         TileMeshData = meshBuilder.BuildWithUV(TileCode.ToString());

//         // bool saveMesh = true;
//         // if (saveMesh)
//         // {
//         //     // Save the mesh to a file
//         //     GloMeshDataIO.WriteMeshToFile(meshBuilder.meshData, Filepaths.MeshFilepath);
//         //     GloCentralLog.AddEntry($"Saved mesh: {Filepaths.MeshFilepath}");
//         // }
//     }


// }
