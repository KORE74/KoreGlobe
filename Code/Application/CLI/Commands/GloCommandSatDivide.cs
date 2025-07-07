using System.Collections.Generic;
using System.Text;
using System.IO;

// CLI Usage: sat collate AB

public class GloCommandSatDivide : GloCommand
{
    public GloCommandSatDivide()
    {
        Signature.Add("sat");
        Signature.Add("divide");
    }

    public override string HelpString => $"{SignatureString} <tilecode> <source image filename>";

    public override string Execute(List<string> parameters)
    {
        StringBuilder sb = new StringBuilder();

        if (parameters.Count != 2)
        {
            return "GloCommandEleSaveTile.Execute -> insufficient parameters";
        }

        foreach (string param in parameters)
        {
            sb.AppendLine($"param: {param}");
        }

        string tileCodeStr   = parameters[0];
        string inImgFilename = parameters[1];

        if (tileCodeStr == "world")
        {
            string worldTileFilepath = GloMapTileFilepaths.WorldTileFilepath();

            sb.AppendLine($"Collating child tile images for the world tile: {worldTileFilepath}");
            GloMapFileOperations.CollateChildTileImages_Lvl0();
        }
        else
        {
            bool validOperation = true;

            GloMapTileCode tileCode = GloMapTileCode.TileCodeFromString(tileCodeStr);
            if (!tileCode.IsValid())         { validOperation = false; sb.AppendLine("Invalid tile code."); }
            if (!File.Exists(inImgFilename)) { validOperation = false; sb.AppendLine("source image not found."); }

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

                GloMapFileOperations.DivideChildTileImages(tileCodeStr, inImgFilename);

                sb.AppendLine("Done.");
            }

            // -------------------------------------------------

            // sb.AppendLine($"Elevation System Report:");
            // sb.AppendLine(GloAppFactory.Instance.EleSystem.Report());
        }

        return sb.ToString();
    }
}