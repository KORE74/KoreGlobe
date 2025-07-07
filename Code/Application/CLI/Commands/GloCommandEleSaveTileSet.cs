using System.Collections.Generic;
using System.Text;


// CLI Usage: ele prep <inEleFilename> <inTileCode> <inOutDir> <action>
// CLI Usage: ele prep c:/Util/GlobeLibrary_MapPrep/Europe/W005N50_UkCentral/Ele_BF_BF_50m.asc BF_BF C:/Util/_temp yes


public class GloCommandEleSaveTileSet : GloCommand
{
    public GloCommandEleSaveTileSet()
    {
        Signature.Add("ele");
        Signature.Add("savetileset");
    }

    public override string HelpString => $"{SignatureString} <tilecode> <layers>";

    public override string Execute(List<string> parameters)
    {
        StringBuilder sb = new StringBuilder();

        if (parameters.Count != 2)
        {
            return "GloCommandEleSaveTileSet.Execute -> wrong param count";
        }

        foreach (string param in parameters)
        {
            sb.AppendLine($"param: {param}");
        }

        string tileCodeStr = parameters[0];
        int    numLayers   = int.Parse(parameters[1]);

        // -------------------------------------------------

        // get the top level tile code
        GloMapTileCode tileCode = GloMapTileCode.TileCodeFromString(tileCodeStr);

        // -------------------------------------------------

        if (tileCode.IsValid())
        {
            List<GloMapTileCode> allCodesList  = new(); // Full list we'll accumulate every tile code in
            List<GloMapTileCode> prevLayerList = new(); // List of the current layer, so we can get the children by iterating over this.
            List<GloMapTileCode> nextLayerList = new();

            // 0 layers means the top level tile code only
            sb.AppendLine($"Adding stated tilecode: {tileCode}");
            allCodesList.Add(tileCode);
            prevLayerList.Add(tileCode);

            // Get the child codes for the current tile, and recurisvely create the child elevation values
            for (int i = 0; i < numLayers; i++)
            {
                // Accumulate the nextlayer codes
                foreach (GloMapTileCode currTileCode in prevLayerList)
                {
                    // Exit this inner loop if we're at the max level
                    // if (currTileCode.MapLvl >= GloMapTileCode.MaxMapLvl)
                    //     break;

                    // Add the prev layer to the full set (layers 1 onwards)
                    List<GloMapTileCode> currTileChildren = currTileCode.ChildCodesList();
                    nextLayerList.AddRange(currTileChildren);
                }

                // Expand the allCodesList with everything we found from this layer
                allCodesList.AddRange(nextLayerList);

                sb.AppendLine($"Adding layer:{i} of {numLayers}: ");
                foreach (GloMapTileCode code in nextLayerList)
                    sb.Append($"{code} ");

                // Set the next layer as the prev layer (which we loop through), clear the next layer
                prevLayerList = nextLayerList;
                nextLayerList = new();
            }

            // Now loop through the allCodesList to create the tiles
            foreach (GloMapTileCode code in allCodesList)
            {
                sb.AppendLine($"Progressing: Writing elevations for tile {code}");
                GloAppFactory.Instance.EleManager.PrepTile(code, true);
            }
        }

        // -------------------------------------------------

        // sb.AppendLine($"Elevation System Report:");
        // sb.AppendLine(GloAppFactory.Instance.EleSystem.Report());

        return sb.ToString();
    }
}