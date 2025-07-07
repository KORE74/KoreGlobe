using System.Collections.Generic;
using System.Text;


// CLI Usage: ele prep <inEleFilename> <inTileCode> <inOutDir> <action>
// CLI Usage: ele prep c:/Util/GlobeLibrary_MapPrep/Europe/W005N50_UkCentral/Ele_BF_BF_50m.asc BF_BF C:/Util/_temp yes


public class GloCommandEleLoadTile : GloCommand
{
    public GloCommandEleLoadTile()
    {
        Signature.Add("ele");
        Signature.Add("loadtile");
    }

    public override string HelpString => $"{SignatureString} <tilecode>";

    public override string Execute(List<string> parameters)
    {
        StringBuilder sb = new StringBuilder();

        if (parameters.Count != 1)
        {
            return "GloCommandEleSaveTile.Execute -> insufficient parameters";
        }

        foreach (string param in parameters)
        {
            sb.AppendLine($"param: {param}");
        }
        // return sb.ToString();

        string tileCodeStr = parameters[0];

        GloMapTileCode tileCode = GloMapTileCode.TileCodeFromString(tileCodeStr);
        GloMapTileFilepaths filepaths = new(tileCode);




        // double inMinLatDegs  = double.Parse(parameters[1]);
        // double inMinLonDegs  = double.Parse(parameters[2]);
        // double inMaxLatDegs  = double.Parse(parameters[3]);
        // double inMaxLonDegs  = double.Parse(parameters[4]);
        // int inLonRes         = int.Parse(parameters[5]);
        // int inLatRes         = int.Parse(parameters[6]);

        // // sb.AppendLine($"Elevation ve Tile:");
        // // sb.AppendLine($"- inEleFilename: {inEleFilename}");

        // GloLLBox llBox = new GloLLBox() {
        //     MinLatDegs = inMinLatDegs,
        //     MinLonDegs = inMinLonDegs,
        //     MaxLatDegs = inMaxLatDegs,
        //     MaxLonDegs = inMaxLonDegs };

        bool validOperation = true;

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
            sb.AppendLine($"Progressing: Writing elevations for tile {tileCode} to {filepaths.EleFilepath}");

            //sb.AppendLine($"Creating new tile: {llBox} {inLatRes} {inLonRes}");

            GloAppFactory.Instance.EleManager.LoadTile(tileCode);

            //GloElevationPatchIO.WriteToTextFile(newTile, inEleFilename);
        }

        // -------------------------------------------------

        // sb.AppendLine($"Elevation System Report:");
        // sb.AppendLine(GloAppFactory.Instance.EleSystem.Report());


        return sb.ToString();
    }
}