using System.Collections.Generic;
using System.IO;
using System.Text;


// CLI Usage: sat collate AB

public class GloCommandSatCollate : GloCommand
{
    public GloCommandSatCollate()
    {
        Signature.Add("sat");
        Signature.Add("collate");
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

        string tileCodeStr = parameters[0];

        bool validOperation = true;


        if (tileCodeStr == "world")
        {
            string worldTileFilepath = GloMapTileFilepaths.WorldTileFilepath();

            sb.AppendLine($"Collating child tile images for the world tile: {worldTileFilepath}");
            GloMapFileOperations.CollateChildTileImages_Lvl0();
            return sb.ToString();
        }
        else
        {


            GloMapTileCode tileCode = GloMapTileCode.TileCodeFromString(tileCodeStr);
            if (!tileCode.IsValid()) { validOperation = false; sb.AppendLine("Invalid tile code."); }

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
                sb.AppendLine($"Collating child tile images for tile {tileCodeStr}");

                GloMapFileOperations.CollateChildTileImages(tileCodeStr);

                sb.AppendLine("Done.");
            }

            // -------------------------------------------------

            // sb.AppendLine($"Elevation System Report:");
            // sb.AppendLine(GloAppFactory.Instance.EleSystem.Report());

        }

        return sb.ToString();
    }
}