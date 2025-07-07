// using System.Collections.Generic;
// using System.Collections.Concurrent;
// using System.Linq;

// public class GloMapTileList
// {
//     // --------------------------------------------------------------------------------------------
//     // MARK: Collection Management
//     // --------------------------------------------------------------------------------------------

//     // A threadsafe dictionary of all tiles, added in creation and removed on deletion
//     public ConcurrentDictionary<string, GloMapTileNode> AllTileCollection = new ConcurrentDictionary<string, GloMapTileNode>();

//     public GloMapTileList()
//     {
//     }

//     public void AddTile(GloMapTileNode tileNode)
//     {
//         string tilename = tileNode.TileCode.ToString();

//         if (!AllTileCollection.TryAdd(tilename, tileNode))
//         {
//             //GloCentralLog.AddEntry($"Failed to add tile '{tilename}': Tile already exists in the collection.");
//         }
//     }

//     public void RemoveTile(GloMapTileNode tileNode)
//     {
//         string tilename = tileNode.TileCode.ToString();

//         if (!AllTileCollection.TryRemove(tilename, out var removedTile))
//         {
//             GloCentralLog.AddEntry($"Failed to remove tile '{tilename}': Tile not found in the collection.");
//         }
//         else if (!ReferenceEquals(removedTile, tileNode))
//         {
//             GloCentralLog.AddEntry($"Warning: Removed tile '{tilename}' does not match the provided tile node.");
//         }
//     }

//     public void ClearAllTiles()
//     {
//         AllTileCollection.Clear();
//     }

//     public List<GloMapTileNode> AllTilesList()
//     {
//         if (AllTileCollection.IsEmpty)
//             return [];

//         return AllTileCollection.Values.ToList();
//     }
// }
