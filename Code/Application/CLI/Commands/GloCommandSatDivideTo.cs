using System.Collections.Generic;
using System.Text;
using System.IO;

// CLI Usage: sat collate AB
using Godot;

#nullable enable


public class GloCommandSatDivideTo : GloCommand
{
    public GloCommandSatDivideTo()
    {
        Signature.Add("sat");
        Signature.Add("divideto");
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


        GloMapTileCode tileCode = GloMapTileCode.TileCodeFromString(tileCodeStr);
        if (!tileCode.IsValid()) { validOperation = false; sb.AppendLine("Invalid tile code."); }

        GloMapTileCode parentTileCode = tileCode.ParentCode();
        if (!parentTileCode.IsValid()) { validOperation = false; sb.AppendLine("Invalid parent tile code."); }

        // Load the parent tile image
        GloMapTileFilepaths parentFilepaths = new GloMapTileFilepaths(parentTileCode);

        if (!File.Exists(parentFilepaths.WebpFilepath))
        {
            sb.AppendLine($"File not found: {parentFilepaths.WebpFilepath}");
            validOperation = false;
        }

        if (validOperation)
        {
            Image? parentImage = GloImageOperations.LoadImage(parentFilepaths.WebpFilepath);

            if (parentImage == null)
            {
                sb.AppendLine($"Failed to load image: {parentFilepaths.WebpFilepath}");
                validOperation = false;
            }
            else
            {
                sb.AppendLine($"Loaded image: {parentFilepaths.WebpFilepath}");

                // Divide up the image into child tiles
                //GloMapFileOperations.DivideChildTileImages(tileCodeStr, parentImage!);

                Image[,] childImages = GloMapFileOperations.DivideIntoChildTileImages(parentTileCode, parentImage);
                Glo2DGridPos tilePos = parentTileCode.ChildPositionInParent();

                Image childImage = childImages[tilePos.PosX, tilePos.PosY];

                // Save the child image
                sb.AppendLine($"Saving:");
                sb.AppendLine($"- Child image size: {childImage.GetWidth()} x {childImage.GetHeight()}");
                sb.AppendLine($"- Child image pos: {tilePos.PosX} {tilePos.PosY}");
                sb.AppendLine($"- Child image file: {parentFilepaths.WebpFilepath}");

                // Now save out the child image
                GloMapTileFilepaths childFilepaths = new GloMapTileFilepaths(tileCode);
                childImage.SaveWebp(childFilepaths.WebpFilepath);
            }
        }

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

            //GloMapFileOperations.DivideChildTileImages(tileCodeStr, inImgFilename);

            sb.AppendLine("Done.");
        }

        // -------------------------------------------------

        // sb.AppendLine($"Elevation System Report:");
        // sb.AppendLine(GloAppFactory.Instance.EleSystem.Report());


        return sb.ToString();
    }
}