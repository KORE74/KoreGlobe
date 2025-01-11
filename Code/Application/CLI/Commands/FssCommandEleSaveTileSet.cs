using System.Collections.Generic;
using System.Text;

#nullable enable

// CLI Usage: ele prep <inEleFilename> <inTileCode> <inOutDir> <action>
// CLI Usage: ele prep c:/Util/GlobeLibrary_MapPrep/Europe/W005N50_UkCentral/Ele_BF_BF_50m.asc BF_BF C:/Util/_temp yes

public class FssCommandEleSaveTileSet : FssCommand
{
    public FssCommandEleSaveTileSet()
    {
        Signature.Add("ele");
        Signature.Add("savetileset");
    }

    public override string HelpString => $"{SignatureString} <TileCode> <levels>";

    public override string Execute(List<string> parameters)
    {
        StringBuilder sb = new StringBuilder();

        if (parameters.Count != 2)
        {
            return "FssCommandEleSaveTileSet.Execute -> parameter count error";
        }

        foreach (string param in parameters)
        {
            sb.AppendLine($"param: {param}");
        }
        // return sb.ToString();

        string inTilecode = parameters[0];

        bool validOperation = true;


        FssMapTileCode? tileCode = FssMapTileOperations.TileCodeFromString(inTilecode);
        if ((tileCode == null) || (!tileCode.IsValid()))
            validOperation = false;

        // double inMinLatDegs  = double.Parse(parameters[1]);
        // double inMinLonDegs  = double.Parse(parameters[2]);
        // double inMaxLatDegs  = double.Parse(parameters[3]);
        // double inMaxLonDegs  = double.Parse(parameters[4]);
        // int inLonRes         = int.Parse(parameters[5]);
        // int inLatRes         = int.Parse(parameters[6]);

        // sb.AppendLine($"Elevation ve Tile:");
        // sb.AppendLine($"- inEleFilename: {inEleFilename}");

        // FssLLBox llBox = new FssLLBox() {
        //     MinLatDegs = inMinLatDegs,
        //     MinLonDegs = inMinLonDegs,
        //     MaxLatDegs = inMaxLatDegs,
        //     MaxLonDegs = inMaxLonDegs };


        // -------------------------------------------------

        // // Convert and validate the inputs
        // if (!System.IO.File.Exists(inEleFilename))
        // {
        //     sb.AppendLine($"File not found: {inEleFilename}");
        //     validOperation = false;
        // }

        // -------------------------------------------------

        if (validOperation)
        {
            // FssLLBox llBox = tileCode.LLBox;
            // int latRes = 50;
            // int lonRes = FssElevationTileSystem.GetLonRes(latRes, llBox.CenterPoint.LatDegs);

            sb.AppendLine($"Valid operation: Progressing...");

            // sb.AppendLine($"Creating new tile: {tileCode.ToString()} {latRes} {lonRes}");

            FssAppFactory.Instance.EleManager.PrepTile(tileCode!, true);

            //FssElevationTileIO.WriteToTextFile(newTile, inEleFilename);
        }

        // -------------------------------------------------

        // sb.AppendLine($"Elevation System Report:");
        // sb.AppendLine(FssAppFactory.Instance.EleSystem.Report());

        //sb.AppendLine($"Tile saved to: {inEleFilename}");

        return sb.ToString();
    }
}